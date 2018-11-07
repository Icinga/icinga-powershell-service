using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace icinga_service
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
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Icinga2(module)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
