using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using SeguimientoGerente.Properties;
using SeguimientoGerente.Collectable;
using SeguimientoGerente.Collectable.PostgresImpl;

namespace SeguimientoGerente.Catalogs
{
    public partial class FormCobradores : Form
    {
        private bool collectorDirty = false;
        private bool noteDirty = false;
        private bool fireEvents = true;
        
        private Collector dbCollector = new Collector();
        private SeguimientoGerente.Collectable.PostgresImpl.Account dbAccount = new Collectable.PostgresImpl.Account();

        private const string NEW_CAPTION = "[Nuevo]";
        private const string ES_LOCAL_CAPTION = "Local";

        public FormCobradores()
        {
            InitializeComponent();
        }

        public void RefreshAccounts()
        {
            refreshAssignmentsGrid();
        }

    # region EVENTS
        private void FormCobradoresGrid_Load(object sender, EventArgs e)
        {
            fireEvents = false;
            refreshCollectorsGrid();
            fireEvents = true;
            collectorDirty = false;
            noteDirty = false;
        }

        private void dataGridViewCollectors_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewCollectors.CurrentRow == null) return;

            string currentValue = dataGridViewCollectors.CurrentRow.Cells[0].Value.ToString();

            if (collectorDirty && !currentValue.Equals(labelID.Text) && fireEvents)
                if (!ConfirmNSaveCollector())
                {
                    ForceCollectorSelection(labelID.Text);
                    collectorDirty = true;
                    return;
                }
            if (fireEvents)
            {
                loadCollector(dataGridViewCollectors.CurrentRow);
                refreshNotesGrid();
            }
        }

        private void collectorControls_TextChanged(object sender, EventArgs e)
        {
            collectorDirty = true;
        }

        private void dataGridViewAssignedDocuments_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dataGridViewAssignedDocuments.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;

            /*"SELECT ID_DOCO, F_DOCUMENTO, F_VENCIMIENTO, F_COBRO, CTRL_CUENTA.ID_CLIENTE, CD_CLIENTE, NOMBRE_CLIENTE, RUTA, DIA_PAGO, SERIE_DOCO, FOLIO_DOCO, " +
                "TIPO_DOCUMENTO, TIPO_COBRO, FACTURADO, SALDO, MONEDA, OBSERVACIONES " +
                "FROM CTRL_CUENTA INNER JOIN CAT_CLIENTE ON CTRL_CUENTA.ID_CLIENTE = CAT_CLIENTE.ID_CLIENTE " +
                "WHERE CTRL_CUENTA.ID_CLIENTE = " + customerId.ToString() + ";";*/

            Collectable.Account account = new Collectable.Account();
            account.DocId = int.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["id_doco"].Value.ToString());
            account.DocDate = DateTime.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["f_documento"].Value.ToString());
            account.DueDate = DateTime.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["f_vencimiento"].Value.ToString());
            account.CollectDate = DateTime.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["f_cobro"].Value.ToString());
            account.Serie = dataGridViewAssignedDocuments.CurrentRow.Cells["serie_doco"].Value.ToString();
            account.Folio = int.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["folio_doco"].Value.ToString());
            account.DocType = dataGridViewAssignedDocuments.CurrentRow.Cells["tipo_documento"].Value.ToString();
            account.CollectType = dataGridViewAssignedDocuments.CurrentRow.Cells["tipo_cobro"].Value.ToString();
            account.Amount = double.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["facturado"].Value.ToString());
            account.Balance = double.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["saldo"].Value.ToString());
            account.Currency = dataGridViewAssignedDocuments.CurrentRow.Cells["moneda"].Value.ToString();
            account.Note = dataGridViewAssignedDocuments.CurrentRow.Cells["observaciones"].Value.ToString();

            Collectable.Company company = new Collectable.Company();
            company.Id = int.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["id_cliente"].Value.ToString());
            company.Code = dataGridViewAssignedDocuments.CurrentRow.Cells["cd_cliente"].Value.ToString();
            company.Name = dataGridViewAssignedDocuments.CurrentRow.Cells["nombre_cliente"].Value.ToString();
            company.AgentCode = dataGridViewAssignedDocuments.CurrentRow.Cells["ruta"].Value.ToString();
            company.PaymentDay = dataGridViewAssignedDocuments.CurrentRow.Cells["dia_pago"].Value.ToString();
            account.Company = company;

            int accountId = int.Parse(dataGridViewAssignedDocuments.CurrentRow.Cells["id_doco"].Value.ToString());
            DataTable dtPayments = dbAccount.ReadPayments(accountId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = new Payment();
                payment.PaymentId = int.Parse(paymentRow["id_abono"].ToString());
                payment.DocId = account.DocId;
                payment.PaymentType = paymentRow["tipo_pago"].ToString();
                payment.Amount = double.Parse(paymentRow["importe_pago"].ToString());
                payment.Folio = int.Parse(paymentRow["folio"].ToString());
                payment.Concept = paymentRow["concepto"].ToString();
                payment.DepositDate = DateTime.Parse(paymentRow["fecha_deposito"].ToString());
                payment.Account = paymentRow["cuenta"].ToString();

                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void toolStripButtonPrintAssignments_Click(object sender, EventArgs e)
        {
            List<Reports.Account> rAccounts = new List<Reports.Account>();

            foreach (DataGridViewRow documentRow in dataGridViewAssignedDocuments.Rows)
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


            Reports.ReportViewer rv = new Reports.ReportViewer();
            rv.ReportAccounts = rAccounts;

            rv.Show();
        }

        #region Notes

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                int selectedId = 0;
                bool idSelected = int.TryParse(labelID.Text, out selectedId);
                if (!idSelected)
                {
                    MessageBox.Show("Seleccione un cobrador de la lista para ver las notas.", "Identificador Interno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tabControl1.SelectTab(0);
                    return;
                }
                else 
                {
                    refreshNotesGrid();
                }
            }

        }

        private void dataGridViewNotes_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewNotes.CurrentRow == null) return;

            string currentValue = dataGridViewNotes.CurrentRow.Cells[0].Value.ToString();

            if (noteDirty && !currentValue.Equals(labelNoteID.Text) && fireEvents)
                if (!ConfirmNSaveNote())
                {
                    ForceNoteSelection(labelNoteID.Text);
                    noteDirty = true;
                    return;
                }
            if (fireEvents)
                loadNote(dataGridViewNotes.CurrentRow);
        }

        private void toolStripButtonSaveNote_Click(object sender, EventArgs e)
        {
            SaveNote();
        }

        private void toolStripButtonAddNote_Click(object sender, EventArgs e)
        {
            if (noteDirty)
                if (!ConfirmNSaveNote()) return;

            clearNoteSelection();
            labelNoteID.Text = NEW_CAPTION;

            textBoxComment.Focus();
            noteDirty = false;
        }

        private void toolStripButtonRemoveNote_Click(object sender, EventArgs e)
        {
            RemoveNote();
            noteDirty = false;
        }

        private void toolStripButtonUndoNote_Click(object sender, EventArgs e)
        {
            loadNote(dataGridViewNotes.CurrentRow);
        }

        #endregion

        #region toolBox

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveCollector();
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            if (collectorDirty)
                if (!ConfirmNSaveCollector()) return;

            clearCollectorSelection();
            labelID.Text = NEW_CAPTION;

            textBoxNombre.Focus();
            collectorDirty = false;
        }

        private void toolStripButtonRemoveCollector_Click(object sender, EventArgs e)
        {
            RemoveCollector();
            collectorDirty = false;
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            loadCollector(dataGridViewCollectors.CurrentRow);
        }
        #endregion

    # endregion

        private void ForceNoteSelection(string noteId)
        {
            string currentValue = dataGridViewNotes.CurrentRow.Cells[0].Value.ToString();
            if (currentValue.Equals(noteId)) return;

            foreach (DataGridViewRow row in dataGridViewNotes.Rows)
            {
                if (row.Cells[0].Value.ToString().Equals(noteId))
                {
                    fireEvents = false;
                    dataGridViewNotes.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    fireEvents = true;
                    return;
                }
            }
        }

        private void refreshNotesGrid()
        {
            dataGridViewNotes.DataSource = dbCollector.ReadLogCobrador(labelID.Text);

            dataGridViewNotes.Columns[0].Width = 0;
            dataGridViewNotes.Columns[1].Width = 300;
            
            dataGridViewNotes.Columns[0].HeaderText = "ID";
            dataGridViewNotes.Columns[0].Visible = false;
            dataGridViewNotes.Columns[1].HeaderText = "Nota";
            
            if (dataGridViewNotes.Rows.Count > 0)
            {
                dataGridViewNotes.Sort(dataGridViewNotes.Columns[1], ListSortDirection.Ascending);
                loadNote(dataGridViewNotes.Rows[0]);
            }
            else clearNoteSelection();
        }

        private void loadNote(DataGridViewRow row)
        {
            if (row == null) return;
            labelNoteID.Text = row.Cells[0].Value.ToString();
            textBoxComment.Text = row.Cells[1].Value.ToString();
            textBoxComment.Focus();
            noteDirty = false;
        }

        private void clearNoteSelection()
        {
            labelNoteID.Text = String.Empty;
            textBoxComment.Text = String.Empty;
            textBoxComment.Focus();
            noteDirty = false;
        }

        private bool ConfirmNSaveNote()
        {
            DialogResult confirm = MessageBox.Show("¿Desea guardar los cambios realizados a la nota seleccionada?", "Cambios a nota", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (confirm)
            {
                case DialogResult.Yes:
                    SaveNote();
                    break;
                case DialogResult.Cancel:
                    return false;
            }

            return true;
        }

        private void SaveNote()
        {
            if (!ValidateNote()) return;

            if (labelNoteID.Text.Equals(NEW_CAPTION))
                AddNote();
            else
                UpdateNote();

            noteDirty = false;
        }

        private bool ValidateNote()
        { 
            if(String.Empty.Equals(labelNoteID.Text))
            {
                MessageBox.Show("Seleccione una nota de la lista o seleccione [Nueva Nota] para agregar una nota nueva", "Identificador Interno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                clearNoteSelection();
                return false;
            }

            if (String.Empty.Equals(textBoxComment.Text.Trim()))
            {
                MessageBox.Show("No se pueden agregar Notas vacías", "Descripción de la Nota", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxComment.Focus();
                return false;  
            }

            return true;
        }

        private void AddNote()
        {
            dbCollector.AddLogCobrador(int.Parse(labelID.Text), textBoxComment.Text);
            noteDirty = false;
            refreshNotesGrid();
        }

        private void UpdateNote()
        {
            dbCollector.UpdateLogCobrador(int.Parse(labelNoteID.Text), textBoxComment.Text);
            noteDirty = false;
            refreshNotesGrid();
        }

        private void RemoveNote()
        {
            int selectedId = 0;
            bool idSelected = int.TryParse(labelNoteID.Text, out selectedId);
            if (!idSelected)
            {
                MessageBox.Show("Seleccione una nota de la lista para intentar eliminarla", "Identificador Interno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                refreshNotesGrid();
                return;
            }

            string question = "¿Está usted seguro de eliminar la nota seleccionada?";
            DialogResult confirm = MessageBox.Show(question, "Borrar nota", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;


            dbCollector.RemoveLogCobrador(selectedId);
            collectorDirty = false;
            refreshCollectorsGrid();

        }

        private void ForceCollectorSelection(string sInternalID)
        {
            string currentValue = dataGridViewCollectors.CurrentRow.Cells[0].Value.ToString();
            if (currentValue.Equals(sInternalID)) return;

            foreach (DataGridViewRow row in dataGridViewCollectors.Rows)
            {
                if (row.Cells[0].Value.ToString().Equals(sInternalID))
                {
                    fireEvents = false;
                    dataGridViewCollectors.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    fireEvents = true;
                    return;
                }
            }
        }

        private void loadCollector(DataGridViewRow row)
        {
            if (row == null) return;
            labelID.Text = row.Cells[0].Value.ToString();
            textBoxNombre.Text = row.Cells[1].Value.ToString();
            comboBoxArea.Text = row.Cells[2].Value.ToString();
            textBoxNombre.Focus();

            refreshAssignmentsGrid();

            collectorDirty = false;
        }

        private void refreshAssignmentsGrid()
        { 
            if(dataGridViewCollectors.CurrentRow == null) return;
            int selectedCollectorId = int.Parse(dataGridViewCollectors.CurrentRow.Cells["id_cobrador"].Value.ToString());
            dataGridViewAssignedDocuments.DataSource = dbCollector.ReadAssignments(selectedCollectorId);

            if (dataGridViewAssignedDocuments.Rows.Count > 0)
            {
                dataGridViewAssignedDocuments.Sort(dataGridViewAssignedDocuments.Columns["cd_cliente"], ListSortDirection.Ascending);
                FixAccountColumns();
            }
            else
                dataGridViewAssignedDocuments.DataSource = null;
        }

        private void FixAccountColumns()
        {

            dataGridViewAssignedDocuments.Columns["id_cliente"].Visible = false;
            dataGridViewAssignedDocuments.Columns["dia_pago"].Visible = false;
            

            FixColumn(dataGridViewAssignedDocuments.Columns["id_doco"], 15, "DocId", 60);
            FixColumn(dataGridViewAssignedDocuments.Columns["dias_vencido"], 0, "Dias Vencimiento", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["f_documento"], 1, "Fecha Documento", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["f_vencimiento"], 2, "Fecha Vencimiento", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["f_cobro"], 3, "Fecha Cobro", 80);

            FixColumn(dataGridViewAssignedDocuments.Columns["serie_doco"], 4, "Serie", 40);
            FixColumn(dataGridViewAssignedDocuments.Columns["folio_doco"], 5, "Doc #", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["tipo_documento"], 6, "Tipo Doc", 150);

            FixColumn(dataGridViewAssignedDocuments.Columns["cd_cliente"], 7, "# Cliente", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["nombre_cliente"], 8, "Nombre del Cliente", 180);
            FixColumn(dataGridViewAssignedDocuments.Columns["ruta"], 9, "Ruta", 80);

            FixColumn(dataGridViewAssignedDocuments.Columns["tipo_cobro"], 10, "Tipo Cobro", 150);
            FixColumn(dataGridViewAssignedDocuments.Columns["facturado"], 11, "Total Facturado", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["saldo"], 12, "Saldo", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["moneda"], 13, "Moneda", 80);
            FixColumn(dataGridViewAssignedDocuments.Columns["observaciones"], 14, "Observaciones", 150);


            dataGridViewAssignedDocuments.Columns["f_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dataGridViewAssignedDocuments.Columns["tipo_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dataGridViewAssignedDocuments.Columns["observaciones"].DefaultCellStyle.BackColor = Color.Beige;

            dataGridViewAssignedDocuments.Columns["facturado"].DefaultCellStyle.Format = "c";
            dataGridViewAssignedDocuments.Columns["saldo"].DefaultCellStyle.Format = "c";
        }

        private void FixColumn(DataGridViewColumn column, int displayedIndex, string HeaderText, int width)
        {
            column.DisplayIndex = displayedIndex;
            column.HeaderText = HeaderText;
            column.Width = width;
        }

        private void refreshCollectorsGrid()
        {
            dataGridViewCollectors.DataSource = dbCollector.ReadCollectors();

            dataGridViewCollectors.Columns[0].Width = 50;
            dataGridViewCollectors.Columns[1].Width = 250;
            dataGridViewCollectors.Columns[2].Width = 50;

            dataGridViewCollectors.Columns[0].HeaderText = "ID";
            dataGridViewCollectors.Columns[1].HeaderText = "Nombre";
            dataGridViewCollectors.Columns[2].HeaderText = "Área";

            if (dataGridViewCollectors.Rows.Count > 0)
            {
                dataGridViewCollectors.Sort(dataGridViewCollectors.Columns[1], ListSortDirection.Ascending);
                loadCollector(dataGridViewCollectors.Rows[0]);
            }
            else
                clearCollectorSelection();
        }

        private bool ConfirmNSaveCollector()
        {
            DialogResult confirm = MessageBox.Show("¿Desea guardar los cambios realizados al cobrador seleccionado?", "Cambios a cobrador", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (confirm)
            {
                case DialogResult.Yes:
                    SaveCollector();
                    break;
                case DialogResult.Cancel:
                    return false;
            }

            return true;
        }

        private void SaveCollector()
        {
            if (!ValidateCollector()) return;

            if (labelID.Text.Equals(NEW_CAPTION))
                AddCollector();
            else
                UpdateCollector();

            collectorDirty = false;
        }

        private void RemoveCollector()
        {
            int selectedId = 0;
            bool idSelected = int.TryParse(labelID.Text, out selectedId);
            if (!idSelected)
            {
                MessageBox.Show("Seleccione un cobrador de la lista para intentar eliminarlo", "Identificador Interno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                refreshCollectorsGrid();
                return;
            }

            string question = String.Format("¿Está usted seguro de eliminar el cobrador '{0}' de la base de datos?", textBoxNombre.Text);
            DialogResult confirm = MessageBox.Show(question, "Borrar cobrador", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;


            dbCollector.RemoveCollector(selectedId);
            collectorDirty = false;
            refreshCollectorsGrid();

        }

        private bool ValidateCollector()
        {
            if (String.Empty.Equals(labelID.Text))
            {
                MessageBox.Show("Seleccione un cobrador de la lista o seleccione [Nuevo Cobrador] para agregar un cobrador nuevo", "Identificador Interno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                clearCollectorSelection();
                return false;
            }

            if (String.Empty.Equals(comboBoxArea.Text))
            {
                MessageBox.Show("Seleccione el área a la cual pertenece el cobrador", "Local o Foráneo?", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxArea.Focus();
                return false;
            }

            if (String.Empty.Equals(textBoxNombre.Text.Trim()))
            {
                MessageBox.Show("El cobrador debe tener un nombre", "Nombre del cobrador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxNombre.Focus();
                return false;
            }

            return true;
        }

        private void UpdateCollector()
        {
            dbCollector.UpdateCollector(int.Parse(labelID.Text), textBoxNombre.Text, comboBoxArea.Text.Equals(ES_LOCAL_CAPTION));
            collectorDirty = false;
            refreshCollectorsGrid();
        }

        private void AddCollector()
        {
            dbCollector.AddCollector(textBoxNombre.Text, comboBoxArea.Text.Equals(ES_LOCAL_CAPTION));
            collectorDirty = false;
            refreshCollectorsGrid();
        }

        private void clearCollectorSelection()
        {
            labelID.Text = "";
            textBoxNombre.Text = "";
            comboBoxArea.Text = String.Empty;
            textBoxNombre.Focus();
            collectorDirty = false;
        }

        
    }
}
