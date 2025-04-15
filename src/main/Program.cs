using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;

namespace IcingaForWindows
{
    static class Program
    {
        static void Main(string[] args)
        {
            // We should only accept one argument: The PowerShell Module path for imports
            string module = "";

            if (args.Length > 0) {
                module = args[0];
            }

            IcingaForWindows ifw = new IcingaForWindows(module);

            // Ensure our module path is valid
            if (ifw.DoesFrameworkExist() == false) {
                Environment.Exit(1);
            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] {
                ifw
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

