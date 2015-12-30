using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace NetGenQueueService
{
    [RunInstaller(true)]
    public partial class NetGenQueueServiceInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller; 

        public NetGenQueueServiceInstaller()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            this.serviceInstaller.ServiceName = "NetGenQueueService";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(new System.Configuration.Install.Installer[] 
                { 
                    this.serviceProcessInstaller, 
                    this.serviceInstaller
                }
            );
        }

        public override void Install(IDictionary stateSaver)
        {
            if (Context.Parameters.ContainsKey("ServiceName"))
            {
                serviceInstaller.ServiceName = Context.Parameters["ServiceName"];

                stateSaver.Add("ServiceName", Context.Parameters["ServiceName"]);
            }

            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            restoreState(savedState);

            base.Uninstall(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            restoreState(savedState);

            base.Commit(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            restoreState(savedState);

            base.Rollback(savedState);
        }

        private void restoreState(IDictionary savedState)
        {
            if (savedState.Contains("ServiceName"))
            {
                serviceInstaller.ServiceName = (string)savedState["ServiceName"];
            }
        }

    }
}
