using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace AppConfigLibrary.Configuration
{
    public class GenericConfigurationElementCollection<T> : ConfigurationElementCollection
        where T : ConfigurationElement, IConfigurationCollectionElement, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IConfigurationCollectionElement)element).GetElementKey();
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName
        {
            get { return (new T()).GetElementName(); }
        }

        public T this[int index]
        {

            get { return (T)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public T this[string s]
        {
            get { return (T)BaseGet(s); }
        }

        public bool ContainsKey(string key)
        {
            bool result = false;
            object[] keys = BaseGetAllKeys();
            foreach (object obj in keys)
            {
                if ((string)obj == key)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
