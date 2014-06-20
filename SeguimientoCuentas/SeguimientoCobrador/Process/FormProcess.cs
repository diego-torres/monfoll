﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoCobrador.Collectable.PostgresImpl;
using SeguimientoCobrador.Collectable;
using SeguimientoCobrador.Properties;

namespace SeguimientoCobrador.Process
{
    public partial class FormProcess : Form
    {
        private AdminPaqImp api;

        private SeguimientoCobrador.Collectable.PostgresImpl.Account dbAccount = new SeguimientoCobrador.Collectable.PostgresImpl.Account();

        private DataTable dtMaster, dtAttended;

        public AdminPaqImp API { get { return api; } set { api = value; } }

        public FormProcess()
        {
            InitializeComponent();
        }

        public void SearchData(String client, String serie, String folio)
        {
            if (string.Empty.Equals(client.Trim()) && string.Empty.Equals(serie.Trim()) && string.Empty.Equals(folio)) return;
            string filterToApply = string.Empty;

            if(!string.Empty.Equals(client.Trim()))
                filterToApply = FilterString("cd_cliente", client);

            if (string.Empty.Equals(filterToApply))
            {
                if (!string.Empty.Equals(serie.Trim()))
                    filterToApply = FilterString("serie_doco", serie);
            }
            else 
            {
                if (!string.Empty.Equals(serie.Trim()))
                    filterToApply += " AND " + FilterString("serie_doco", serie);
            }

            if (string.Empty.Equals(filterToApply))
            {
                if (!string.Empty.Equals(folio.Trim()))
                    filterToApply = FilterString("folio_doco", folio);
            }
            else
            {
                if (!string.Empty.Equals(folio.Trim()))
                    filterToApply += " AND " + FilterString("folio_doco", folio);
            }

            toolStripStatusFilterMaster.Text = "FILTRO: " + filterToApply;
            dtMaster.DefaultView.RowFilter = filterToApply;
            dataGridViewMaster.DataSource = dtMaster;
            FormatAccountsGridView(dataGridViewMaster);

            toolStripStatusFilterAttended.Text = "FILTRO: " + filterToApply;
            dtAttended.DefaultView.RowFilter = filterToApply;
            dataGridViewAttended.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttended);

            if (dtMaster.Rows.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 1;
                return;
            }

            if (dtAttended.Rows.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 2;
                return;
            }
            
            MessageBox.Show("No fue posible encontrar la cuenta con los siguientes datos: \n" +
                "Cliente: " + client + "\n" +
                "Serie: " + serie + "\n" +
                "Folio: " + folio,
                "Documento no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

        
        private void FormProcess_Load(object sender, EventArgs e)
        {
            dtMaster = dbAccount.MasterTable();
            dataGridViewMaster.DataSource = dtMaster;
            FormatAccountsGridView(dataGridViewMaster);

            //attended
            dtAttended = dbAccount.Attended();
            dataGridViewAttended.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttended);
        }

        private Payment PaymentFromDataRow(DataRow paymentRow)
        {
            Payment payment = new Payment();
            payment.PaymentId = int.Parse(paymentRow["id_abono"].ToString());
            payment.PaymentType = paymentRow["tipo_pago"].ToString();
            payment.Amount = double.Parse(paymentRow["importe_pago"].ToString());
            payment.Folio = int.Parse(paymentRow["folio"].ToString());
            payment.Concept = paymentRow["concepto"].ToString();
            payment.DepositDate = DateTime.Parse(paymentRow["fecha_deposito"].ToString());
            payment.Account = paymentRow["cuenta"].ToString();

            return payment;
        }

        private int ConfiguredCompanyId()
        {
            Settings set = Settings.Default;
            return set.empresa;
        }

