using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;
using SeguimientoSuper.Catalogs;
using SeguimientoSuper.Collectable;
using SeguimientoSuper.Properties;

namespace SeguimientoSuper.Process
{
    public partial class FormProcess : Form
    {
        private AdminPaqImp api;

        private SeguimientoSuper.Collectable.PostgresImpl.Account dbAccount = new SeguimientoSuper.Collectable.PostgresImpl.Account();

        private DataTable dtUnassigned, dtAssigned, dtEscalated, dtClosed, dtCancelled;

        public AdminPaqImp API { get { return api; } set { api = value; } }

        public FormProcess()
        {
            InitializeComponent();
        }

        #region EVENTS

        private void FormProcess_Load(object sender, EventArgs e)
        {
            dtUnassigned = dbAccount.UnAssigned();
            dataGridViewNotAssignedAccounts.DataSource = dtUnassigned;
            FormatAccountsGridView(dataGridViewNotAssignedAccounts);

            dtAssigned = dbAccount.Assigned();
            dataGridViewAssignedAccounts.DataSource = dtAssigned;
            FormatAccountsGridView(dataGridViewAssignedAccounts);

            dtEscalated = dbAccount.Escalated();
            dataGridViewEscalatedAccounts.DataSource = dtEscalated;
            FormatAccountsGridView(dataGridViewEscalatedAccounts);

            dtClosed = dbAccount.Closed();
            dataGridViewClosedAccounts.DataSource = dtClosed;
            FormatAccountsGridView(dataGridViewClosedAccounts);

            dtCancelled = dbAccount.Cancelled();
            dataGridViewCancelledAccounts.DataSource = dtCancelled;
            FormatAccountsGridView(dataGridViewCancelledAccounts);
        }

