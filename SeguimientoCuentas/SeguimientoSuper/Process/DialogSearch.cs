using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;
using CommonAdminPaq;

namespace SeguimientoSuper.Process
{
    public partial class DialogSearch : Form
    {

        public DataTable DtCustomer { get; set; }
        public DataTable DtSeries { get; set; }
        public DataTable DtFolios { get; set; }

        private Customer dbCustomer = new Customer();
        //private Account dbAccount = new Account();

        private bool inCustSelection = false;
        private bool inCustNameTyping = false;

        public DialogSearch()
        {
            InitializeComponent();
        }

        private void DialogSearch_Load(object sender, EventArgs e)
        {
            try {
                comboBoxClient.DataSource = DtCustomer;
                comboBoxClient.DisplayMember = "cd_cliente";
                comboBoxClient.ValueMember = "id_cliente";
                
                comboBoxSerie.DataSource = DtSeries;
                comboBoxSerie.DisplayMember = "serie_doco";
                comboBoxSerie.ValueMember = "serie_doco";

                comboBoxClientName.DataSource = DtCustomer;
                comboBoxClientName.DisplayMember = "nombre_cliente";
                comboBoxClientName.ValueMember = "id_cliente";

                comboBoxFolios.DataSource = DtFolios;
                comboBoxFolios.DisplayMember = "folio_doco";
                comboBoxFolios.ValueMember = "folio_doco";

                comboBoxClient.SelectedIndex = -1;
                comboBoxClient.Text = "";
                
                comboBoxClientName.SelectedIndex = -1;
                comboBoxClientName.Text = "";

                comboBoxFolios.SelectedIndex = -1;
                comboBoxFolios.Text = "";

            }catch(Exception ex){
                ErrLogger.Log(ex.StackTrace);
                MessageBox.Show("No fue posible consultar algunos datos en la base de datos. Intente más tarde: \n" +
                    ex.Message, "Error al obtener datos para búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void comboBoxClient_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboBoxClient.Text.Equals(string.Empty)||inCustNameTyping) return;
            try {
                comboBoxClientName.Text = dbCustomer.CustomerNameByCode(comboBoxClient.Text);
                inCustSelection = false;
            }catch(Exception ex){
                ErrLogger.Log(ex.StackTrace);
                MessageBox.Show("No fue posible consultar el nombre del cliente en la base de datos. Intente más tarde: \n" +
                    ex.Message, "Error al obtener nombre del cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxClient_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxClientName.Text.Trim().Equals(string.Empty) || inCustSelection) return;
            try {
                DataTable dtCustomers = dbCustomer.ReadCustomers();
                foreach(DataRow dr in dtCustomers.Rows)
                {
                    if (dr["nombre_cliente"].ToString().ToLower().Equals(comboBoxClientName.Text.Trim().ToLower()))
                        comboBoxClient.Text = dr["cd_cliente"].ToString();
                }

                comboBoxSerie.DataSource = dbCustomer.SeriesFromCustomer(comboBoxClient.Text);

                inCustNameTyping = false;
            }catch(Exception ex){
                ErrLogger.Log(ex.StackTrace);
                MessageBox.Show("No fue posible consultar el código del cliente en la base de datos. Intente más tarde: \n" +
                    ex.Message, "Error al obtener nombre del cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxClient_Enter(object sender, EventArgs e)
        {
            inCustSelection = true;
        }

        private void textBoxClient_Enter(object sender, EventArgs e)
        {
            inCustNameTyping = true;
        }

        private void comboBoxClient_Leave(object sender, EventArgs e)
        {
            inCustSelection = false;
        }
    }
}
