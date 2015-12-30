using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    public class NetSmtpMailSender : IMailSender
    {
        public NetSmtpMailSender() {}
        //
        // ВНИМАНИЕ! SSL връзки на порт 465 не се поддържат нито от тази библиотека
        // нито от стандартната System.net.mail
        // защото не поддържат т.н. Implicit SSL. 
        //
 
        public void  Send(MailMessage msg, MailSenderConfiguration cfg)
        {
            var readyMessage = msg.ToSystemNetMailMessage();

            using (var client = new System.Net.Mail.SmtpClient())
            {
                if (cfg != null)
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(cfg.UserName, cfg.Password);
                    client.EnableSsl = cfg.EnableSsl;
                    client.Host = cfg.Host;
                    client.Port = cfg.Port;
                }

                client.Send(readyMessage);
            }
        }
    }
}
