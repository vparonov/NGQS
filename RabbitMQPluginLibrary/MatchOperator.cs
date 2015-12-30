using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabbitMQPluginLibrary
{
    public enum MatchOperator
    {
        Equal = 0, 
        LessThan = 1,
        LessThanOrEqual = 2, 
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        NotEqual = 5
    }
}
