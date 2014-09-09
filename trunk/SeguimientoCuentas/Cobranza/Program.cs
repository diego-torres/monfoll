using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Cobranza
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            /*System.Diagnostics.EventLog eventLogService = new System.Diagnostics.EventLog();

            if (!System.Diagnostics.EventLog.SourceExists("Cobranza"))
            {
                System.Diagnostics.EventLog.CreateEventSource("Cobranza", "CobranzaLog");
            }

            eventLogService.Source = "Cobranza";
            eventLogService.Log = "CobranzaLog";

            eventLogService.WriteEntry("DEBUG ETL Process Execution BEGIN.");

            CommonAdminPaq.AdminPaqLib apl = new CommonAdminPaq.AdminPaqLib();
            apl.SetDllFolder();

            Process.Main process = new Process.Main();
            process.Execute(eventLogService);

            eventLogService.WriteEntry("DEBUG ETL Process Execution END.");*/
            
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new CobranzaService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
