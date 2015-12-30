using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using System.Configuration;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using EasyNetQ;

namespace GenMailServiceLibrary
{
    public class RabbitMQMailSender : IMailSender, IDisposable
    {
        public RabbitMQMailSender()
        {
        }

        public void Send(MailMessage msg, MailSenderConfiguration cfg)
        {
            var connectionString = cfg.BrokerConnectionString; //gen.EGenSettings.MQConnectionString;

            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.Publish(msg);
            }
        }

        public void Dispose()
        {
        
        }
    }
}
