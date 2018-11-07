using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using icinga_service.src.classes;

namespace icinga_service
{
    public partial class Icinga2 : ServiceBase
    {
        private string ModulePath = "";
        private Agent m_agent     = null;
   
        public Icinga2(string ModulePath)
        {
            InitializeComponent();
            this.ModulePath = ModulePath;
            this.m_agent = new Agent(this.ModulePath);
        }

        protected override void OnStart(string[] args)
        {
            this.m_agent.StartAgent();
        }

        protected override void OnStop()
        {
            this.m_agent.StopAgent();
        }
    }
}
