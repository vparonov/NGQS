using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using AppConfigLibrary.Configuration;

namespace NetGenQueueService.Configuration.SharedSettings
{
    public class Section : ConfigurationSection
    {
        public Section()
        {

        }
        [ConfigurationProperty("settings", IsDefaultCollection = true)]
        public GenericConfigurationElementCollection<Element> Settings
        {

            get { return (GenericConfigurationElementCollection<Element>)base["settings"]; }

        }
    }
}
