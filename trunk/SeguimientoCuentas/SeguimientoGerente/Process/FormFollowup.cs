using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoGerente.Collectable;
using CommonAdminPaq;
using SeguimientoGerente.Properties;

namespace SeguimientoGerente.Process
{
    public partial class FormFollowup : Form
    {
        private Account account;
        private bool followUpDirty = false;
        private bool adminPaqDirty = false;
        private bool fireEvents = true;

        private AdminPaqImp api;
        public AdminPaqImp API { get { return api; } set { api = value; } }

        private Collectable.PostgresImpl.Account postgresAcct = new Collectable.PostgresImpl.Account();
        public Account Account { get { return account; } set { account = value; } }

        public FormFollowup()
        {
            InitializeComponent();
        }

        #region EVENTS
        private void FormFollowup_Load(object sender, EventArgs e)
        {
            if (account == null)
            {
                MessageBox.Show("Ocurrió un error al intentar cargar la información de la cuenta, intentelo de nuevo", "No se pudo cargar la cuenta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            this.Text = "Seguimiento - Documento por cobrar: " + account.Serie + "-" + account.Folio.ToString();

            LoadDates();
            LoadCustomer();
            LoadDocument();
            LoadPayments();
            RefreshFollowUpGrid();
            followUpDirty = false;
            adminPaqDirty = false;
        }

        private void toolStripButtonAddFollowUp_Click(object sender, EventArgs e)
        {
            LockFollowUp(false);

            labelSysFlag.Text = "";
            comboBoxFollowUpType.Text = "";
            textBoxFollowUpNote.Text = "";
            labelFollowUpId.Text = "[Nuevo Registro]";
        }

        private void toolStripButtonSaveFollowUp_Click(object sender, EventArgs e)
        {
            SaveFollowUp();
        }

        private void FollowUp_Changed(object sender, EventArgs e)
        {
            followUpDirty = true;
        }

        private void toolStripButtonUndoFollowUp_Click(object sender, EventArgs e)
        {
            LoadFollowUp(dataGridViewFollowUp.CurrentRow);
            followUpDirty = false;
        }

        private void toolStripButtonRemoveFollowUp_Click(object sender, EventArgs e)
        {
            if (ValidateFollowUp())
            {
                int deleteFollowUp = int.Parse(dataGridViewFollowUp.CurrentRow.Cells[0].Value.ToString());
                postgresAcct.RemoveFollowUp(deleteFollowUp);
            }
            followUpDirty = false;
            RefreshFollowUpGrid();
        }

        private void dataGridViewFollowUp_SelectionChanged(object sender, EventArgs e)
        {

            if (dataGridViewFollowUp.CurrentRow == null) return;

            string currentValue = dataGridViewFollowUp.CurrentRow.Cells[0].Value.ToString();

            if (followUpDirty && !currentValue.Equals(labelFollowUpId.Text) && fireEvents)
                if (!ConfirmNSaveFollowUp())
                {
                    if (labelFollowUpId.Text == "[Nuevo Registro]") return;
                    ForceGridSelection(labelFollowUpId.Text);
                    followUpDirty = true;
                    return;
                }

            if (fireEvents)
            {
                LoadFollowUp(dataGridViewFollowUp.CurrentRow);
                followUpDirty = false;
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (!adminPaqDirty) return;

            if (SaveAdminPaq())
            {
                RefreshFollowUpGrid();
                adminPaqDirty = false;
            }
        }

        private void AdminPaq_ValueChanged(object sender, EventArgs e)
        {
            adminPaqDirty = true;
        }


        private void FormFollowup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (followUpDirty)
            {
                if (!ConfirmNSaveFollowUp())
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (adminPaqDirty)
            {
                if (!ConfirmNSaveAdminPaq())
                {
                    e.Cancel = true;
                    return;
                }
            }

        }

        #endregion

        private bool ConfirmNSaveAdminPaq()
        {
            DialogResult dr = MessageBox.Show("¿Desea guardar los cambios realizados a los datos del documento?", "Cambios en documento", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (dr)
            {
                case DialogResult.Yes:
                    return SaveAdminPaq();
                case DialogResult.No:
                    return true;
                default:
                    return false;
            }
        }

        private bool SaveAdminPaq()
        {
            if (dateTimePickerCollectDate.Checked)
                account.CollectDate = dateTimePickerCollectDate.Value;
            else
                account.CollectDate = new DateTime(0);

            account.CollectType = textBoxCollectType.Text;
            account.Note = textBoxNote.Text;
            try
            {
                api.UpdateCollectable(account);
                postgresAcct.UpdateAccountById(account);
                MessageBox.Show("Los datos han sido grabados exitosamente.", "Datos Guardados", MessageBoxButtons.OK, MessageBoxIcon.Information);

                FormMain fMain = (FormMain)this.MdiParent;

                if (fMain.IsProcessOpen)
                    fMain.RefreshProcessAccounts();


                adminPaqDirty = false;

                return true;
            }
            catch (Exception ex)
            {
                ErrLogger.Log("Unable to save account data: " + ex.Message);
                MessageBox.Show("No fue posible agregar los datos: \n" + ex.Message, "Error al grabar datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ForceGridSelection(string followUpId)
        {
            string currentValue = dataGridViewFollowUp.CurrentRow.Cells[0].Value.ToString();
            if (currentValue.Equals(followUpId)) return;

            foreach (DataGridViewRow row in dataGridViewFollowUp.Rows)
            {
                if (row.Cells[0].Value.ToString().Equals(followUpId))
                {
                    fireEvents = false;
                    dataGridViewFollowUp.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    fireEvents = true;
                    return;
                }
            }
        }

        private void SaveFollowUp()
        {
            if (labelFollowUpId.Text.Equals("[Nuevo Registro]"))
            {
                AddFollowUp();
            }
            else
            {
                UpdateFollowUp();
            }
            followUpDirty = false;
            RefreshFollowUpGrid();
        }

        private bool ConfirmNSaveFollowUp()
        {
            DialogResult dr = MessageBox.Show("¿Desea guardar los cambios realizados al seguimiento de la cuenta?", "Cambios en seguimiento", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (dr)
            {
                case DialogResult.Yes:
                    SaveFollowUp();
                    return true;
                case DialogResult.No:
                    return true;
                default:
                    return false;
            }
        }

        private void AddFollowUp()
        {
            if (ValidateFollowUp())
            {
                postgresAcct.AddFollowUp(comboBoxFollowUpType.Text, textBoxFollowUpNote.Text, account.DocId);
            }
        }

        private void UpdateFollowUp()
        {
            if (ValidateFollowUp())
            {
                int updatingFollowUp = int.Parse(labelFollowUpId.Text);
                postgresAcct.UpdateFollowUp(comboBoxFollowUpType.Text, textBoxFollowUpNote.Text, updatingFollowUp);
            }
        }

        private bool ValidateFollowUp()
        {
            if (!string.Empty.Equals(labelSysFlag.Text))
            {
                MessageBox.Show("No está permitido editar los registros de sistema", "Registro de sistema", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }

            if (string.Empty.Equals(textBoxFollowUpNote.Text.Trim()))
            {
                MessageBox.Show("Debe capturar la información del detalle del seguimiento", "Detalle del seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                textBoxFollowUpNote.Focus();
                return false;
            }

            if (string.Empty.Equals(comboBoxFollowUpType.Text.Trim()))
            {
                MessageBox.Show("Debe seleccionar la naturaleza del seguimiento", "Detalle del seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                comboBoxFollowUpType.Focus();
                return false;
            }

            string[] validFollowUps = { "Llamada", "Visita", "Email", "Otro", "Cerrado" };
            bool validFollowUp = Array.Exists(validFollowUps, x => x.Equals(comboBoxFollowUpType.Text));

            if (!validFollowUp)
            {
                MessageBox.Show("La naturaleza de seguimiento seleccionada no se encontró como una naturaleza de seguimiento válida. " +
                    "\nIntente seleccionar nuevamente el tipo de seguimiento.", "Detalle del seguimiento", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                comboBoxFollowUpType.Focus();
                return false;
            }

            return true;
        }

        private void LoadDates()
        {
            this.labelDocDate.Text = account.DocDate.ToShortDateString();
            this.labelDueDate.Text = account.DueDate.ToShortDateString();
            if (account.CollectDate.Ticks > 0)
            {
                this.dateTimePickerCollectDate.Value = account.CollectDate;
                this.dateTimePickerCollectDate.Format = DateTimePickerFormat.Short;
                dateTimePickerCollectDate.Checked = true;
            }
            else
            {
                dateTimePickerCollectDate.CustomFormat = " ";
                dateTimePickerCollectDate.Checked = false;
            }

            DateTime now = DateTime.Now;
            TimeSpan elapsed = now.Subtract(account.DueDate);
            int elapsedDays = int.Parse(elapsed.TotalDays.ToString("0")) - 1;
            this.labelDueDays.Text = elapsedDays.ToString();
        }

        private void LoadCustomer()
        {
            this.labelCusAgent.Text = account.Company.AgentCode;
            this.labelCusCode.Text = account.Company.Code;
            this.labelCusName.Text = account.Company.Name;
            this.labelCusCollectionDays.Text = account.Company.PaymentDay;
        }

        private void LoadDocument()
        {
            labelSerie.Text = account.Serie;
            labelFolio.Text = account.Folio.ToString();
            labelDocID.Text = account.DocId.ToString();
            labelTotal.Text = string.Format("{0:C2}", account.Amount);
            labelSaldo.Text = string.Format("{0:C2}", account.Balance);
            labelCurrency.Text = account.Currency;
            textBoxCollectType.Text = account.CollectType;
            textBoxNote.Text = account.Note;
        }

        private void LoadPayments()
        {
            dataGridViewPayments.DataSource = account.Payments;
            FixPaymentsColumns();
        }

        private void RefreshFollowUpGrid()
        {
            dataGridViewFollowUp.DataSource = postgresAcct.FollowUp(account.DocId);

            if (dataGridViewFollowUp.DataSource == null || dataGridViewFollowUp.Columns.Count <= 0) return;

            dataGridViewFollowUp.Sort(dataGridViewFollowUp.Columns[0], ListSortDirection.Descending);
            FixFollowUpColumns();

            if (dataGridViewFollowUp.Rows.Count <= 0) return;
            LoadFollowUp(dataGridViewFollowUp.Rows[0]);
        }

        private void FixPaymentsColumns()
        {
            dataGridViewPayments.Columns["PaymentId"].Visible = false;
            dataGridViewPayments.Columns["DocId"].Visible = false;

            FixColumn(dataGridViewPayments.Columns["Concept"], 0, "Concepto", 230);
            FixColumn(dataGridViewPayments.Columns["Amount"], 1, "Importe", 80);
            FixColumn(dataGridViewPayments.Columns["DepositDate"], 2, "Fecha", 80);
            FixColumn(dataGridViewPayments.Columns["Folio"], 3, "Folio", 60);
            FixColumn(dataGridViewPayments.Columns["PaymentType"], 4, "Tipo", 250);
            FixColumn(dataGridViewPayments.Columns["Account"], 5, "Cuenta", 200);

            dataGridViewPayments.Columns["Amount"].DefaultCellStyle.Format = "c";
        }

        private void FixFollowUpColumns()
        {
            dataGridViewFollowUp.Columns["id_seguimiento"].Visible = false;
            dataGridViewFollowUp.Columns["id_movimiento"].Visible = false;
            dataGridViewFollowUp.Columns["system_based"].Visible = false;
            //dataGridViewFollowUp.Columns["ts_seguimiento"].Visible = false;

            FixColumn(dataGridViewFollowUp.Columns["movimiento"], 0, "Movimiento", 120);
            FixColumn(dataGridViewFollowUp.Columns["seguimiento"], 1, "Seguimiento", 350);
            FixColumn(dataGridViewFollowUp.Columns["ts_seguimiento"], 2, "Fecha", 150);
        }

        private void FixColumn(DataGridViewColumn column, int displayedIndex, string HeaderText, int width)
        {
            column.DisplayIndex = displayedIndex;
            column.HeaderText = HeaderText;
            column.Width = width;
        }

        private void LoadFollowUp(DataGridViewRow row)
        {
            if (row == null) return;
            labelFollowUpId.Text = row.Cells[0].Value.ToString();
            bool isSystem = bool.Parse(row.Cells[3].Value.ToString());
            LockFollowUp(isSystem);

            if (isSystem)
            {
                labelSysFlag.Text = "Seguimiento automático";
                comboBoxFollowUpType.Text = "Otro";
                textBoxFollowUpNote.Text = row.Cells[4].Value.ToString();
            }
            else
            {
                labelSysFlag.Text = "";
                comboBoxFollowUpType.Text = row.Cells[2].Value.ToString();
                textBoxFollowUpNote.Text = row.Cells[4].Value.ToString();
            }
            followUpDirty = false;
        }

        private void LockFollowUp(bool Lock)
        {
            textBoxFollowUpNote.Enabled = !Lock;
            comboBoxFollowUpType.Enabled = !Lock;
        }
    }
}