        private Collectable.Account AccountFromGrid(DataGridView dgv)
        {
            Collectable.Account account = new Collectable.Account();
            account.DocId = int.Parse(dgv.CurrentRow.Cells["id_doco"].Value.ToString());
            account.ApId = int.Parse(dgv.CurrentRow.Cells["ap_id"].Value.ToString());
            account.DocDate = DateTime.Parse(dgv.CurrentRow.Cells["f_documento"].Value.ToString());
            account.DueDate = DateTime.Parse(dgv.CurrentRow.Cells["f_vencimiento"].Value.ToString());
            account.CollectDate = DateTime.Parse(dgv.CurrentRow.Cells["f_cobro"].Value.ToString());
            account.Serie = dgv.CurrentRow.Cells["serie_doco"].Value.ToString();
            account.Folio = int.Parse(dgv.CurrentRow.Cells["folio_doco"].Value.ToString());
            account.DocType = dgv.CurrentRow.Cells["tipo_documento"].Value.ToString();
            account.CollectType = dgv.CurrentRow.Cells["tipo_cobro"].Value.ToString();
            account.Amount = double.Parse(dgv.CurrentRow.Cells["facturado"].Value.ToString());
            account.Balance = double.Parse(dgv.CurrentRow.Cells["saldo"].Value.ToString());
            account.Currency = dgv.CurrentRow.Cells["moneda"].Value.ToString();
            account.Note = dgv.CurrentRow.Cells["observaciones"].Value.ToString();

            Collectable.Company company = new Collectable.Company();
            company.Id = int.Parse(dgv.CurrentRow.Cells["id_cliente"].Value.ToString());
            company.Code = dgv.CurrentRow.Cells["cd_cliente"].Value.ToString();
            company.Name = dgv.CurrentRow.Cells["nombre_cliente"].Value.ToString();
            company.AgentCode = dgv.CurrentRow.Cells["ruta"].Value.ToString();
            company.PaymentDay = dgv.CurrentRow.Cells["dia_pago"].Value.ToString();
            company.EnterpriseId = ConfiguredCompanyId();
            account.Company = company;

            return account;
        }

        private void FormatAccountsGridView(DataGridView dgv)
        {
            dgv.Columns["id_cliente"].Visible = false;
            dgv.Columns["id_doco"].Visible = false;
            dgv.Columns["ap_id"].Visible = false;

            FixColumn(dgv.Columns["f_documento"], 0, "Fecha Documento", 80);
            FixColumn(dgv.Columns["f_vencimiento"], 1, "Fecha Vencimiento", 80);
            FixColumn(dgv.Columns["dias_vencido"], 2, "Dias Vencimiento", 80);
            FixColumn(dgv.Columns["f_cobro"], 3, "Fecha Cobro", 80);

            FixColumn(dgv.Columns["ruta"], 4, "Ruta", 80);
            FixColumn(dgv.Columns["serie_doco"], 5, "Serie", 40);
            FixColumn(dgv.Columns["folio_doco"], 6, "Doc #", 80);

            FixColumn(dgv.Columns["cd_cliente"], 7, "# Cliente", 80);
            FixColumn(dgv.Columns["nombre_cliente"], 8, "Nombre del Cliente", 180);

            FixColumn(dgv.Columns["tipo_cobro"], 9, "Tipo Cobro", 150);
            FixColumn(dgv.Columns["facturado"], 10, "Total Facturado", 80);
            FixColumn(dgv.Columns["saldo"], 11, "Saldo", 80);
            FixColumn(dgv.Columns["moneda"], 12, "Moneda", 80);
            FixColumn(dgv.Columns["observaciones"], 13, "Observaciones", 150);

            FixColumn(dgv.Columns["tipo_documento"], 14, "Tipo Doc", 150);
            FixColumn(dgv.Columns["dia_pago"], 15, "Dia de Pago", 120);
            FixColumn(dgv.Columns["lista_negra"], 16, "Cuenta Especial", 40);

            dgv.Columns["f_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dgv.Columns["tipo_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dgv.Columns["observaciones"].DefaultCellStyle.BackColor = Color.Beige;

            dgv.Columns["facturado"].DefaultCellStyle.Format = "c";
            dgv.Columns["saldo"].DefaultCellStyle.Format = "c";
        }

        private void FixColumn(DataGridViewColumn column, int displayedIndex, string HeaderText, int width)
        {
            if (column == null) return;
            column.DisplayIndex = displayedIndex;
            column.HeaderText = HeaderText;
            column.Width = width;
        }

        private string NegativeFilterString(string columnName, string columnValue)
        {
            if (IsNumericColumn(columnName))
                return string.Format("{0}<>{1}", columnName, columnValue);

            return string.Format("{0}<>'{1}'", columnName, columnValue);
        }

        private string FilterString(string columnName, string columnValue)
        {
            if (IsNumericColumn(columnName))
                return string.Format("{0}={1}", columnName, columnValue);

            return string.Format("{0}='{1}'", columnName, columnValue);
        }

        private bool IsNumericColumn(string columnName)
        {
            string[] numericColumns = { "id_doco", "id_cliente", "folio_doco", "facturado", "saldo", "dias_vencido" };
            return numericColumns.Contains(columnName);
        }

        private List<int> SelectedIds(DataGridView dgv)
        {
            List<int> selectedIds = new List<int>();

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                DataGridViewRow selectedRow = dgv.Rows[cell.RowIndex];
                int selectedId = int.Parse(selectedRow.Cells["id_doco"].Value.ToString());
                if (!selectedIds.Contains(selectedId))
                    selectedIds.Add(selectedId);
            }
            return selectedIds;
        }

