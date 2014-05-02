using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace SeguimientoGerente.Reports
{
    public partial class ReportViewer : Form
    {

        private List<Account> rAccounts = new List<Account>();
        public List<Account> ReportAccounts { get { return rAccounts; } set { rAccounts = value; } }

        public ReportViewer()
        {
            InitializeComponent();
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            DataSet dsAccountsReport = new DataSet();
            ReportDataSource rds = new ReportDataSource();
            
            rds.Name = "DataSet1";
            rds.Value = rAccounts;

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.ReportEmbeddedResource = "SeguimientoSuper.ReportCustomerAccounts.rdlc";
            reportViewer1.LocalReport.ReportPath = @"Reports/ReportCustomerAccounts.rdlc";
            reportViewer1.LocalReport.Refresh();

            this.reportViewer1.RefreshReport();
        }
    }
}
