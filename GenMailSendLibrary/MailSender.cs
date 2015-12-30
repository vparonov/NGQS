using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GenMailServiceLibrary
{
    public class MailSender
    {
        public MailSender(IMailSender sender = null, IMailLogger logger = null) 
        {
            Sender = sender ?? new NetSmtpMailSender();
            Logger = logger ?? new MailLogger();
        }

        public MailSender(MailSenderConfiguration cfg, IMailSender sender = null, IMailLogger logger = null) 
        {
            Sender = sender ?? new NetSmtpMailSender();
            Logger = logger ?? new MailLogger();
            Configuration = cfg;
        }

        public MailSenderConfiguration Configuration { set; get; }
        public IMailSender Sender { set; get; }
        public IMailLogger Logger { set; get; }
 
        public void Send(MailMessage msg)
        {
            //if (msg.From == null)
            //{
            //    msg.From = msg.Sender;
            //}

            Sender.Send(msg, Configuration);
            Logger.Log(msg);
        }
    }
}
