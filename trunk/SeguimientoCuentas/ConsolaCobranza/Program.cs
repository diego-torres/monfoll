using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Configuration;
using System.Collections.Specialized;

namespace ConsolaCobranza
{
    class Program
    {
        static void Main(string[] args)
        {

            EventLog eventLogService = new EventLog();

            if (!EventLog.SourceExists("Consola Cobranza"))
            {
                EventLog.CreateEventSource("Consola Cobranza", "CobCmdLog");
            }

            eventLogService.Source = "Consola Cobranza";
            eventLogService.Log = "CobCmdLog";

            eventLogService.WriteEntry("CONSOLE EXECUTION BEGIN.");

            if (args.Length == 0)
            {
                eventLogService.WriteEntry("Console usage: NO ARGUMENTS FOUND.", EventLogEntryType.Error, 1, 0);
                return;
            }

            CommonAdminPaq.AdminPaqLib apl = new CommonAdminPaq.AdminPaqLib();
            string baseDir = ConfigurationManager.AppSettings["libDir"].ToString();

            eventLogService.WriteEntry("Setting baseDir as [" + baseDir + "]");
            apl.SetDllFolder(baseDir);

            if ("EXISTENTES".Equals(args[0]))
            {
                eventLogService.WriteEntry("Download existing accounts update.", EventLogEntryType.Information, 2,0);
                AdminPaqMiner miner = new AdminPaqMiner();
                miner.UpdateAccounts(eventLogService);
                eventLogService.WriteEntry("Existing accounts update downloaded.", EventLogEntryType.Information, 3, 0);
            }
            else if ("HOY".Equals(args[0]))
            {
                eventLogService.WriteEntry("Download today accounts.", EventLogEntryType.Information, 4, 0);
                AdminPaqMiner miner = new AdminPaqMiner();
                miner.DownloadTodayAccounts(eventLogService);
                eventLogService.WriteEntry("Today accounts downloaded.", EventLogEntryType.Information, 5, 0);
            }
            else if ("TODO".Equals(args[0]))
            {
                eventLogService.WriteEntry("Download All accounts.", EventLogEntryType.Information, 9, 0);
                AdminPaqMiner miner = new AdminPaqMiner();
                miner.DownloadAllAccounts(eventLogService);
                eventLogService.WriteEntry("All accounts downloaded.", EventLogEntryType.Information, 10, 0);
            }
            else if ("MONITORES".Equals(args[0]))
            {
                eventLogService.WriteEntry("Download monitors.", EventLogEntryType.Information, 9, 0);
                AdminPaqMiner miner = new AdminPaqMiner();
                miner.MineMonitors(eventLogService);
                eventLogService.WriteEntry("monitors downloaded.", EventLogEntryType.Information, 10, 0);
            }
            else
            { 
                DateTime requested = DateTime.Today;
                bool parsed = DateTime.TryParseExact(args[0], "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out requested);
                if (!parsed)
                {
                    eventLogService.WriteEntry("Invalid date format. Received: [" + args[0] + "] must be dd-MM-yyyy", EventLogEntryType.Error, 6, 0);
                    return;
                }

                eventLogService.WriteEntry("Download accounts for date [" + requested.ToShortDateString() + "].", EventLogEntryType.Information, 7, 0);
                AdminPaqMiner miner = new AdminPaqMiner();
                miner.DownloadTodayAccounts(eventLogService);
                eventLogService.WriteEntry("Accounts for specific date downloaded.", EventLogEntryType.Information, 8, 0);


            }


            eventLogService.WriteEntry("CONSOLE EXECUTION END.");
        }
    }
}
