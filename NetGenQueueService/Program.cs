using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetGenQueueService.ServiceHelpers;
using System.ServiceProcess;

namespace NetGenQueueService
{
    class Program
    {
        public const string ServiceName = "NetGenQueueService";

        static void Main(string[] args)
        {
            using (var service = new Service(ServiceName))
            {
                // running as service
                if (!Environment.UserInteractive)
                {
                    ServiceBase.Run(service);
                }
                else
                {
                    // running as console app
                    service.StartService(args);

                    Console.WriteLine("Press any key to stop...");
                    Console.ReadKey(true);

                    service.StopService();
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
            }
        }
    }
}
