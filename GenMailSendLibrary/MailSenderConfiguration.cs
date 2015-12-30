using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    public class MailSenderConfiguration
    {
        public string Host { set; get; }
        public int Port { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public bool EnableSsl { set; get; }
        public string HostName { set; get; }
        public string BrokerConnectionString { set; get; }
    }
}
