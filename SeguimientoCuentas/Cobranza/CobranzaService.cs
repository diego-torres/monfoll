using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace Cobranza
{
    public partial class CobranzaService : ServiceBase
    {

        private Timer tmrDelay;

        public CobranzaService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("CobranzaService"))
            {
                System.Diagnostics.EventLog.CreateEventSource("CobranzaService", "CobranzaLog");
            }

            eventLogService.Source = "CobranzaService";
            eventLogService.Log = "CobranzaLog";
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
            if (tmrDelay.Interval == 30000)
            {
                tmrDelay.Interval = 400000;
            }

            eventLogService.WriteEntry("START TODAY DOWNLOAD PROCESS.",
                EventLogEntryType.Information, 1, 1);

            System.Threading.Thread.Sleep(180000);

            eventLogService.WriteEntry("START UPDATE PROCESS.",
                EventLogEntryType.Information, 2, 1);
        }
    }
}
