using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
//using DbEngine;

namespace GenMailServiceLibrary
{
    public class MailLogger : IMailLogger
    {
  //      private CDbEngine CDatabaseEngine { get; set; }
        public bool LogLocally { get; private set; }

        public MailLogger() 
        {
            this.LogLocally = true;
        }

        //public MailLogger(CDbEngine cDatabaseEngine)
        //{
        //    this.LogLocally = false;
        //    this.CDatabaseEngine = cDatabaseEngine;
        //}

        /// <summary>
        /// Logs a message either in the log or in the database, returns success bool value
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Log(MailMessage message)
        {
            ILog log = LogManager.GetLogger("GenService.GenService");

            try
            {
                if (LogLocally)
                {
                    var mailInfo = String.Format("- - - MAIL SENT - - - sender: {0} | reciever: {1} | subject: {2}",
                        message.From != null ? message.From.Address : message.Sender.Address,
                        String.Join(", ",  message.To.Select(m => m.Address).ToArray()),
                        message.Subject);
                    log.Info(mailInfo);
                }
                else
                {
                    this.InsertMailIntoLog(message);
                }

                return true;
            }
            catch (Exception ex) 
            {
                log.Error(String.Format("{0} - {1} at {2}", ex.Source, ex.Message, ex.StackTrace));
                return false;
            }
        }

        private void InsertMailIntoLog(MailMessage message)
        {
//            Dictionary<string, object> parameters = new Dictionary<string, object>(10);
//            parameters.Add("@EmailFrom", message.From.Address);
//            parameters.Add("@EmailTo", String.Join(", ", message.To.Select(m => m.Address).ToArray()));
//            parameters.Add("@BCC", String.Join(", ", message.BCC.Select(m => m.Address).ToArray()));
//            parameters.Add("@CC", String.Join(", ", message.CC.Select(m => m.Address).ToArray()));
//            parameters.Add("@Subject", message.Subject);
//            parameters.Add("@AttachmentList", String.Join(", ", message.Attachments.Select(a => a.Name).ToArray()));
//            parameters.Add("@Body", message.Body);
//            parameters.Add("@Sender", message.Sender != null ? message.Sender.Address : "");

//            string sql =
//                @"INSERT INTO [dbo].[WebGen_EmailLog]
//                   ([LogDate]
//                   ,[EmailFrom]
//                   ,[EmailTo]
//                   ,[BCC]
//                   ,[CC]
//                   ,[Subject]
//                   ,[AttachmentList]
//                   ,[Body]
//                   ,[Sender])
//                VALUES
//                   (GETDATE(),@EmailFrom,@EmailTo,@BCC,@CC,@Subject,@AttachmentList,@Body,@Sender)";

//            sql.ForEachRecord(this.CDatabaseEngine, parameters, x => { });
        }
    }
}
