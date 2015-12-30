using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using AppConfigLibrary.Configuration;

namespace NetGenQueueService.Configuration.Plugins
{
    class Section : ConfigurationSection
    {

        public Section()
        {

        }
        [ConfigurationProperty("plugins", IsDefaultCollection = true)]
        public GenericConfigurationElementCollection<Element> Plugins
        {

            get { return (GenericConfigurationElementCollection<Element>)base["plugins"]; }

        }
    }
}
