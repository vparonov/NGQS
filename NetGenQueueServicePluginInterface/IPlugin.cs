using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetGenQueueService
{
    public interface IPlugin
    {
        void Init(string instanceAliasName, int instanceID, string[] args, Dictionary<string, string> settings);
        void Start(CancellationTokenSource cancelationTokenSource);
        void Stop();
    }
}
