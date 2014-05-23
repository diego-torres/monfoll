using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoGerente.Collectable;
using SeguimientoGerente.Properties;
using Npgsql;

namespace SeguimientoGerente.Config
{
    public partial class FormConfig : Form
    {
        private AdminPaqImp api;
        private Collectable.PostgresImpl.Account AccountInterface = new Collectable.PostgresImpl.Account();
        private bool adminPaqConfigDirty = false;
        private bool dbConfigDirty = false;

        public AdminPaqImp API { get { return api; } set { api = value; } }

        public FormConfig()
        {
            InitializeComponent();
        }

        #region events

        private void FormConfig_Load(object sender, EventArgs e)
        {
            bindingSourceEmpresas.DataSource = api.Empresas;
            comboBoxEmpresas.DataSource = bindingSourceEmpresas;

            comboBoxEmpresas.DisplayMember = "Nombre";
            comboBoxEmpresas.ValueMember = "Id";

            loadConfiguration();
        }

        private void comboBoxEmpresas_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string stringSelected = comboBoxEmpresas.SelectedValue.ToString();
            int iSelected = int.Parse(stringSelected);
            if (iSelected == 0)
            {
                labelRutaEmpresa.Text = "";
                return;
            }

            AsignarRutaEmpresa(iSelected);
            adminPaqConfigDirty = true;
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveAdminPaqConfig();
        }

        private void adminPaqconfig_ValueChanged(object sender, EventArgs e)
        {
            adminPaqConfigDirty = true;
        }

        private void buttonAddFactura_Click(object sender, EventArgs e)
        {
            if (textBoxConceptoFactura.Text.Trim().Length == 0) return;

            listBoxConceptosFactura.Items.Add(textBoxConceptoFactura.Text);
            adminPaqConfigDirty = true;
        }

        private void buttonRemoveFactura_Click(object sender, EventArgs e)
        {
            if (listBoxConceptosFactura.SelectedItems.Count == 0) return;

            for (int i = listBoxConceptosFactura.SelectedIndices.Count - 1; i >= 0; i--)
            {
                listBoxConceptosFactura.Items.RemoveAt(listBoxConceptosFactura.SelectedIndices[i]);
            }

            adminPaqConfigDirty = true;
        }

        private void buttonAddAbono_Click(object sender, EventArgs e)
        {
            if (textBoxConceptoAbono.Text.Trim().Length == 0) return;

            listBoxConceptosAbono.Items.Add(textBoxConceptoAbono.Text);
            adminPaqConfigDirty = true;
        }

        private void buttonRemoveAbono_Click(object sender, EventArgs e)
        {
            if (listBoxConceptosAbono.SelectedItems.Count == 0) return;

            for (int i = listBoxConceptosAbono.SelectedIndices.Count - 1; i >= 0; i--)
            {
                listBoxConceptosAbono.Items.RemoveAt(listBoxConceptosAbono.SelectedIndices[i]);
            }

            adminPaqConfigDirty = true;
        }

