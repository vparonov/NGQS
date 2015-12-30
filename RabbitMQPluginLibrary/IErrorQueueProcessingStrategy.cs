using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQPluginLibrary
{
    public interface IErrorQueueProcessingStrategy
    {
        void Init(Dictionary<string, string> settings);
        void ProcessError(EasyNetQ.SystemMessages.Error msg);
    }
}
