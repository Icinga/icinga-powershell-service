using System;
using System.Collections.Generic;
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
            string JEA    = "";

            if (args.Length > 0) {
                module = args[0];
            }

            if (args.Length > 1) {
                JEA = args[1];
            }
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] {
                new IcingaForWindows(module, JEA)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

