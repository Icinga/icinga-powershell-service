using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using IcingaForWindows.src.classes;

namespace IcingaForWindows
{
    public partial class IcingaForWindows : ServiceBase
    {
        private string m_modulePath = "";
        private Agent m_agent       = null;
   
        public IcingaForWindows(string ModulePath)
        {
            InitializeComponent();
            this.m_modulePath = ModulePath;
            this.m_agent      = new Agent(this.m_modulePath);
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
