using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;
using log4net;
using EasyNetQ.Topology;

namespace RabbitMQPluginLibrary.Strategies
{
    public class RetryOperationStrategy : IErrorQueueProcessingStrategy
    {
        protected ILog log;
        protected IBus bus;

        public RetryOperationStrategy()
        {
            log = LogManager.GetLogger(GetType());
        }

        public void Init(Dictionary<string, string> settings)
        {
            string enableLoggingString;
            settings.TryGetValue("enableLogging", out enableLoggingString);
            bool enableLogging = false;
            if (enableLoggingString != null)
            {
                enableLogging = Boolean.Parse(enableLoggingString);
            }

            bus = RabbitHutch.CreateBus(settings["connectionString"],
                          x => x.Register<IEasyNetQLogger>(_ => new PluginLogger(log, enableLogging)));
        }

        public void ProcessError(EasyNetQ.SystemMessages.Error msg)
        {
            log.InfoFormat("Retrying message {0} {1}", msg.Exchange, msg.Message);

            bus.Advanced.Publish
                (
                    new Exchange(msg.Exchange),
                    msg.RoutingKey,
                    false,
                    false,
                    msg.BasicProperties,
                    Encoding.UTF8.GetBytes(msg.Message)
                );
        }
    }
}
