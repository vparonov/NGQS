using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HigLabo.Net.Smtp;

namespace GenMailServiceLibrary
{
    public class HigLaboSmtpMailSender : IMailSender
    {
        public HigLaboSmtpMailSender() { }


        public void Send(MailMessage msg, MailSenderConfiguration cfg)
        {
            var realCfg = cfg ?? createConfigurationFromAppSettings();
            SmtpClient cl = new SmtpClient();

            cl.ServerName = realCfg.Host;
            cl.Port = realCfg.Port;
            cl.AuthenticateMode = SmtpAuthenticateMode.Plain;
            //cl.Ssl = true;
            cl.Tls = realCfg.EnableSsl;
            cl.UserName = realCfg.UserName;
            cl.Password = realCfg.Password;
            cl.HostName = realCfg.HostName;

            var mg = msg.ToSystemHigLaboSmtpMessage();

            var rs = cl.SendMail(mg); // 

            if (rs.SendSuccessful == false)
            {
                // TODO - add error handling here!
                throw new Exception(String.Format("Error! Mail to {0} with subject = {1} was not sent! {2}", msg.To.ToString(), msg.Subject, rs.State.ToString())) ;
            }
        }

        private MailSenderConfiguration createConfigurationFromAppSettings()
        {
            var retVal = new MailSenderConfiguration();

//            var config = ConfigurationSettings.GetConfig( .OpenExeConfiguration((ConfigurationUserLevel.None);

            var mailSettings = ConfigurationManager.GetSection("system.net/mailSettings/smtp")
                as System.Net.Configuration.SmtpSection;

            if (mailSettings != null)
            {
                retVal.Host = mailSettings.Network.Host;
                retVal.Port = mailSettings.Network.Port;
                retVal.EnableSsl = mailSettings.Network.EnableSsl;
                retVal.UserName = mailSettings.Network.UserName;
                retVal.Password = mailSettings.Network.Password;
                retVal.HostName = ConfigurationManager.AppSettings["HostName"] ?? "localhost";
            }
            return retVal;
        }
    }
}
