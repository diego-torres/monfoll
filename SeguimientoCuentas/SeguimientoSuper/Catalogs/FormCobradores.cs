using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using SeguimientoSuper.Properties;
using SeguimientoSuper.Collectable;
using SeguimientoSuper.Collectable.PostgresImpl;

namespace SeguimientoSuper.Catalogs
{
    public partial class FormCobradores : Form
    {
        private bool collectorDirty = false;
        private bool noteDirty = false;
        private bool fireEvents = true;
        
        private Collector dbCollector = new Collector();

        private const string NEW_CAPTION = "[Nuevo]";
        private const string ES_LOCAL_CAPTION = "Local";

        public FormCobradores()
        {
            InitializeComponent();
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
            collectorDirty = false;
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