        private void FormConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmNSaveAdminPaqConfig())
            {
                e.Cancel = true;
            }
        }

        private void toolStripButtonDownload_Click(object sender, EventArgs e)
        {
            if (!ConfirmNSaveAdminPaqConfig())
            {
                MessageBox.Show("Operación de descarga cancelada por el usuario", "Descarga cancelada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            adminPaqConfigDirty = false;
            dbConfigDirty = false;
            FormMain parent = (FormMain)MdiParent;
            parent.ShowDownload();

            List<Collectable.Account> adminPaqAccounts = api.DownloadCollectables();
            AccountInterface.UploadAccounts(adminPaqAccounts, api.Cancelados, api.Saldados, api.Conceptos);
            parent.CloseDownload();
            this.Close();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    if (!ConfirmNSaveAdminPaqConfig())
                        tabControl1.SelectedIndex = 0;
                    break;
                case 1:
                    if (!ConfirmNSaveDBConfig())
                        tabControl1.SelectedIndex = 1;
                    break;
            }
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            loadConfiguration();
        }

        private void toolStripButtonSaveDB_Click(object sender, EventArgs e)
        {
            SaveDBConfig();
        }

        private void toolStripButtonUndoChanges_Click(object sender, EventArgs e)
        {
            loadConfiguration();
        }

        private void toolStripButtonCheckDB_Click(object sender, EventArgs e)
        {
            if (ValidateDBConfig())
                MessageBox.Show("La configuración de la base de datos es válida", "Conexión exitosa", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            dbConfigDirty = true;
        }

        #endregion

        private bool ConfirmNSaveAdminPaqConfig()
        {
            if (!adminPaqConfigDirty) return true;

            DialogResult confirm = MessageBox.Show("¿Desea guardar los cambios realizados a la configuración de descarga de cuentas de AdminPaq?",
                "¿Guardar cambios?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (confirm)
            {
                case DialogResult.Yes:
                    SaveAdminPaqConfig();
                    return true;
                case DialogResult.No:
                    return true;
            }

            return false;
        }

        private bool ConfirmNSaveDBConfig()
        {
            if (!ValidateDBConfig()) return false;

            if (!dbConfigDirty) return true;

            DialogResult confirm = MessageBox.Show("¿Desea guardar los cambios realizados a la configuración de base de datos de cuentas por cobrar?",
                "¿Guardar cambios?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (confirm)
            {
                case DialogResult.Yes:
                    if (ValidateDBConfig())
                    {
                        SaveDBConfig();
                        return true;
                    }
                    else return false;
                case DialogResult.No:
                    return true;
            }

            return false;
        }

        private void SaveDBConfig()
        {
            Settings set = Settings.Default;

            set.server = textBoxServer.Text;
            set.port = textBoxPort.Text;
            set.user = textBoxUser.Text;
            set.database = textBoxDB.Text;
            set.password = textBoxPassword.Text;

            set.Save();
            dbConfigDirty = false;
        }

        private bool ValidateDBConfig()
        {
            try
            {
                NpgsqlConnection conn;
                string connString = String.Format("Server={0};Port={1};" +
                        "User Id={2};Password={3};Database={4};",
                        textBoxServer.Text, textBoxPort.Text, textBoxUser.Text,
                        textBoxPassword.Text, textBoxDB.Text);
                conn = new NpgsqlConnection(connString);
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("No fue posible establecer una conexión con la base de datos:\n" + err.Message,
                    "Sin conexion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void SaveAdminPaqConfig()
        {
            Settings set = Settings.Default;

            set.empresa = int.Parse(comboBoxEmpresas.SelectedValue.ToString());
            set.fecha_inicio = dateTimePickerFrom.Value;
            set.fecha_fin = dateTimePickerTo.Value;
            set.fecha_doco = radioButtonUseDocDate.Checked;
            set.con_saldo = radioButtonWithAmount.Checked;

            string facturas = "";
            foreach (var listboxItem in listBoxConceptosFactura.Items)
            {
                facturas = facturas + listboxItem + ",";
            }

            facturas = facturas.Substring(0, facturas.Length - 1);
            set.facturas = facturas;

            string abonos = "";
            foreach (var listboxItem in listBoxConceptosAbono.Items)
            {
                abonos = abonos + listboxItem.ToString() + ",";
            }
            abonos = abonos.Substring(0, abonos.Length - 1);
            set.abonos = abonos;
            set.Save();

            adminPaqConfigDirty = false;
        }

        private void AsignarRutaEmpresa(int selected)
        {
            Empresa selectedCompany = api.Empresas.ToList<Empresa>().Find(x => x.Id == selected);
            labelRutaEmpresa.Text = selectedCompany.Ruta;
        }

        private void loadConfiguration()
        {

            Settings set = Settings.Default;

            for (int i = 0; i < comboBoxEmpresas.Items.Count; i++)
            {
                Empresa itemEmpresa = (Empresa)comboBoxEmpresas.Items[i];

                if (itemEmpresa.Id == set.empresa)
                {
                    comboBoxEmpresas.SelectedIndex = i;
                }
            }

            AsignarRutaEmpresa(set.empresa);
            dateTimePickerFrom.Value = set.fecha_inicio;
            dateTimePickerTo.Value = set.fecha_fin;

            radioButtonUseDocDate.Checked = set.fecha_doco;
            radioButtonUseCollectDate.Checked = !set.fecha_doco;

            radioButtonWithAmount.Checked = set.con_saldo;
            radioButtonAllDocuments.Checked = !set.con_saldo;

            string[] facturas = set.facturas.Split(',');
            string[] abonos = set.abonos.Split(',');

            listBoxConceptosFactura.Items.Clear();
            foreach (string codigoFactura in facturas)
            {
                listBoxConceptosFactura.Items.Add(codigoFactura);
            }

            listBoxConceptosAbono.Items.Clear();
            foreach (string codigoAbono in abonos)
            {
                listBoxConceptosAbono.Items.Add(codigoAbono);
            }

            textBoxServer.Text = set.server;
            textBoxPort.Text = set.port;
            textBoxDB.Text = set.database;
            textBoxUser.Text = set.user;
            textBoxPassword.Text = set.password;

            adminPaqConfigDirty = false;
            dbConfigDirty = false;
        }
    }
}
