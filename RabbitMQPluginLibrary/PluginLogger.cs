using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;
using log4net;

namespace RabbitMQPluginLibrary
{
    public class PluginLogger : IEasyNetQLogger 
    {
        protected ILog log;
        protected bool enableLogging;

        public PluginLogger(ILog _log, bool _enableLogging)
        {
            log = _log;
            enableLogging = _enableLogging;
        }

        public void DebugWrite(string format, params object[] args)
        {
            if (enableLogging)
            {
                log.DebugFormat(format, args);
            }
        }

        public void ErrorWrite(Exception exception)
        {
            if (enableLogging)
            {
                log.ErrorFormat("Exception: {0}", exception.ToString());
            }
        }

        public void ErrorWrite(string format, params object[] args)
        {
            if (enableLogging)
            {
                log.ErrorFormat(format, args);
            }
        }

        public void InfoWrite(string format, params object[] args)
        {
            if (enableLogging)
            {
                log.InfoFormat(format, args);
            }
        }
    }
}
