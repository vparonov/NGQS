using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using log4net;
using System.Threading.Tasks;
using NetGenQueueService.Configuration.Plugins;
using System.Threading;

namespace NetGenQueueService.ServiceHelpers
{
    public class Service : ServiceBase
    {
        internal class PluginWrapper
        {
            public IPlugin Plugin { get; set; }
            public string Alias { get; set; }
            public int InstanceID { get; set; }
            public ManualResetEvent MRE { get; set; }
            public Dictionary<string, string> Settings { get; set; }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<Task> pluginTasks = new List<Task>();
        private List<PluginWrapper> plugins = new List<PluginWrapper>();
        private static CancellationTokenSource cancelationTokenSource = new CancellationTokenSource();
        private bool isRunning = false;

        Action<object> taskAction = (object obj) =>
            {
                var plugin = (obj as PluginWrapper);
                do
                {
                    try
                    {
                        plugin.Plugin.Start(cancelationTokenSource);
                    }
                    catch (Exception ex)
                    {
                        var t = ex.GetType();
                        log.ErrorFormat("Exception caught in plugin {0}/{1}, {2}", plugin.Alias, plugin.InstanceID, ex.ToString());
                        log.InfoFormat("Restarting the plugin in 10 seconds {0}/{1}", plugin.Alias, plugin.InstanceID);
                        Thread.Sleep(10000);
                        continue;
                    }
                    plugin.MRE.WaitOne();
                    break;
                }
                while (true);  
            };

        public Service(string serviceName)
        {
            ServiceName = serviceName;
        }

        private void initPlugins()
        {
            var pluginSection = (Section)System.Configuration.ConfigurationManager.GetSection("pluginsettings");

            var sharedSettings = readSharedSettings();
           

            for (var i = 0; i < pluginSection.Plugins.Count; i++)
            {
                var pe = (pluginSection.Plugins[i] as IElement);

                for (var j = 1 ; j <= pe.NumberOfInstances ; j++)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(pe.Assembly, pe.Type).Unwrap();
                    var mre = new ManualResetEvent(false);
                    var wrapper = new PluginWrapper() 
                            { 
                                Plugin = plugin, 
                                Alias = pe.Alias, 
                                InstanceID = j, 
                                MRE = mre,
                                Settings = convertToDictionary(pe.Settings, sharedSettings)
                            };

                    var t = new Task(taskAction, wrapper, cancelationTokenSource.Token);
                    plugins.Add(wrapper);

                    pluginTasks.Add(t);
                }
            }

        }

        private Dictionary<string, string> readSharedSettings()
        {
            var res = new Dictionary<string, string>();

            var sharedSettingsSection = (NetGenQueueService.Configuration.SharedSettings.Section)
                               System.Configuration.ConfigurationManager.GetSection("sharedSettings");

            if (sharedSettingsSection == null)
            {
                return res;
            }

            for (var i = 0; i < sharedSettingsSection.Settings.Count; i++)
            {
                res.Add(sharedSettingsSection.Settings[i].Key, sharedSettingsSection.Settings[i].Value);
            }

            return res;
        }

        private Dictionary<string, string> convertToDictionary(
            AppConfigLibrary.Configuration.GenericConfigurationElementCollection<Settings> settings,
            Dictionary<string, string> sharedSettings)
        {
            var res = new Dictionary<string, string>();
            
            for (var i = 0; i < settings.Count; i++)
            {
                var value = settings[i].Value ;

                if (value.Length > 7 && value.Substring(0, 7) == "shared!")
                {
                    var sharedKey = value.Substring(7) ;
                    value = sharedSettings[sharedKey] ;
                }

                res.Add(settings[i].Key, value);
            }
            return res;
        }

        protected override void OnStart(string[] args)
        {
            StartService(args);
        }

        protected override void OnStop()
        {
            StopService();
        }

        public void StartService(string[] args)
        {
            if (isRunning)
            {
                StopService();
            }

            isRunning = true;
            log.Info("Start service");
            log.Info("Initialization of plugins");
            initPlugins();

            foreach (var p in plugins)
            {
                p.Plugin.Init(p.Alias, p.InstanceID, args, p.Settings);
            }

            foreach (var t in pluginTasks)
            {
                t.Start();
            }
        }

        public void StopService()
        {
            log.Info("Stop service");
            cancelationTokenSource.Cancel();

            foreach (var p in plugins)
            {
                log.InfoFormat("Stopping {0}/{1}", p.Alias, p.InstanceID);
                p.Plugin.Stop();
                p.MRE.Set();
            }

            pluginTasks.Clear();
            plugins.Clear();
            isRunning = false;
        }
    }
}
