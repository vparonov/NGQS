using System;
using AppConfigLibrary.Configuration;
namespace NetGenQueueService.Configuration.Plugins
{
    interface IElement
    {
        string Alias { get; set; }
        string Assembly { get; set; }
        string Type { get; set; }
        int NumberOfInstances { get; set; }
        GenericConfigurationElementCollection<Settings> Settings { get; }
    }
}
