using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;
using SeguimientoSuper.Collectable;
using Microsoft.Reporting.WinForms;
using SeguimientoSuper.Properties;

namespace SeguimientoSuper.Catalogs
{
    public partial class FormClientes : Form
    {
        private Customer dbCustomer = new Customer();
        private SeguimientoSuper.Collectable.PostgresImpl.Account dbAccount = new Collectable.PostgresImpl.Account();
        
        public FormClientes()
        {
            InitializeComponent();
        }

        public void RefreshAccounts()
        {
            RefreshAccountsGrid(int.Parse(dataGridViewCustomers.CurrentRow.Cells[0].Value.ToString()));
        }

        #region EVENTS
        private void FormClientes_Load(object sender, EventArgs e)
        {
            RefreshClientesGrid();
            RefreshNotesGrid();
        }

        private void dataGridViewCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCustomers.CurrentRow == null) return;
            LoadCustomer(dataGridViewCustomers.CurrentRow);
        }

        private void dataGridViewAccounts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAccounts.CurrentRow == null) return;
            RefreshPaymentsGrid(int.Parse(dataGridViewAccounts.CurrentRow.Cells[0].Value.ToString()));
            RefreshNotesGrid();
        }

        private int ConfiguredCompanyId()
        {
            Settings set = Settings.Default;
            return set.empresa;
        }

        private void dataGridViewAccounts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridViewAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            
            Collectable.Account account = new Collectable.Account();
            account.DocId = int.Parse(dataGridViewAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
            account.ApId = int.Parse(dataGridViewAccounts.CurrentRow.Cells["ap_id"].Value.ToString());
            account.DocDate = DateTime.Parse(dataGridViewAccounts.CurrentRow.Cells["f_documento"].Value.ToString());
            account.DueDate = DateTime.Parse(dataGridViewAccounts.CurrentRow.Cells["f_vencimiento"].Value.ToString());
            account.CollectDate = DateTime.Parse(dataGridViewAccounts.CurrentRow.Cells["f_cobro"].Value.ToString());
            account.Serie = dataGridViewAccounts.CurrentRow.Cells["serie_doco"].Value.ToString();
            account.Folio = int.Parse(dataGridViewAccounts.CurrentRow.Cells["folio_doco"].Value.ToString());
            account.DocType = dataGridViewAccounts.CurrentRow.Cells["tipo_documento"].Value.ToString();
            account.CollectType = dataGridViewAccounts.CurrentRow.Cells["tipo_cobro"].Value.ToString();
            account.Amount = double.Parse(dataGridViewAccounts.CurrentRow.Cells["facturado"].Value.ToString());
            account.Balance = double.Parse(dataGridViewAccounts.CurrentRow.Cells["saldo"].Value.ToString());
            account.Currency = dataGridViewAccounts.CurrentRow.Cells["moneda"].Value.ToString();
            account.Note = dataGridViewAccounts.CurrentRow.Cells["observaciones"].Value.ToString();

            Collectable.Company company = new Collectable.Company();
            company.Id = int.Parse(dataGridViewAccounts.CurrentRow.Cells["id_cliente"].Value.ToString());
            company.Code = dataGridViewAccounts.CurrentRow.Cells["cd_cliente"].Value.ToString();
            company.Name = dataGridViewAccounts.CurrentRow.Cells["nombre_cliente"].Value.ToString();
            company.AgentCode = dataGridViewAccounts.CurrentRow.Cells["ruta"].Value.ToString();
            company.PaymentDay = dataGridViewAccounts.CurrentRow.Cells["dia_pago"].Value.ToString();
            company.EnterpriseId = ConfiguredCompanyId();
            account.Company = company;

            foreach (DataGridViewRow paymentRow in dataGridViewPayments.Rows)
            {
                /*
                 "SELECT ID_ABONO, TIPO_PAGO, IMPORTE_PAGO, FOLIO, CONCEPTO, FECHA_DEPOSITO, CUENTA " +
                "FROM CTRL_ABONO " +
                "WHERE ID_DOCO = " + accountId.ToString() + ";"
                 */

                Payment payment = new Payment();
                payment.PaymentId = int.Parse(paymentRow.Cells["id_abono"].Value.ToString());
                payment.DocId = account.DocId;
                payment.PaymentType = paymentRow.Cells["tipo_pago"].Value.ToString();
                payment.Amount = double.Parse(paymentRow.Cells["importe_pago"].Value.ToString());
                payment.Folio = int.Parse(paymentRow.Cells["folio"].Value.ToString());
                payment.Concept = paymentRow.Cells["concepto"].Value.ToString();
                payment.DepositDate = DateTime.Parse(paymentRow.Cells["fecha_deposito"].Value.ToString());
                payment.Account = paymentRow.Cells["cuenta"].Value.ToString();

                account.Payments.Add(payment);
            }
            
            parent.ShowFollowUp(account);
        }

        private void toolStripButtonAssignAll_Click(object sender, EventArgs e)
        {
            AssignDocuments(false);
        }

        private void toolStripButtonAssignSelection_Click(object sender, EventArgs e)
        {
            AssignDocuments(true);
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            List<Reports.Account> rAccounts = new List<Reports.Account>();

            foreach (DataGridViewRow documentRow in dataGridViewAccounts.Rows)
            {
                Reports.Account ra = new Reports.Account();
                ra.Name = labelNombre.Text;
                ra.AgentCode = labelRuta.Text;
                ra.Amount = double.Parse(documentRow.Cells["facturado"].Value.ToString());

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
                ra.CompanyCode = labelCodigo.Text;
                
                ra.DocDate = DateTime.Parse(documentRow.Cells["f_documento"].Value.ToString());
                ra.DocId = int.Parse(documentRow.Cells["id_doco"].Value.ToString());
                ra.DocType = documentRow.Cells["tipo_documento"].Value.ToString();
                ra.DueDate = DateTime.Parse(documentRow.Cells["f_vencimiento"].Value.ToString());
                ra.Folio = int.Parse(documentRow.Cells["folio_doco"].Value.ToString());
                ra.Note = documentRow.Cells["observaciones"].Value.ToString();
                ra.PaymentDay = labelDiaPago.Text;
                ra.Serie = documentRow.Cells["serie_doco"].Value.ToString();

                rAccounts.Add(ra);
            }


            Reports.ReportViewer rv = new Reports.ReportViewer();
            rv.ReportAccounts = rAccounts;

            rv.Show();
        }

        private void toolStripButtonNewCusNote_Click(object sender, EventArgs e)
        {
            labelSysID.Text = "[Nueva Nota]";
            textBoxNote.Text = "";
        }

        private void toolStripButtonSaveCusNote_Click(object sender, EventArgs e)
        {
            if (string.Empty.Equals(labelSysID.Text))
            {
                MessageBox.Show("Seleccione una nota de la lista a ser editada o seleccione \"Nueva Nota\".", "ID Interno", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (string.Empty.Equals(textBoxNote.Text.Trim()))
            {
                MessageBox.Show("El sistema no permite grabar notas vacías, por favor indique contenido.", "Contenido no válido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                textBoxNote.Focus();
                return;
            }

            if (labelSysID.Text.Equals("[Nueva Nota]"))
            {
                if (dataGridViewCustomers.CurrentRow == null)
                {
                    MessageBox.Show("No fue posible identificar el cliente seleccionado, intente de nuevo.", "Selección de cliente no detectada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                int selectedCustomer = int.Parse(dataGridViewCustomers.CurrentRow.Cells[0].Value.ToString());

                dbCustomer.AddNote(selectedCustomer, textBoxNote.Text);
            }

            else
            {
                int selectedNota = int.Parse(labelSysID.Text);
                dbCustomer.UpdateNote(selectedNota, textBoxNote.Text);
            }

            RefreshNotesGrid();

        }

        private void toolStripButtonRemoveCusNote_Click(object sender, EventArgs e)
        {
            int selectedNota = int.Parse(labelSysID.Text);
            dbCustomer.RemoveNote(selectedNota);
            RefreshNotesGrid();
        }

        private void dataGridViewCusNotes_SelectionChanged(object sender, EventArgs e)
        {
            selectAciveNote();
        }

        private void toolStripButtonRestoreNota_Click(object sender, EventArgs e)
        {
            selectAciveNote();
        }

        private void toolStripButtonEscaleSelected_Click(object sender, EventArgs e)
        {
            EscalateDocuments(true);
        }

        private void toolStripButtonEscaleAll_Click(object sender, EventArgs e)
        {
            EscalateDocuments(false);
        }

        # endregion

        private void EscalateDocuments(bool onlySelected)
        {
            foreach (DataGridViewRow row in dataGridViewAccounts.Rows)
            {
                if (onlySelected && !row.Selected) continue;

                int docId = int.Parse(row.Cells["id_doco"].Value.ToString());
                dbAccount.Escale(docId);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.", "Cuentas escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AssignDocuments(bool onlySelected)
        {

            DialogAssign assign = new DialogAssign();
            assign.ShowDialog();

            if (assign.DialogResult == DialogResult.Cancel || string.Empty.Equals(assign.comboBoxCollector.Text)) return;

            int collectorId = int.Parse(assign.comboBoxCollector.SelectedValue.ToString());

            foreach(DataGridViewRow row in dataGridViewAccounts.Rows)
            {
                if (onlySelected && !row.Selected) continue;

                int docId = int.Parse(row.Cells["id_doco"].Value.ToString());
                dbAccount.Assign(docId, collectorId);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido asignadas a " + assign.comboBoxCollector.Text, "Asignación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshNotesGrid()
        {
            if (dataGridViewCustomers.CurrentRow == null) return;

            int selectedCustomer = int.Parse(dataGridViewCustomers.CurrentRow.Cells[0].Value.ToString());
            dataGridViewCusNotes.DataSource = dbCustomer.ReadNotes(selectedCustomer);

            FixNotesGrid();

            if (dataGridViewCusNotes.RowCount == 0) {
                labelSysID.Text = "";
                textBoxNote.Text = "";
            }

        }

        private void selectAciveNote()
        {
            if (dataGridViewCusNotes.CurrentRow == null)
            {
                labelSysID.Text = "";
                textBoxNote.Text = "";
                return;
            }

            labelSysID.Text = dataGridViewCusNotes.CurrentRow.Cells[0].Value.ToString();
            textBoxNote.Text = dataGridViewCusNotes.CurrentRow.Cells[1].Value.ToString();
        }

        private void FixNotesGrid()
        {
            dataGridViewCusNotes.Columns["id_log_cliente"].Visible = false;
            FixColumn(dataGridViewCusNotes.Columns["nota"], 0, "Nota", 400);
        }

        private void RefreshClientesGrid()
        {
            dataGridViewCustomers.DataSource = dbCustomer.ReadCustomers();
            
            dataGridViewCustomers.Columns[0].Width = 50;
            dataGridViewCustomers.Columns[1].Width = 50;
            dataGridViewCustomers.Columns[2].Width = 250;
            dataGridViewCustomers.Columns[3].Width = 50;
            dataGridViewCustomers.Columns[4].Width = 150;

            dataGridViewCustomers.Columns[0].HeaderText = "ID";
            dataGridViewCustomers.Columns[1].HeaderText = "Código";
            dataGridViewCustomers.Columns[2].HeaderText = "Razón Social";
            dataGridViewCustomers.Columns[3].HeaderText = "Ruta";
            dataGridViewCustomers.Columns[4].HeaderText = "Dias de Pago";

            if (dataGridViewCustomers.Rows.Count > 0)
            {
                dataGridViewCustomers.Sort(dataGridViewCustomers.Columns[0], ListSortDirection.Ascending);
                LoadCustomer(dataGridViewCustomers.Rows[0]);
            }
            else
                ClearCustomerSelection();
        }

        private void LoadCustomer(DataGridViewRow row)
        {
            if (row == null) return;
            labelIDCliente.Text = row.Cells[0].Value.ToString();
            labelCodigo.Text = row.Cells[1].Value.ToString();
            labelNombre.Text = row.Cells[2].Value.ToString();
            labelRuta.Text = row.Cells[3].Value.ToString();
            labelDiaPago.Text = row.Cells[4].Value.ToString();
            RefreshAccountsGrid(int.Parse(row.Cells[0].Value.ToString()));
        }

        private void RefreshAccountsGrid(int clientID)
        {
            dataGridViewAccounts.DataSource = dbCustomer.ReadAccounts(clientID);

            FixAccountColumns();

            if (dataGridViewAccounts.Rows.Count > 0)
            {
                dataGridViewAccounts.Sort(dataGridViewAccounts.Columns[0], ListSortDirection.Ascending);
                RefreshPaymentsGrid(int.Parse(dataGridViewAccounts.Rows[0].Cells[0].Value.ToString()));
            }
            else
                dataGridViewPayments.DataSource = null;
        }

        private void FixAccountColumns()
        {

            dataGridViewAccounts.Columns["id_cliente"].Visible = false;
            dataGridViewAccounts.Columns["dia_pago"].Visible = false;
            dataGridViewAccounts.Columns["tipo_documento"].Visible = false;
            dataGridViewAccounts.Columns["ruta"].Visible = false;
            dataGridViewAccounts.Columns["cd_cliente"].Visible = false;
            dataGridViewAccounts.Columns["nombre_cliente"].Visible = false;
            dataGridViewAccounts.Columns["id_doco"].Visible = false;
            dataGridViewAccounts.Columns["ap_id"].Visible = false;

            FixColumn(dataGridViewAccounts.Columns["f_documento"], 0, "Fecha Documento", 80);
            FixColumn(dataGridViewAccounts.Columns["f_vencimiento"], 1, "Fecha Vencimiento", 80);
            FixColumn(dataGridViewAccounts.Columns["dias_vencido"], 2, "Dias Vencimiento", 80);
            FixColumn(dataGridViewAccounts.Columns["f_cobro"], 3, "Fecha Cobro", 80);

            FixColumn(dataGridViewAccounts.Columns["serie_doco"], 4, "Serie", 40);
            FixColumn(dataGridViewAccounts.Columns["folio_doco"], 5, "Doc #", 80);

            FixColumn(dataGridViewAccounts.Columns["tipo_cobro"], 6, "Tipo Cobro", 150);
            FixColumn(dataGridViewAccounts.Columns["facturado"], 7, "Total Facturado", 80);
            FixColumn(dataGridViewAccounts.Columns["saldo"], 8, "Saldo", 80);
            FixColumn(dataGridViewAccounts.Columns["moneda"], 9, "Moneda", 80);
            FixColumn(dataGridViewAccounts.Columns["observaciones"], 10, "Observaciones", 150);


            dataGridViewAccounts.Columns["f_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dataGridViewAccounts.Columns["tipo_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dataGridViewAccounts.Columns["observaciones"].DefaultCellStyle.BackColor = Color.Beige;

            dataGridViewAccounts.Columns["facturado"].DefaultCellStyle.Format = "c";
            dataGridViewAccounts.Columns["saldo"].DefaultCellStyle.Format = "c";
        }

        private void FixColumn(DataGridViewColumn column, int displayedIndex, string HeaderText, int width)
        {
            column.DisplayIndex = displayedIndex;
            column.HeaderText = HeaderText;
            column.Width = width;
        }

        private void RefreshPaymentsGrid(int accountId)
        {
            dataGridViewPayments.DataSource = dbAccount.ReadPayments(accountId);
            FixPaymentsColumns();
        }

        private void FixPaymentsColumns()
        {
            dataGridViewPayments.Columns["id_abono"].Visible = false;

            FixColumn(dataGridViewPayments.Columns["concepto"], 0, "Concepto", 130);
            FixColumn(dataGridViewPayments.Columns["importe_pago"], 1, "Importe", 80);
            FixColumn(dataGridViewPayments.Columns["fecha_deposito"], 2, "Fecha", 80);
            FixColumn(dataGridViewPayments.Columns["folio"], 3, "Folio", 60);
            FixColumn(dataGridViewPayments.Columns["tipo_pago"], 4, "Tipo", 120);
            FixColumn(dataGridViewPayments.Columns["cuenta"], 5, "Cuenta", 100);

            dataGridViewPayments.Columns["importe_pago"].DefaultCellStyle.Format = "c";
        }

        private void ClearCustomerSelection()
        {
            labelIDCliente.Text = "";
            labelCodigo.Text = "";
            labelNombre.Text = "";
            labelRuta.Text = "";
            labelDiaPago.Text = "";

            dataGridViewAccounts.DataSource = null;
            dataGridViewPayments.DataSource = null;

        }
    }
}
