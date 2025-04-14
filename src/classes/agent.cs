using System;
using System.Diagnostics;
using System.Threading;

namespace IcingaForWindows.src.classes
{
    class Agent
    {
        private string m_modulePath = "";
        private Process m_daemon    = null;
        private Thread m_alive      = null;
   
        public Agent(string ModulePath)
        {
            this.m_modulePath = ModulePath;
            this.m_alive = new Thread(new ThreadStart(this.IsRunning));
        }

        private void WriteEventLog(string message, EventLogEntryType severity, int eventId)
        {
            // Icinga for Windows >= v1.8.0
            string EventLogSource = "IfW::Service";

            try {
                if (EventLog.SourceExists(EventLogSource) == false) {
                    // Icinga for Windows < v1.8.0
                    EventLogSource = "Icinga for Windows";

                    if (EventLog.SourceExists(EventLogSource) == false) {
                        // There is no place to write EventLog information to
                        return;
                    }
                }
            } catch {
                try  {
                    // Icinga for Windows < v1.8.0
                    EventLogSource = "Icinga for Windows";

                    if (EventLog.SourceExists(EventLogSource) == false) {
                        // There is no place to write EventLog information to
                        return;
                    }
                } catch {
                    return;
                }
            }

            EventLog eventLog = new EventLog();
            eventLog.Source   = EventLogSource;
            eventLog.WriteEntry(message, severity, eventId, 1);
        }

        private Process CreateProcess(string executable, string arguments)
        {
            Process process                          = new Process();
            process.StartInfo.FileName               = executable;
            process.StartInfo.Arguments              = arguments;
            process.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute        = false;
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.RedirectStandardError  = false;

            return process;
        }

        private void IsRunning()
        {
            while (true) {
                if (this.m_daemon == null || this.m_daemon.HasExited == true) {
                    this.WriteEventLog( 
                        "The Icinga for Windows PowerShell instance assigned to this service is no longer present. It either crashed or was terminated by the user. Stopping service.",
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
            string PowerShellArgs = "";

            PowerShellArgs = string.Format(
                "-NoProfile -NoLogo -Command Invoke-Command {{ Import-Module '{0}'; Use-Icinga -Daemon; if (Test-IcingaFunction -Name 'Start-IcingaForWindowsDaemon') {{ Start-IcingaForWindowsDaemon -RunAsService | Out-Null; }} else {{ Start-IcingaPowerShellDaemon -RunAsService | Out-Null; }} }}",
                this.m_modulePath
            );

            this.m_daemon = this.CreateProcess(
                "powershell.exe", PowerShellArgs
            );

            this.WriteEventLog(
                string.Format("Starting Icinga for Windows service with arguments '{0}'", PowerShellArgs),
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
                        "Failed to terminate Icinga for Windows Service: {0}",
                        exception.Message
                    ),
                    EventLogEntryType.Error,
                    501
                );
            }
        }
    }
}
