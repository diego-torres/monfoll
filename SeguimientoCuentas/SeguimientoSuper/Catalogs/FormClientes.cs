using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;

namespace SeguimientoSuper.Catalogs
{
    public partial class FormClientes : Form
    {
        private Customer dbCustomer = new Customer();

        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            RefreshClientesGrid();
        }

        private void RefreshClientesGrid()
        {
            dataGridViewClientes.DataSource = dbCustomer.ReadCustomers();

            dataGridViewClientes.Columns[0].Width = 50;
            dataGridViewClientes.Columns[1].Width = 50;
            dataGridViewClientes.Columns[2].Width = 250;
            dataGridViewClientes.Columns[3].Width = 50;
            dataGridViewClientes.Columns[4].Width = 150;

            dataGridViewClientes.Columns[0].HeaderText = "ID";
            dataGridViewClientes.Columns[1].HeaderText = "Código";
            dataGridViewClientes.Columns[2].HeaderText = "Razón Social";
            dataGridViewClientes.Columns[3].HeaderText = "Ruta";
            dataGridViewClientes.Columns[4].HeaderText = "Dias de Pago";

            if (dataGridViewClientes.Rows.Count > 0)
            {
                dataGridViewClientes.Sort(dataGridViewClientes.Columns[0], ListSortDirection.Ascending);
                LoadCustomer(dataGridViewClientes.Rows[0]);
            }
            else
                ClearCustomerSelection();
        }

        private void dataGridViewClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClientes.CurrentRow == null) return;
            LoadCustomer(dataGridViewClientes.CurrentRow);
        }

        private void LoadCustomer(DataGridViewRow row)
        {
            if (row == null) return;
            labelIDCliente.Text = row.Cells[0].Value.ToString();
            labelCodigo.Text = row.Cells[1].Value.ToString();
            labelNombre.Text = row.Cells[2].Value.ToString();
            labelRuta.Text = row.Cells[3].Value.ToString();
            labelDiaPago.Text = row.Cells[4].Value.ToString();
        }

        private void ClearCustomerSelection()
        {
            labelIDCliente.Text = "";
            labelCodigo.Text = "";
            labelNombre.Text = "";
            labelRuta.Text = "";
            labelDiaPago.Text = "";
        }

    }
}