        private List<Collectable.Account> SelectedAdminId(DataGridView dgv)
        {
            List<Collectable.Account> selectedAdminId = new List<Collectable.Account>();

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                DataGridViewRow selectedRow = dgv.Rows[cell.RowIndex];
                int selectedId = int.Parse(selectedRow.Cells["ap_id"].Value.ToString());
                int selectedPgDocId = int.Parse(selectedRow.Cells["id_doco"].Value.ToString());

                Collectable.Account pgDoco = new Collectable.Account();
                pgDoco.ApId = selectedId;
                pgDoco.DocId = selectedPgDocId;

                if (!selectedAdminId.Contains(pgDoco))
                    selectedAdminId.Add(pgDoco);
            }
            return selectedAdminId;
        }

        #region CONTEXT_MENU

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!(sender is DataGridView)) return;

                DataGridView dgvSender = (DataGridView)sender;
                var hti = dgvSender.HitTest(e.X, e.Y);
                dgvSender.ClearSelection();
                dgvSender.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected = true;
                dgvSender.CurrentCell = dgvSender.Rows[hti.RowIndex].Cells[hti.ColumnIndex];
            }
        }

        private void applyFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataGridView dgvSender = null;
            DataTable dtSender = null;
            ToolStripStatusLabel status = null;

            switch (tabControlProcess.SelectedIndex)
            {
                case 0:
                    dgvSender = dataGridViewMaster;
                    dtSender = dtMaster;
                    status = toolStripStatusFilterMaster;
                    break;
                case 1:
                    dgvSender = dataGridViewAttended;
                    dtSender = dtAttended;
                    status = toolStripStatusFilterAttended;
                    break;
                default:
                    return;
            }

            string filterToApply = dtSender.DefaultView.RowFilter;
            string columnName = dgvSender.Columns[dgvSender.CurrentCell.ColumnIndex].Name;
            string filterValue = dgvSender.CurrentCell.Value.ToString();


            if (string.Empty.Equals(filterToApply))
                filterToApply = FilterString(columnName, filterValue);
            else
                filterToApply += " AND " + FilterString(columnName, filterValue);
            
            status.Text = "FILTRO: " + filterToApply;

            dtSender.DefaultView.RowFilter = filterToApply;
            dgvSender.DataSource = dtSender;
            FormatAccountsGridView(dgvSender);
        }

        private void avoidValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgvSender = null;
            DataTable dtSender = null;
            ToolStripStatusLabel status = null;

            switch (tabControlProcess.SelectedIndex)
            {
                case 0:
                    dgvSender = dataGridViewMaster;
                    dtSender = dtMaster;
                    status = toolStripStatusFilterMaster;
                    break;
                case 1:
                    dgvSender = dataGridViewAttended;
                    dtSender = dtAttended;
                    status = toolStripStatusFilterAttended;
                    break;
                default:
                    return;
            }

            string filterToApply = dtSender.DefaultView.RowFilter;
            string columnName = dgvSender.Columns[dgvSender.CurrentCell.ColumnIndex].Name;
            string filterValue = dgvSender.CurrentCell.Value.ToString();

            if (string.Empty.Equals(filterToApply))
                filterToApply = NegativeFilterString(columnName, filterValue);
            else
                filterToApply += " AND " + NegativeFilterString(columnName, filterValue);

            status.Text = "FILTRO: " + filterToApply;

            dtSender.DefaultView.RowFilter = filterToApply;
            dgvSender.DataSource = dtSender;
            FormatAccountsGridView(dgvSender);

        }

        #endregion

        #region PRINTERS

        private List<Reports.Account> ReportAccountsFromGrid(DataGridView dgv)
        {
            List<Reports.Account> rAccounts = new List<Reports.Account>();

            foreach (DataGridViewRow documentRow in dgv.Rows)
            {
                Reports.Account ra = new Reports.Account();
                ra.Name = documentRow.Cells["nombre_cliente"].Value.ToString();
                ra.AgentCode = documentRow.Cells["ruta"].Value.ToString();

                ra.Currency = documentRow.Cells["moneda"].Value.ToString();
                if (ra.Currency.ToUpper().Contains("PESO"))
                {
                    ra.Balance = double.Parse(documentRow.Cells["saldo"].Value.ToString());
                    ra.Amount = double.Parse(documentRow.Cells["facturado"].Value.ToString());
                    ra.Dolares = 0;
                    ra.TotalDolares = 0;
                }
                else
                {
                    ra.Balance = 0;
                    ra.Amount = 0;
                    ra.Dolares = double.Parse(documentRow.Cells["saldo"].Value.ToString());
                    ra.TotalDolares = double.Parse(documentRow.Cells["facturado"].Value.ToString());
                }

                ra.CollectDate = DateTime.Parse(documentRow.Cells["f_cobro"].Value.ToString());
                ra.CollectType = documentRow.Cells["tipo_cobro"].Value.ToString();
                ra.CompanyCode = documentRow.Cells["cd_cliente"].Value.ToString();
                ra.DocDate = DateTime.Parse(documentRow.Cells["f_documento"].Value.ToString());
                ra.DocId = int.Parse(documentRow.Cells["id_doco"].Value.ToString());
                ra.DocType = documentRow.Cells["tipo_documento"].Value.ToString();
                ra.DueDate = DateTime.Parse(documentRow.Cells["f_vencimiento"].Value.ToString());
                ra.Folio = int.Parse(documentRow.Cells["folio_doco"].Value.ToString());
                ra.Note = documentRow.Cells["observaciones"].Value.ToString();
                ra.PaymentDay = documentRow.Cells["dia_pago"].Value.ToString();
                ra.Serie = documentRow.Cells["serie_doco"].Value.ToString();

                rAccounts.Add(ra);
            }
            return rAccounts;
        }

        private void ShowReportFromGrid(DataGridView dgv)
        {
            Reports.ReportViewer rv = new Reports.ReportViewer();
            rv.ReportAccounts = ReportAccountsFromGrid(dgv);
            rv.Show();
        }

        private void toolStripButtonPrintMaster_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewMaster);
        }

        private void toolStripButtonPrintAttended_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewAttended);
        }
        #endregion

        #region GRID_REFRESHERS
        private void RefreshMaster()
        {
            string prevFilter = dtMaster.DefaultView.RowFilter;
            dtMaster = dbAccount.MasterTable();
            dtMaster.DefaultView.RowFilter = prevFilter;
            dataGridViewMaster.DataSource = dtMaster;

            FormatAccountsGridView(dataGridViewMaster);
        }

        private void RefreshAttended()
        {
            string prevFilter = dtAttended.DefaultView.RowFilter;
            dtAttended = dbAccount.Attended();
            dtAttended.DefaultView.RowFilter = prevFilter;
            dataGridViewAttended.DataSource = dtAttended;

            FormatAccountsGridView(dataGridViewAttended);
        }
        #endregion

        #region DATE_SETTERS
        private void toolStripButtonSetDateMaster_Click(object sender, EventArgs e)
        {
            DialogCollectDate dlgCollectDate = new DialogCollectDate();
            dlgCollectDate.ShowDialog();
            if (dlgCollectDate.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewMaster);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetCollectDate(account.DocId, dlgCollectDate.dateTimePickerCollectDate.Value);
                api.SetCollectDate(account.ApId, dlgCollectDate.dateTimePickerCollectDate.Value);
            }

            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshMaster();
            RefreshAttended();
        }

        private void toolStripButtonSetCollectDateAttended_Click(object sender, EventArgs e)
        {
            DialogCollectDate dlgCollectDate = new DialogCollectDate();
            dlgCollectDate.ShowDialog();
            if (dlgCollectDate.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAttended);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetCollectDate(account.DocId, dlgCollectDate.dateTimePickerCollectDate.Value);
                api.SetCollectDate(account.ApId, dlgCollectDate.dateTimePickerCollectDate.Value);
            }

            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
        }
        #endregion

        #region NOTE_SETTERS
        
        private void toolStripButtonSetObservationsMaster_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewMaster);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshMaster();
            RefreshAttended();
        }

        private void toolStripButtonSetObservationsAttended_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAttended);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
        }
        #endregion

        #region FILTER_REMOVERS
        private void toolStripButtonRemoveFilterMaster_Click(object sender, EventArgs e)
        {
            dtMaster = dbAccount.MasterTable();
            dtMaster.DefaultView.RowFilter = string.Empty;
            dataGridViewMaster.DataSource = dtMaster;
            toolStripStatusFilterMaster.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilterAttended_Click(object sender, EventArgs e)
        {
            dtAttended = dbAccount.Attended();
            dtAttended.DefaultView.RowFilter = string.Empty;
            dataGridViewAttended.DataSource = dtAttended;
            toolStripStatusFilterAttended.Text = "FILTRO:";
        }
        #endregion

        # region REFRESH_BUTTONS
        private void toolStripButtonRefreshMaster_Click(object sender, EventArgs e)
        {
            RefreshMaster();
        }

        private void toolStripButtonRefreshAttended_Click(object sender, EventArgs e)
        {
            RefreshAttended();
        }
        #endregion

        #region FOLLOW_UP
        private void dataGridViewMaster_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewMaster.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewMaster);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewAttended_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewAttended.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewAttended);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }
        #endregion

        #region ADMIN_PAQ_DOWNLOADERS
        private void UpdateFromAdminPaq(DataGridView dgv)
        {
            Settings set = Settings.Default;
            SeguimientoCobrador.Collectable.PostgresImpl.Enterprise dbEnterprise = new SeguimientoCobrador.Collectable.PostgresImpl.Enterprise();

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                DataGridViewRow selectedRow = dgv.Rows[cell.RowIndex];

                int selectedId = int.Parse(selectedRow.Cells["ap_id"].Value.ToString());
                int selectedPgDocId = int.Parse(selectedRow.Cells["id_doco"].Value.ToString());

                Collectable.Account pgDoco = new Collectable.Account();
                pgDoco.ApId = selectedId;
                pgDoco.DocId = selectedPgDocId;
                
                bool cancelled = false;

                api.DownloadCollectable(ref pgDoco, dbEnterprise.ConceptosPago(set.empresa), out cancelled);
                //Collectable.PostgresImpl.Account dbAccount = new Collectable.PostgresImpl.Account();
                dbAccount.SaveAccount(pgDoco);

                foreach (Collectable.Payment payment in pgDoco.Payments)
                {
                    payment.DocId = pgDoco.DocId;
                    dbAccount.SavePayment(payment);
                }

                if (cancelled) 
                    dbAccount.CancelAccount(pgDoco.DocId);
            }
        }

        private void toolStripButtonAdminPaqDownloadMaster_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewMaster);
            RefreshMaster();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButtonAdminPaqDownloadAttended_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewAttended);
            RefreshAttended();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region FILTER_SAVERS
        private void SaveFilter(string filter)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            saveFileDialog1.Title = "Grabar Filtro";
            //saveFileDialog1.CheckFileExists = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "ftr";
            saveFileDialog1.Filter = "Filtros (*.ftr)|*.ftr";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!saveFileDialog1.FileName.Equals(string.Empty))
                    System.IO.File.WriteAllText(saveFileDialog1.FileName, filter);
            }
        }

        private void toolStripButtonSaveFilterMaster_Click(object sender, EventArgs e)
        {
            if(!dtMaster.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtMaster.DefaultView.RowFilter);
        }

        private void toolStripButtonSaveFilterAttended_Click(object sender, EventArgs e)
        {
            if (!dtAttended.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtAttended.DefaultView.RowFilter);
        }

        #endregion

        private string ReadFilter()
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Filtros (*.ftr)|*.ftr";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            ;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open the selected file to read.
                string filter = System.IO.File.ReadAllText(openFileDialog1.FileName);
                return filter;
            }
            return string.Empty;
        }

        private void toolStripButtonOpenFilterMaster_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtMaster.DefaultView.RowFilter = filter;
                dataGridViewMaster.DataSource = dtMaster;

                FormatAccountsGridView(dataGridViewMaster);
                toolStripStatusFilterMaster.Text = "FILTRO: " + filter;
            }
        }

        private void toolStripButtonOpenFilterAttended_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtAttended.DefaultView.RowFilter = filter;
                dataGridViewAttended.DataSource = dtAttended;

                FormatAccountsGridView(dataGridViewAttended);
                toolStripStatusFilterAttended.Text = "FILTRO: " + filter;
            }
        }
    }
}
