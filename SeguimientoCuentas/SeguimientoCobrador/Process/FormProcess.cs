using System;
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

        private DataTable dtAssigned, dtAttended;

        public AdminPaqImp API { get { return api; } set { api = value; } }

        public FormProcess()
        {
            InitializeComponent();
        }

        #region EVENTS

        private void FormProcess_Load(object sender, EventArgs e)
        {
            dtAssigned = dbAccount.Assigned();
            dataGridViewAssignedAccounts.DataSource = dtAssigned;
            FormatAccountsGridView(dataGridViewAssignedAccounts);

            dtAttended = dbAccount.Attended();
            dataGridViewAttendedAccounts.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttendedAccounts);
        }


        # region PRINT_BUTTONS

        private void toolStripButtonPrintAttended_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewAttendedAccounts);
        }

        private void toolStripButtonPrintAssigned_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewAssignedAccounts);
        }

        # endregion

        # region FILTER_REMOVERS

        private void toolStripButtonRemmoveFilterAttended_Click(object sender, EventArgs e)
        {
            dtAttended = dbAccount.Attended();
            dtAttended.DefaultView.RowFilter = string.Empty;
            dataGridViewAttendedAccounts.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttendedAccounts);
            toolStripStatusLabelAttended.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilter_Click(object sender, EventArgs e)
        {
            dtAssigned = dbAccount.Assigned();
            dtAssigned.DefaultView.RowFilter = string.Empty;
            dataGridViewAssignedAccounts.DataSource = dtAssigned;
            FormatAccountsGridView(dataGridViewAssignedAccounts);
            assignedToolStripStatusFilter.Text = "FILTRO:";
        }

        # endregion

        # region REFRESH_BUTTONS


        private void toolStripButtonUpdateAttended_Click(object sender, EventArgs e)
        {
            RefreshAttended();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshAssigned();
        }

        #endregion

        #region ACTIVATE_ACCOUNT_GRID
        
        private void dataGridViewAttendedAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAttendedAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewAttendedAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewAttendedFollowUp.DataSource = dbAccount.FollowUp(docId);
            FormatFollowUpGridView(dataGridViewAttendedFollowUp);
        }
        
        private void dataGridViewAssignedAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAssignedAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewAssignedAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewAssignedFollowUp.DataSource = dbAccount.FollowUp(docId);
            FormatFollowUpGridView(dataGridViewAssignedFollowUp);
        }


        #endregion

        #region FOLLOW_UP_ACCOUNT

        private void dataGridViewAttendedAccounts_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewAttendedAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewAttendedAccounts);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewAssignedAccounts_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewAssignedAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewAssignedAccounts);
            
            DataTable dtPayments =  dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }


        #endregion

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
                    dgvSender = dataGridViewAssignedAccounts;
                    dtSender = dtAssigned;
                    status = assignedToolStripStatusFilter;
                    break;
                case 1:
                    dgvSender = dataGridViewAttendedAccounts;
                    dtSender = dtAttended;
                    status = toolStripStatusLabelAttended;
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
                    dgvSender = dataGridViewAssignedAccounts;
                    dtSender = dtAssigned;
                    status = assignedToolStripStatusFilter;
                    break;
                case 1:
                    dgvSender = dataGridViewAttendedAccounts;
                    dtSender = dtAttended;
                    status = toolStripStatusLabelAttended;
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

        #region DATE_SETTERS

        private void toolStripButtonAttendedObservations_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAttendedAccounts);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
        }

        private void toolStripButtonObservacionesAsignados_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAssignedAccounts);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAssigned();
            RefreshAttended();
        }

        private void toolStripButtonSetCollectDateAttended_Click(object sender, EventArgs e)
        {
            DialogCollectDate collectDate = new DialogCollectDate();
            collectDate.ShowDialog();

            if (collectDate.DialogResult == DialogResult.Cancel) return;
            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAttendedAccounts);

            foreach (Collectable.Account account in selectedIds)
            {
                if (collectDate.dateTimePickerCollectDate.Checked)
                {
                    dbAccount.SetCollectDate(account.DocId, collectDate.dateTimePickerCollectDate.Value);
                    api.SetCollectDate(account.ApId, collectDate.dateTimePickerCollectDate.Value);
                }
                else
                {
                    dbAccount.SetCollectDate(account.DocId, new DateTime(0));
                    api.SetCollectDate(account.ApId, collectDate.dateTimePickerCollectDate.Value);
                }
            }
            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAssigned();
            RefreshAttended();
        }

        private void AssignedToolStripButtonSetCollectDate_Click(object sender, EventArgs e)
        {
            DialogCollectDate collectDate = new DialogCollectDate();
            collectDate.ShowDialog();

            if (collectDate.DialogResult == DialogResult.Cancel) return;
            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAssignedAccounts);

            foreach (Collectable.Account account in selectedIds)
            {

                if (collectDate.dateTimePickerCollectDate.Checked)
                {
                    dbAccount.SetCollectDate(account.DocId, collectDate.dateTimePickerCollectDate.Value);
                    api.SetCollectDate(account.ApId, collectDate.dateTimePickerCollectDate.Value);
                }
                else
                {
                    dbAccount.SetCollectDate(account.DocId, new DateTime(0));
                    api.SetCollectDate(account.ApId, collectDate.dateTimePickerCollectDate.Value);
                }
            }
            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAssigned();
            RefreshAttended();
        }

        #endregion


        #endregion

        # region GRID_REFRESHERS

        private void RefreshAttended()
        {
            string prevFilter = dtAttended.DefaultView.RowFilter;
            dtAttended = dbAccount.Attended();
            dtAttended.DefaultView.RowFilter = prevFilter;
            dataGridViewAttendedAccounts.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttendedAccounts);
        }

        private void RefreshAssigned()
        {
            string prevFilter = dtAssigned.DefaultView.RowFilter;
            dtAssigned = dbAccount.Assigned();
            dtAssigned.DefaultView.RowFilter = prevFilter;
            dataGridViewAssignedAccounts.DataSource = dtAssigned;
            FormatAccountsGridView(dataGridViewAssignedAccounts);
            if (dtAssigned.Rows.Count == 0)
            {
                dataGridViewAssignedFollowUp.DataSource = null;
                dataGridViewAssignedFollowUp.Refresh();
            }
                
        }

        #endregion

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

        private Payment PaymentFromGridRow(DataGridViewRow paymentRow)
        {
            Payment payment = new Payment();
            payment.PaymentId = int.Parse(paymentRow.Cells["id_abono"].Value.ToString());
            payment.PaymentType = paymentRow.Cells["tipo_pago"].Value.ToString();
            payment.Amount = double.Parse(paymentRow.Cells["importe_pago"].Value.ToString());
            payment.Folio = int.Parse(paymentRow.Cells["folio"].Value.ToString());
            payment.Concept = paymentRow.Cells["concepto"].Value.ToString();
            payment.DepositDate = DateTime.Parse(paymentRow.Cells["fecha_deposito"].Value.ToString());
            payment.Account = paymentRow.Cells["cuenta"].Value.ToString();

            return payment;
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

        private int ConfiguredCompanyId()
        {
            Settings set = Settings.Default;
            return set.empresa;
        }

        private void FormatFollowUpGridView(DataGridView dgv)
        {
            dgv.Columns["id_seguimiento"].Visible = false;
            dgv.Columns["id_movimiento"].Visible = false;
            dgv.Columns["system_based"].Visible = false;

            FixColumn(dgv.Columns["movimiento"], 0, "Movimiento", 120);
            FixColumn(dgv.Columns["seguimiento"], 1, "Seguimiento", 350);
            FixColumn(dgv.Columns["ts_seguimiento"], 2, "Fecha", 150);

            dgv.Sort(dgv.Columns["id_seguimiento"], ListSortDirection.Descending);
        }

        private void FormatPaymentsGridView(DataGridView dgv)
        {
            dgv.Columns["id_abono"].Visible = false;

            FixColumn(dgv.Columns["concepto"], 0, "Concepto", 130);
            FixColumn(dgv.Columns["importe_pago"], 1, "Importe", 80);
            FixColumn(dgv.Columns["fecha_deposito"], 2, "Fecha", 80);
            FixColumn(dgv.Columns["folio"], 3, "Folio", 60);
            FixColumn(dgv.Columns["tipo_pago"], 4, "Tipo", 120);
            FixColumn(dgv.Columns["cuenta"], 5, "Cuenta", 100);

            dgv.Columns["importe_pago"].DefaultCellStyle.Format = "c";
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
       
        private void ShowReportFromGrid(DataGridView dgv)
        {
            Reports.ReportViewer rv = new Reports.ReportViewer();
            rv.ReportAccounts = ReportAccountsFromGrid(dgv);
            rv.Show();
        }

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

    }
}
