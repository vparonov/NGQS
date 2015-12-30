using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using AppConfigLibrary.Configuration;

namespace NetGenQueueService.Configuration.Plugins
{
    class Element : ConfigurationElement, IConfigurationCollectionElement, IElement
    {
        [ConfigurationProperty("alias", IsKey = true, IsRequired = true)]
        public string Alias
        {
            get { return (string)this["alias"]; }
            set { this["alias"] = value; }
        }

        [ConfigurationProperty("assembly", IsKey = false, IsRequired = true)]
        public string Assembly
        {
            get { return (string)this["assembly"]; }
            set { this["assembly"] = value; }
        }

        [ConfigurationProperty("type", IsKey = false, IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("number-of-instances", IsKey = false, IsRequired = false, DefaultValue = 1)]
        public int NumberOfInstances
        {
            get { return (int)this["number-of-instances"]; }
            set { this["number-of-instances"] = value; }
        }


        [ConfigurationProperty("settings", IsDefaultCollection = true)]
        public GenericConfigurationElementCollection<Settings> Settings
        {

            get { return (GenericConfigurationElementCollection<Settings>)base["settings"]; }

        }

        public object GetElementKey() { return Alias; }
        public string GetElementName() { return "plugin"; }

    }
}
