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

namespace SeguimientoSuper.Catalogs
{
    public partial class FormClientes : Form
    {
        private Customer dbCustomer = new Customer();
       
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
        }

        private void dataGridViewAccounts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridViewAccounts.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;

            /*"SELECT ID_DOCO, F_DOCUMENTO, F_VENCIMIENTO, F_COBRO, CTRL_CUENTA.ID_CLIENTE, CD_CLIENTE, NOMBRE_CLIENTE, RUTA, DIA_PAGO, SERIE_DOCO, FOLIO_DOCO, " +
                "TIPO_DOCUMENTO, TIPO_COBRO, FACTURADO, SALDO, MONEDA, OBSERVACIONES " +
                "FROM CTRL_CUENTA INNER JOIN CAT_CLIENTE ON CTRL_CUENTA.ID_CLIENTE = CAT_CLIENTE.ID_CLIENTE " +
                "WHERE CTRL_CUENTA.ID_CLIENTE = " + customerId.ToString() + ";";*/
            
            Collectable.Account account = new Collectable.Account();
            account.DocId = int.Parse(dataGridViewAccounts.CurrentRow.Cells["id_doco"].Value.ToString());
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
        # endregion

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

            /*dataGridViewClientes.Columns[0].Width = 50;
            dataGridViewClientes.Columns[1].Width = 50;
            dataGridViewClientes.Columns[2].Width = 250;
            dataGridViewClientes.Columns[3].Width = 50;
            dataGridViewClientes.Columns[4].Width = 150;

            dataGridViewClientes.Columns[0].HeaderText = "ID";
            dataGridViewClientes.Columns[1].HeaderText = "Código";
            dataGridViewClientes.Columns[2].HeaderText = "Razón Social";
            dataGridViewClientes.Columns[3].HeaderText = "Ruta";
            dataGridViewClientes.Columns[4].HeaderText = "Dias de Pago";*/

            if (dataGridViewAccounts.Rows.Count > 0)
            {
                dataGridViewAccounts.Sort(dataGridViewAccounts.Columns[0], ListSortDirection.Ascending);
                RefreshPaymentsGrid(int.Parse(dataGridViewAccounts.Rows[0].Cells[0].Value.ToString()));
                //LoadCustomer(dataGridViewClientes.Rows[0]);
            }
            else
                dataGridViewPayments.DataSource = null;
        }

        private void RefreshPaymentsGrid(int accountId)
        {
            dataGridViewPayments.DataSource = dbCustomer.ReadPayments(accountId);
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
