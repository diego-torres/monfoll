using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Cobranza.Process;

namespace Cobranza
{
    public partial class CobranzaService : ServiceBase
    {

        private Timer tmrDelay;
        CommonAdminPaq.AdminPaqLib apl;
        Main process = new Main();

        public CobranzaService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("CobranzaService"))
            {
                System.Diagnostics.EventLog.CreateEventSource("CobranzaService", "CobranzaLog");
            }

            eventLogService.Source = "CobranzaService";
            eventLogService.Log = "CobranzaLog";
            apl = new CommonAdminPaq.AdminPaqLib();
            apl.SetDllFolder();
        }

        protected override void OnStart(string[] args)
        {
            eventLogService.WriteEntry("Cobranza Service started.", EventLogEntryType.Information, 0, 0);
            // Timer start
            tmrDelay = new Timer(30000);
            tmrDelay.Elapsed += new ElapsedEventHandler(timerDelay_Tick);
            tmrDelay.Enabled = true;
            tmrDelay.Start();
        }

        protected override void OnStop()
        {
            tmrDelay.Stop();
            eventLogService.WriteEntry("Cobranza Service stoped.", EventLogEntryType.Information, 1, 0);
        }

        private void timerDelay_Tick(object sender, EventArgs e)
        {
            try
            {
                tmrDelay.Stop();
                tmrDelay.Interval = 300000;
                eventLogService.WriteEntry("PERIODICAL ETL Process Execution BEGIN.", EventLogEntryType.Information, 2, 1);
                
                process.Execute(eventLogService);
                
                eventLogService.WriteEntry("PERIODICAL ETL Process Execution END.", EventLogEntryType.Information, 3, 1);
                tmrDelay.Start();
            }
            catch (Exception ex)
            {
                eventLogService.WriteEntry("Exception while running process. " + ex.Message + "::" + ex.StackTrace, EventLogEntryType.Error, 4, 1);
            }
        }
    }
}
