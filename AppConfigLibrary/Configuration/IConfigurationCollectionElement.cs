using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfigLibrary.Configuration
{
    public interface IConfigurationCollectionElement
    {
        object GetElementKey();
        string GetElementName();
    }
}