        # region ESCALATORS
        private void toolStripButtonEscalated_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewEscalatedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.Unescale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido desasignadas", "Desasignación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
            RefreshAssigned();
            RefreshEscalated();
        }
        private void toolStripButtonEscaleAssigned_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.Escale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.", "Cuentas escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshAssigned();
            RefreshEscalated();
        }

        private void NAtoolStripButtonEscale_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewNotAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.Escale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.", "Cuentas escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            RefreshUnassigned();
            RefreshEscalated();
        }
        #endregion

        # region PRINT_BUTTONS

        private void toolStripButtonPrintCancelled_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewCancelledAccounts);
        }

        private void toolStripButtonPrintClosed_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewClosedAccounts);
        }

        private void toolStripButtonPrintEscalated_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewEscalatedAccounts);
        }

        private void NAtoolStripButtonPrint_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewNotAssignedAccounts);
        }

        private void toolStripButtonPrintAssigned_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewAssignedAccounts);
        }

        # endregion

        # region ASSIGNERS

        private void toolStripButtonRemoveAssignment_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.Unassign(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido desasignadas", "Desasignación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
            RefreshAssigned();
        }

        private void NAtoolStripButtonAssign_Click(object sender, EventArgs e)
        {
            DialogAssign assign = new DialogAssign();
            assign.ShowDialog();

            if (assign.DialogResult == DialogResult.Cancel || string.Empty.Equals(assign.comboBoxCollector.Text)) return;

            int collectorId = int.Parse(assign.comboBoxCollector.SelectedValue.ToString());
            List<int> selectedIds = SelectedIds(dataGridViewNotAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.Assign(id, collectorId);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido asignadas a " + assign.comboBoxCollector.Text, "Asignación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
            RefreshAssigned();
        }

        #endregion

        # region CLOSERS

        private void toolStripButtonCloseAssigned_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.CloseAccount(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido cerradas.", "Cuentas cerradas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
            RefreshClosed();
        }

        private void NAtoolStripButtonClose_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewNotAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.CloseAccount(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido cerradas.", "Cuentas cerradas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
            RefreshClosed();
        }

        private void toolStripButtonReopen_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewClosedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.ReOpen(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido re-abiertas.", "Cuentas abiertas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
            RefreshAssigned();
            RefreshEscalated();
            RefreshClosed();
        }

        #endregion

        # region FILTER_REMOVERS

        private void toolStripButtonRemoveFilterCancelled_Click(object sender, EventArgs e)
        {
            dtCancelled = dbAccount.Cancelled();
            dtCancelled.DefaultView.RowFilter = string.Empty;
            dataGridViewCancelledAccounts.DataSource = dtCancelled;
            toolStripStatusLabelCancelled.Text = "FILTRO:";
        }

        private void toolStripButtonClosedRemoveFilter_Click(object sender, EventArgs e)
        {
            dtClosed = dbAccount.Closed();
            dtClosed.DefaultView.RowFilter = string.Empty;
            dataGridViewClosedAccounts.DataSource = dtClosed;
            toolStripStatusLabelClosed.Text = "FILTRO";
        }
        private void toolStripButtonEscalatedRemoveFilter_Click(object sender, EventArgs e)
        {
            dtEscalated = dbAccount.Escalated();
            dtEscalated.DefaultView.RowFilter = string.Empty;
            dataGridViewEscalatedAccounts.DataSource = dtEscalated;
            toolStripStatusLabelEscalated.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilter_Click(object sender, EventArgs e)
        {
            dtAssigned = dbAccount.Assigned();
            dtAssigned.DefaultView.RowFilter = string.Empty;
            dataGridViewAssignedAccounts.DataSource = dtAssigned;
            FormatAccountsGridView(dataGridViewAssignedAccounts);
            assignedToolStripStatusFilter.Text = "FILTRO:";
        }

        private void NAtoolStripButtonClearFilter_Click(object sender, EventArgs e)
        {
            dtUnassigned = dbAccount.UnAssigned();
            dtUnassigned.DefaultView.RowFilter = string.Empty;
            dataGridViewNotAssignedAccounts.DataSource = dtUnassigned;
            FormatAccountsGridView(dataGridViewNotAssignedAccounts);
            NAtoolStripStatusFilter.Text = "FILTRO:";
        }
        # endregion

        # region REFRESH_BUTTONS
        private void toolStripButtonRefreshCancelled_Click(object sender, EventArgs e)
        {
            RefreshCancelled();
        }

        private void toolStripButtonClosedRefresh_Click(object sender, EventArgs e)
        {
            RefreshClosed();
        }

        private void toolStripButtonEscalatedRefresh_Click(object sender, EventArgs e)
        {
            RefreshEscalated();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshAssigned();
        }

        private void NAtoolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            RefreshUnassigned();
        }
        #endregion

        #region ACTIVATE_ACCOUNT_GRID

        private void dataGridViewCancelledAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCancelledAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewCancelledAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewCancelledFollowUps.DataSource = dbAccount.FollowUp(docId);
            FormatFollowUpGridView(dataGridViewCancelledFollowUps);
        }

        private void dataGridViewClosedAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClosedAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewClosedAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewClosedPayments.DataSource = dbAccount.ReadPayments(docId);
            FormatPaymentsGridView(dataGridViewClosedPayments);
        }

        private void dataGridViewEscalatedAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewEscalatedAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewEscalatedAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewEscalatedLog.DataSource = dbAccount.FollowUp(docId);
            FormatFollowUpGridView(dataGridViewEscalatedLog);
        }

        private void dataGridViewAssignedAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAssignedAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewAssignedAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewAssignedFollowUp.DataSource = dbAccount.FollowUp(docId);
            FormatFollowUpGridView(dataGridViewAssignedFollowUp);
        }

        private void dataGridViewNotAssignedAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewNotAssignedAccounts.CurrentRow == null) return;
            int docId = int.Parse(dataGridViewNotAssignedAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            dataGridViewNotAssignedPayments.DataSource = dbAccount.ReadPayments(docId);
            FormatPaymentsGridView(dataGridViewNotAssignedPayments);
        }

        #endregion

        #region FOLLOW_UP_ACCOUNT

        private void dataGridViewCancelledAccounts_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewCancelledAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewCancelledAccounts);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewClosedAccounts_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewClosedAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewClosedAccounts);

            foreach (DataGridViewRow paymentRow in dataGridViewClosedPayments.Rows)
            {
                Payment payment = PaymentFromGridRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewEscalatedAccounts_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewEscalatedAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewEscalatedAccounts);

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

        private void dataGridViewNotAssignedAccounts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridViewNotAssignedAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewNotAssignedAccounts);
            
            foreach (DataGridViewRow paymentRow in dataGridViewNotAssignedPayments.Rows)
            {
                Payment payment = PaymentFromGridRow(paymentRow);
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
                    dgvSender = dataGridViewNotAssignedAccounts;
                    dtSender = dtUnassigned;
                    status = NAtoolStripStatusFilter;
                    break;
                case 1:
                    dgvSender = dataGridViewAssignedAccounts;
                    dtSender = dtAssigned;
                    status = assignedToolStripStatusFilter;
                    break;
                case 2:
                    dgvSender = dataGridViewEscalatedAccounts;
                    dtSender = dtEscalated;
                    status = toolStripStatusLabelEscalated;
                    break;
                case 3:
                    dgvSender = dataGridViewClosedAccounts;
                    dtSender = dtClosed;
                    status = toolStripStatusLabelClosed;
                    break;
                case 4:
                    dgvSender = dataGridViewCancelledAccounts;
                    dtSender = dtCancelled;
                    status = toolStripStatusLabelCancelled;
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
                    dgvSender = dataGridViewNotAssignedAccounts;
                    dtSender = dtUnassigned;
                    status = NAtoolStripStatusFilter;
                    break;
                case 1:
                    dgvSender = dataGridViewAssignedAccounts;
                    dtSender = dtAssigned;
                    status = assignedToolStripStatusFilter;
                    break;
                case 2:
                    dgvSender = dataGridViewEscalatedAccounts;
                    dtSender = dtEscalated;
                    status = toolStripStatusLabelEscalated;
                    break;
                case 3:
                    dgvSender = dataGridViewClosedAccounts;
                    dtSender = dtClosed;
                    status = toolStripStatusLabelClosed;
                    break;
                case 4:
                    dgvSender = dataGridViewCancelledAccounts;
                    dtSender = dtCancelled;
                    status = toolStripStatusLabelCancelled;
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

        private void EscalatedToolStripButtonSetCollectDate_Click(object sender, EventArgs e)
        {
            DialogCollectDate collectDate = new DialogCollectDate();
            collectDate.ShowDialog();

            if (collectDate.DialogResult == DialogResult.Cancel) return;
            List<int> selectedIds = SelectedIds(dataGridViewEscalatedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.SetCollectDate(id, collectDate.dateTimePickerCollectDate.Value);
                api.SetCollectDate(id, collectDate.dateTimePickerCollectDate.Value);
            }
            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshEscalated();
        }

        private void AssignedToolStripButtonSetCollectDate_Click(object sender, EventArgs e)
        {
            DialogCollectDate collectDate = new DialogCollectDate();
            collectDate.ShowDialog();

            if (collectDate.DialogResult == DialogResult.Cancel) return;
            List<int> selectedIds = SelectedIds(dataGridViewAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.SetCollectDate(id, collectDate.dateTimePickerCollectDate.Value);
                api.SetCollectDate(id, collectDate.dateTimePickerCollectDate.Value);
            }
            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAssigned();
        }

        private void toolStripButtonSetCollectDate_Click(object sender, EventArgs e)
        {
            DialogCollectDate collectDate = new DialogCollectDate();
            collectDate.ShowDialog();

            if (collectDate.DialogResult == DialogResult.Cancel) return;
            List<int> selectedIds = SelectedIds(dataGridViewNotAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.SetCollectDate(id, collectDate.dateTimePickerCollectDate.Value);
                api.SetCollectDate(id, collectDate.dateTimePickerCollectDate.Value);
            }
            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUnassigned();
        }

        #endregion


        #endregion

        # region GRID_REFRESHERS
        private void RefreshUnassigned()
        {
            string prevFilter = dtUnassigned.DefaultView.RowFilter;
            dtUnassigned = dbAccount.UnAssigned();
            dtUnassigned.DefaultView.RowFilter = prevFilter;
            dataGridViewNotAssignedAccounts.DataSource = dtUnassigned;
            FormatAccountsGridView(dataGridViewNotAssignedAccounts);
        }

        private void RefreshAssigned()
        {
            string prevFilter = dtAssigned.DefaultView.RowFilter;
            dtAssigned = dbAccount.Assigned();
            dtAssigned.DefaultView.RowFilter = prevFilter;
            dataGridViewAssignedAccounts.DataSource = dtAssigned;
            FormatAccountsGridView(dataGridViewAssignedAccounts);
        }

        private void RefreshEscalated()
        {
            string prevFilter = dtEscalated.DefaultView.RowFilter;
            dtEscalated = dbAccount.Escalated();
            dtEscalated.DefaultView.RowFilter = prevFilter;
            dataGridViewEscalatedAccounts.DataSource = dtEscalated;
            FormatAccountsGridView(dataGridViewEscalatedAccounts);
        }

        private void RefreshClosed()
        {
            string prevFilter = dtClosed.DefaultView.RowFilter;
            dtClosed = dbAccount.Closed();
            dtClosed.DefaultView.RowFilter = prevFilter;
            dataGridViewClosedAccounts.DataSource = dtClosed;
            FormatAccountsGridView(dataGridViewClosedAccounts);
        }

        private void RefreshCancelled()
        {
            string prevFilter = dtCancelled.DefaultView.RowFilter;
            dtCancelled = dbAccount.Cancelled();
            dtCancelled.DefaultView.RowFilter = prevFilter;
            dataGridViewCancelledAccounts.DataSource = dtCancelled;
            FormatAccountsGridView(dataGridViewCancelledAccounts);
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
            
            FixColumn(dgv.Columns["dias_vencido"], 0, "Dias Vencimiento", 80);
            FixColumn(dgv.Columns["f_documento"], 1, "Fecha Documento", 80);
            FixColumn(dgv.Columns["f_vencimiento"], 2, "Fecha Vencimiento", 80);
            FixColumn(dgv.Columns["f_cobro"], 3, "Fecha Cobro", 80);

            FixColumn(dgv.Columns["serie_doco"], 4, "Serie", 40);
            FixColumn(dgv.Columns["folio_doco"], 5, "Doc #", 80);
            FixColumn(dgv.Columns["tipo_documento"], 6, "Tipo Doc", 150);

            FixColumn(dgv.Columns["cd_cliente"], 7, "# Cliente", 80);
            FixColumn(dgv.Columns["nombre_cliente"], 8, "Nombre del Cliente", 180);
            FixColumn(dgv.Columns["ruta"], 9, "Ruta", 80);
            FixColumn(dgv.Columns["dia_pago"], 10, "Ruta", 120);

            FixColumn(dgv.Columns["tipo_cobro"], 11, "Tipo Cobro", 150);
            FixColumn(dgv.Columns["facturado"], 12, "Total Facturado", 80);
            FixColumn(dgv.Columns["saldo"], 13, "Saldo", 80);
            FixColumn(dgv.Columns["moneda"], 14, "Moneda", 80);
            FixColumn(dgv.Columns["observaciones"], 15, "Observaciones", 150);
            FixColumn(dgv.Columns["ap_id"], 16, "DocId", 60);


            dgv.Columns["f_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dgv.Columns["tipo_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dgv.Columns["observaciones"].DefaultCellStyle.BackColor = Color.Beige;

            dgv.Columns["facturado"].DefaultCellStyle.Format = "c";
            dgv.Columns["saldo"].DefaultCellStyle.Format = "c";
        }

        private void FixColumn(DataGridViewColumn column, int displayedIndex, string HeaderText, int width)
        {
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
                ra.Amount = double.Parse(documentRow.Cells["facturado"].Value.ToString());
                ra.Balance = double.Parse(documentRow.Cells["saldo"].Value.ToString());
                ra.CollectDate = DateTime.Parse(documentRow.Cells["f_cobro"].Value.ToString());
                ra.CollectType = documentRow.Cells["tipo_cobro"].Value.ToString();
                ra.CompanyCode = documentRow.Cells["cd_cliente"].Value.ToString();
                ra.Currency = documentRow.Cells["moneda"].Value.ToString();
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
    }
}
