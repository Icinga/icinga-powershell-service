using System;
using System.Diagnostics;
using System.Threading;

namespace icinga_service.src.classes
{
    class Agent
    {
        private string ModulePath = "";
        private Process m_daemon  = null;
        private Thread m_alive    = null;
   
        public Agent(string ModulePath)
        {
            this.ModulePath = ModulePath;
            this.m_alive = new Thread(new ThreadStart(this.IsRunning));
        }

        private void WriteEventLog(string message, EventLogEntryType severity, int eventId)
        {
            EventLog eventLog = new EventLog("Application");
            eventLog.Source = "Icinga PowerShell Service";
            eventLog.WriteEntry(message, severity, eventId, 1);
        }

        private Process CreateProcess(string executable, string arguments)
        {
            Process process                          = new Process();
            process.StartInfo.FileName               = executable;
            process.StartInfo.Arguments              = arguments;
            process.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute        = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError  = true;

            return process;
        }

        private void IsRunning()
        {
            while (true) {
                if (this.m_daemon == null || this.m_daemon.HasExited == true) {
                    this.WriteEventLog(
                        "The PowerShell instances assigned to this service are no longer present. They either crashed or were terminated by the user. Stopping service.",
                        EventLogEntryType.Error,
                        515
                    );
                    Environment.Exit(1);
                }
                
                Thread.Sleep(2000);
            }
        }

        // Start the Agent on the listen socket
        public void StartAgent()
        {
            this.m_daemon = this.CreateProcess(
                "powershell.exe",
                string.Format(
                    "-Command Invoke-Command {0} Import-Module '{1}'; Use-Icinga; Start-IcingaPowerShellDaemon -RunAsService | Out-Null; {2}",
                    "{",
                    this.ModulePath,
                    "}"
                )
            );

            this.WriteEventLog(
                "Starting Icinga PowerShell Service Daemon.",
                EventLogEntryType.Information,
                101
            );
            this.m_daemon.Start();

            this.m_alive.Start();
        }

        // Stop the Agent on the listen socket
        public void StopAgent()
        {
            this.m_alive.Abort();

            try {
                if (this.m_daemon != null && this.m_daemon.HasExited == false) {
                    this.m_daemon.Kill();
                }
            } catch (Exception exception) {
                this.WriteEventLog(
                    string.Format(
                        "Failed to terminate Icinga PowerShell Service Daemon: {0}",
                        exception.Message
                    ),
                    EventLogEntryType.Error,
                    501
                );
            }
        }
    }
}
