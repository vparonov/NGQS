using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GenMailServiceLibrary;
using log4net;
using EasyNetQ;

namespace RabbitMQPluginLibrary.Strategies
{
    public class SendToEMailStrategy : IErrorQueueProcessingStrategy
    {
        protected ILog log;
        protected IBus bus;

        private List<MailAddress> receivers;
        private MailAddress from;

        private string rabbitMQHost;
        private string rabbitMQVirtualHost;

        public SendToEMailStrategy()
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

            // From:xxxxxx,To:yyyyy;zzzzz
            var pars = settings["SS_SendToEMail"].Split(',');

            from = pars[0].Split(':')[1].ToMailAddress() ;

            receivers = pars[1]
                    .Split(':')
                    [1]
                    .Split(';')
                    .Select(s => s.ToMailAddress())
                    .ToList() ;

            var parsedConnectionString = settings["connectionString"]
                .Split(';')
                .Select(s =>
                    {
                        var ar = s.Split('=');
                        return new 
                        { 
                            Key = ar[0].Trim().ToUpper(), 
                            Value = ar[1].Trim().ToUpper() 
                        };
                    });

            rabbitMQHost = parsedConnectionString
                .Where(kvp => kvp.Key == "HOST")
                .SingleOrDefault()
                .Value ;

            rabbitMQVirtualHost = parsedConnectionString
                .Where(kvp => kvp.Key == "VIRTUALHOST")
                .SingleOrDefault()
                .Value ;                 
        }

        public void ProcessError(EasyNetQ.SystemMessages.Error msg)
        {
            log.InfoFormat("Sending message to mail {0} {1}", msg.Exchange, msg.Message);

            var mailMsg = new MailMessage();

            mailMsg.From = from;
            mailMsg.To = receivers;
            mailMsg.Subject = String.Format("[NetGenQueueService] [{0}] [{1}]", rabbitMQHost, rabbitMQVirtualHost);
            mailMsg.Body = msg.Exception.Replace("\\r\\n", "\n");
            mailMsg.Attachments.Add(new Attachment(Encoding.UTF8.GetBytes(msg.Message), "Payload.txt", "text/plain"));

            bus.Publish(mailMsg);
        }
    }
}

