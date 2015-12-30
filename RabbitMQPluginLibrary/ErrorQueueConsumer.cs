using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetGenQueueService;
using log4net;
using EasyNetQ;
using EasyNetQ.Topology;
using System.IO;
using Microsoft.Isam.Esent.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQPluginLibrary
{
    public class ErrorQueueConsumer : IPlugin, IDisposable
    {
        protected ILog log;
        protected IBus bus;
        protected IQueue queue;
        protected int _instanceID;
        protected string _instanceAliasName;
        protected string persistenceFolder;
        protected string instancePersistenceFolder;

        protected Dictionary<string, ErrorQueueConsumerSettingsRegistry> settingsDictionary =
            new Dictionary<string, ErrorQueueConsumerSettingsRegistry>();

        public ErrorQueueConsumer()
        {
            log = LogManager.GetLogger(GetType());
            persistenceFolder = "ErrorQueueConsumer";
        }

        public void Init(string instanceAliasName, int instanceID, string[] args, Dictionary<string, string> settings)
        {
            var errorQueueName = new Conventions(new TypeNameSerializer()).ErrorQueueNamingConvention();

            _instanceID = instanceID;
            _instanceAliasName = instanceAliasName;

            initStrategyRegistry(settings);

            string s;

            if (settings.TryGetValue("persistenceFolder", out s))
            {
                persistenceFolder = Environment.ExpandEnvironmentVariables(s);
            }

            instancePersistenceFolder = Path.Combine(persistenceFolder, _instanceAliasName);

            string enableLoggingString;
            settings.TryGetValue("enableLogging", out enableLoggingString);
            bool enableLogging = false;
            if (enableLoggingString != null)
            {
                enableLogging = Boolean.Parse(enableLoggingString);
            }

            bus = RabbitHutch.CreateBus(settings["connectionString"],
                          x => x.Register<IEasyNetQLogger>(_ => new PluginLogger(log, enableLogging)));

            queue = bus.Advanced.QueueDeclare(errorQueueName);
        }

        private void initStrategyRegistry(Dictionary<string, string> settings)
        {
            var ss = 
                settings
                .Keys
                .Where(s => s.Length > 3 && s.Substring(0, 3) == "EH-")
                .Select(s => 
                    { 
                        // EH-[type]-[operator]-[retrycount]-WithDelay-[delay_in_seconds]
                        // EH-CommandLineCommand-LessThan-5-WithDelay-10
                        // operator по подразбиране е GreaterThanOrEqual
                        // retrycount по поздразбиране е = 1
                        // delay_in_seconds по подразбиране = 0 
                        var elements = s.Split('-') ;
                        return new 
                        { 
                                Key = elements[1], 
                                MatchOperator = elements.Length > 2 ? (MatchOperator)Enum.Parse(typeof(MatchOperator), elements[2]) : MatchOperator.GreaterThanOrEqual, 
                                RetryCount = elements.Length > 3 ? Int32.Parse(elements[3]) : 1, 
                                Delay = elements.Length > 5 ? Int32.Parse(elements[5]) : 0, 
                                Value = settings[s] 
                        }; 
                    });
            
            foreach (var s in ss)
            {
                if (!settingsDictionary.ContainsKey(s.Key))
                {
                    settingsDictionary.Add(s.Key, new ErrorQueueConsumerSettingsRegistry());
                }

                foreach (var e in s.Value.Split(','))
                {
                    settingsDictionary[s.Key].RegisterStrategy(s.MatchOperator, s.RetryCount, s.Delay, e, settings);
                }
            }


        }

        public void Start(CancellationTokenSource cancelationTokenSource)
        {
            //асинхронен хендлър - Consume<T>(IQueue queue, Func<IMessage<T>, MessageReceivedInfo, Task> onMessage)
            //Consume(queue, (body, properties, info) => Task.Factory.StartNew(() =>
            //{
                //var message = Encoding.UTF8.GetString(body);
                //Console.WriteLine("Got message: '{0}'", message);
            //}));
            //bus.Advanced.Consume(

            //синхронен хендлър - Consume<T>(IQueue queue, Action<IMessage<T>, MessageReceivedInfo> on Message)
            bus.Advanced.Consume<EasyNetQ.SystemMessages.Error>(queue, (message, info) => 
            {
                try
                {
                    using (var processedErrorMessages = new PersistentDictionary<string, int>(instancePersistenceFolder))
                    {

                        var key = message.Body.BasicProperties.CorrelationId;
                        int retryCount = 1;
                        if (processedErrorMessages.ContainsKey(key))
                        {
                            retryCount = processedErrorMessages[key];
                            processedErrorMessages[key] = ++retryCount;
                        }
                        else
                        {
                            processedErrorMessages.Add(key, retryCount);
                        }

                        
                        var msgBody = message.Body;

                        var className = getClassNameFromExchange(msgBody.Exchange);

                        log.InfoFormat(
                            "Processing ErrorQueue Message. DateTime = {0}, RetryCount = {1}, Message = {2}", 
                            msgBody.DateTime,
                            retryCount,
                            msgBody.Message);

                        applyStrategies(className, retryCount, msgBody);
                    }
                }
                catch (Exception ex)
                {
                    if (!cancelationTokenSource.IsCancellationRequested)
                    {
                        log.ErrorFormat("ERROR Consuming message from ErrorQueue! {0}", ex);
                    }
                }          
            });
        }

        private void applyStrategies(string className, int retryCount, EasyNetQ.SystemMessages.Error msg)
        {
            var settingsKey = "Default";

            if (settingsDictionary.ContainsKey(className))
            {
                settingsKey = className;
            }

            var registry = settingsDictionary[settingsKey];

            var strategies = registry.MatchStrategies(retryCount);

            foreach (var strategy in strategies)
            {
                var local_strategy = strategy;
                Task.Factory.StartNew(() =>
                    {
                        if (local_strategy.DelayInSeconds > 0)
                        {
                            log.InfoFormat("Waiting for {0} seconds", local_strategy.DelayInSeconds);
                            Thread.Sleep(local_strategy.DelayInSeconds * 1000);
                        }
                        log.InfoFormat("Applying error queue processing strategy {0}", local_strategy.StrategyClassName);
                        local_strategy.ErrorQueueProcessingStrategy.ProcessError(msg);
                    }
                );
            }
        }

        
        private string getClassNameFromExchange(string exchange)
        {
            //NB: очевидно е, че ако публикуваме класове с еднакво име, който са в друг namespace
            //    или в друго assembly, е възможно да се получи колизия
            //    надявам се на никой да не му дойде на ума да прави подобна глупост
            //    exchange е с формата:
            //    namespace.classname:assembly
            //    от него връщаме classname

            //exchange = SandboxPlugin.CommandLineCommand:SandboxPlugin
            return exchange.Split(':')[0].Split('.')[1] ;
        }

        public void Stop()
        {
            if (bus.IsConnected)
            {
                bus.Dispose();
            }
            bus = null;
        }

        public void Dispose()
        {
            if (bus != null)
            {
                bus.Dispose();
            }
        }
    }
}
