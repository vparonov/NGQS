using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace RabbitMQPluginLibrary.Strategies
{
    public class SaveToDiskStrategy : IErrorQueueProcessingStrategy
    {
        protected ILog log;
        private string saveToFolderName;

        public SaveToDiskStrategy()
        {
            log = LogManager.GetLogger(GetType());
        }

        public void Init(Dictionary<string, string> settings)
        {
            saveToFolderName = settings["SS_SaveToDisk"];
        }

        public void ProcessError(EasyNetQ.SystemMessages.Error msg)
        {
            log.InfoFormat("Saving message to disk {0} {1}", msg.Exchange, msg.Message);

            var fileName = generateFileName(msg);

            using (StreamWriter file = new StreamWriter(fileName))
            {
                file.WriteLine(msg.Message);
            }
        }

        private string generateFileName(EasyNetQ.SystemMessages.Error msg)
        {
            return 
                Path.Combine(saveToFolderName,
                    String.Format("{0}-{1}-{2}.txt",
                        msg.Exchange.Replace(':', '_').Replace('.', '_'),
                        msg.BasicProperties.CorrelationId, 
                        DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")) 
                );
        }
    }
}
