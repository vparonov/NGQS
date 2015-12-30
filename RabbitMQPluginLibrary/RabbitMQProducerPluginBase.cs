using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGenQueueService;
using log4net;
using System.Configuration;
using EasyNetQ;
using System.Threading;

namespace RabbitMQPluginLibrary
{
    public class RabbitMQProducerPluginBase : IPlugin, IDisposable 
    {
        protected ILog log;

        protected IBus bus;
        protected int _instanceID;
        protected string _instanceAliasName;

        public RabbitMQProducerPluginBase() 
        {
            log = LogManager.GetLogger(GetType());
        }

        public void Init(string instanceAliasName, int instanceID, string[] args, Dictionary<string, string> settings)
        {
            _instanceID = instanceID;
            _instanceAliasName = instanceAliasName;

            string enableLoggingString;
            settings.TryGetValue("enableLogging", out enableLoggingString);
            bool enableLogging = false;
            if (enableLoggingString != null)
            {
                enableLogging = Boolean.Parse(enableLoggingString);
            }

            bus = RabbitHutch.CreateBus(settings["connectionString"],
                x => x.Register<IEasyNetQLogger>(_ => new PluginLogger(log, enableLogging)));
            OnInit(args, settings) ;
        }

        public void Start(CancellationTokenSource cancelationTokenSource)
        {
            // не се правят никакви проверки дали сме възани към rabbit, 
            // защото реално самата връзка е lazzy и е възможно да се случи
            // при първото използване, като самата библиотека
            // се грижи да я поддържа 'жива'
            Produce(cancelationTokenSource);
        }

        protected virtual void Produce(CancellationTokenSource cancelationTokenSource)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            bus.Dispose();
            bus = null;
        }

        public void Dispose()
        {
            if (bus != null)
            {
                bus.Dispose();
            }
        }

        protected virtual void OnInit(string[] args, Dictionary<string, string> settings) 
        {

        }

    }
}
