using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RabbitMQPluginLibrary
{
    public class ErrorQueueConsumerSetting
    {
        private static Dictionary<string, Type> strategies;

        public MatchOperator MatchOperator;
        public int RetryCount;
        public int DelayInSeconds;
        public string StrategyClassName;
        public IErrorQueueProcessingStrategy ErrorQueueProcessingStrategy;

        public ErrorQueueConsumerSetting()
        {

        }

        private ErrorQueueConsumerSetting(MatchOperator mo, int rc, int delayInSeconds, IErrorQueueProcessingStrategy eqps)
        {
            MatchOperator = mo;
            RetryCount = rc;
            ErrorQueueProcessingStrategy = eqps;
            DelayInSeconds = delayInSeconds;
        }

        public ErrorQueueConsumerSetting(MatchOperator mo, int rc, int delayInSeconds, string errorQueueProcessingStrategyType)
            : this(mo, rc, delayInSeconds, findStrategyByName(errorQueueProcessingStrategyType))
        {
            StrategyClassName = errorQueueProcessingStrategyType;
        }

      
        private static IErrorQueueProcessingStrategy findStrategyByName(string errorQueueProcessingStrategyType)
        {
            initStrategiesDict();

            return (Activator.CreateInstance(strategies[generateTypeName(errorQueueProcessingStrategyType)]) as IErrorQueueProcessingStrategy);
        }

        private static string generateTypeName(string errorQueueProcessingStrategyType)
        {
            return String.Format("{0}Strategy", errorQueueProcessingStrategyType);
        }

        private static void initStrategiesDict()
        {
            if (strategies != null)
            {
                return;
            }

            strategies = new Dictionary<string, Type>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var typeofIErrorQueueProcessingStrategy = typeof(IErrorQueueProcessingStrategy) ;

            foreach (var asm in assemblies)
            {
                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }

                foreach (var t in types
                    .Where(
                        t => t != null 
                            && typeofIErrorQueueProcessingStrategy.IsAssignableFrom(t)
                            && typeofIErrorQueueProcessingStrategy != t))
                {
                    if (!strategies.ContainsKey(t.Name))
                    {
                        strategies.Add(t.Name, t);
                    }
                }
            }

        }

        public bool Match(int retryCount)
        {
            switch (MatchOperator)
            {
                case MatchOperator.Equal:
                    return retryCount == RetryCount;
                case MatchOperator.LessThan:
                    return retryCount < RetryCount;
                case MatchOperator.LessThanOrEqual:
                    return retryCount <= RetryCount;
                case MatchOperator.GreaterThan:
                    return retryCount > RetryCount;
                case MatchOperator.GreaterThanOrEqual:
                    return retryCount >= RetryCount;
                case MatchOperator.NotEqual:
                    return retryCount != RetryCount;
            }
            return false;
        }
    }
}
