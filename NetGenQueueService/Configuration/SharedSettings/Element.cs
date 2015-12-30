using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using AppConfigLibrary.Configuration;

namespace NetGenQueueService.Configuration.SharedSettings
{
    public class Element : ConfigurationElement, IConfigurationCollectionElement
    {
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", IsKey = false, IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        public object GetElementKey() { return Key; }
        public string GetElementName() { return "param"; }
    }
}
