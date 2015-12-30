using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQPluginLibrary
{
    public interface IWorker<T, U>
    {
        int ID { get; set; }
        U Execute(T param); 
        void ExecuteVoid(T param);
    }
}
