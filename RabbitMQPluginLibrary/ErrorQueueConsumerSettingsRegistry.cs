using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQPluginLibrary
{
    public class ErrorQueueConsumerSettingsRegistry
    {
        List<ErrorQueueConsumerSetting> settings = new List<ErrorQueueConsumerSetting>();

        public ErrorQueueConsumerSettingsRegistry()
        {

        }

        public void RegisterStrategy(MatchOperator mo, 
            int rc, 
            int delayInSeconds, 
            string errorQueueProcessingStrategyType, 
            Dictionary<string, string> plugin_settings)
        {
            var newItem = new ErrorQueueConsumerSetting(mo, rc, delayInSeconds, errorQueueProcessingStrategyType);

            newItem.ErrorQueueProcessingStrategy.Init(plugin_settings);
            settings.Add(newItem);
        }

        public IEnumerable<ErrorQueueConsumerSetting> MatchStrategies(int retryCount)
        {
            return settings.Where(s => s.Match(retryCount)); 
        }
    }
}
