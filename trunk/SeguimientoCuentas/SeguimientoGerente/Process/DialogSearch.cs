using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoGerente.Collectable.PostgresImpl;
using CommonAdminPaq;

namespace SeguimientoGerente.Process
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

                foreach (DataRow dr in DtCustomer.Rows)
                {
                    if (!textBoxClient.AutoCompleteCustomSource.Contains(dr["nombre_cliente"].ToString()))
                        textBoxClient.AutoCompleteCustomSource.Add(dr["nombre_cliente"].ToString());
                }

               // DataTable dtFolios = dbAccount.ReadFolios();
                foreach (DataRow dr in DtFolios.Rows)
                {
                    if (!textBoxFolio.AutoCompleteCustomSource.Contains(dr["folio_doco"].ToString()))
                        textBoxFolio.AutoCompleteCustomSource.Add(dr["folio_doco"].ToString());
                }

                comboBoxClient.SelectedIndex = -1;
                comboBoxClient.Text = "";
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
                textBoxClient.Text = dbCustomer.CustomerNameByCode(comboBoxClient.Text);
                inCustSelection = false;
            }catch(Exception ex){
                ErrLogger.Log(ex.StackTrace);
                MessageBox.Show("No fue posible consultar el nombre del cliente en la base de datos. Intente más tarde: \n" +
                    ex.Message, "Error al obtener nombre del cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxClient_TextChanged(object sender, EventArgs e)
        {
            if (textBoxClient.Text.Trim().Equals(string.Empty) || inCustSelection) return;
            try {
                DataTable dtCustomers = dbCustomer.ReadCustomers();
                foreach(DataRow dr in dtCustomers.Rows)
                {
                    if(dr["nombre_cliente"].ToString().ToLower().Equals(textBoxClient.Text.Trim().ToLower()))
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
