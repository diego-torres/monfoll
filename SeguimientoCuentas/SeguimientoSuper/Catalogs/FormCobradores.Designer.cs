namespace SeguimientoSuper.Catalogs
{
    partial class FormCobradores
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCobradores));
            this.dataGridViewCollectors = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemoveCollector = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxArea = new System.Windows.Forms.ComboBox();
            this.textBoxNombre = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labelID = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labelNoteID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSaveNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemoveNote = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndoNote = new System.Windows.Forms.ToolStripButton();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.dataGridViewNotes = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCollectors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNotes)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewCollectors
            // 
            this.dataGridViewCollectors.AllowUserToAddRows = false;
            this.dataGridViewCollectors.AllowUserToDeleteRows = false;
            this.dataGridViewCollectors.AllowUserToOrderColumns = true;
            this.dataGridViewCollectors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCollectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCollectors.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewCollectors.MultiSelect = false;
            this.dataGridViewCollectors.Name = "dataGridViewCollectors";
            this.dataGridViewCollectors.ReadOnly = true;
            this.dataGridViewCollectors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCollectors.Size = new System.Drawing.Size(370, 566);
            this.dataGridViewCollectors.TabIndex = 0;
            this.dataGridViewCollectors.SelectionChanged += new System.EventHandler(this.dataGridViewCollectors_SelectionChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewCollectors);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(792, 566);
            this.splitContainer1.SplitterDistance = 370;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(418, 566);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.comboBoxArea);
            this.tabPage1.Controls.Add(this.textBoxNombre);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.labelID);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(410, 537);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSave,
            this.toolStripButtonAdd,
            this.toolStripSeparator1,
            this.toolStripButtonRemoveCollector,
            this.toolStripButtonUndo});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(404, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Grabar";
            this.toolStripButtonSave.ToolTipText = "Grabar";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonAdd
            // 
            this.toolStripButtonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAdd.Image")));
            this.toolStripButtonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAdd.Name = "toolStripButtonAdd";
            this.toolStripButtonAdd.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAdd.Text = "Agregar";
            this.toolStripButtonAdd.ToolTipText = "Agregar Cobrador";
            this.toolStripButtonAdd.Click += new System.EventHandler(this.toolStripButtonAdd_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemoveCollector
            // 
            this.toolStripButtonRemoveCollector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveCollector.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemoveCollector.Image")));
            this.toolStripButtonRemoveCollector.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveCollector.Name = "toolStripButtonRemoveCollector";
            this.toolStripButtonRemoveCollector.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveCollector.Text = "Borrar Cobrador";
            this.toolStripButtonRemoveCollector.Click += new System.EventHandler(this.toolStripButtonRemoveCollector_Click);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndo.Text = "Revertir Cambios";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Área:";
            // 
            // comboBoxArea
            // 
            this.comboBoxArea.AutoCompleteCustomSource.AddRange(new string[] {
            "Local",
            "Foráneo"});
            this.comboBoxArea.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxArea.FormattingEnabled = true;
            this.comboBoxArea.Items.AddRange(new object[] {
            "Local",
            "Foráneo"});
            this.comboBoxArea.Location = new System.Drawing.Point(93, 115);
            this.comboBoxArea.Name = "comboBoxArea";
            this.comboBoxArea.Size = new System.Drawing.Size(142, 24);
            this.comboBoxArea.TabIndex = 6;
            this.comboBoxArea.SelectedIndexChanged += new System.EventHandler(this.collectorControls_TextChanged);
            // 
            // textBoxNombre
            // 
            this.textBoxNombre.Location = new System.Drawing.Point(93, 87);
            this.textBoxNombre.MaxLength = 150;
            this.textBoxNombre.Name = "textBoxNombre";
            this.textBoxNombre.Size = new System.Drawing.Size(259, 22);
            this.textBoxNombre.TabIndex = 4;
            this.textBoxNombre.TextChanged += new System.EventHandler(this.collectorControls_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Nombre:";
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Location = new System.Drawing.Point(93, 68);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(0, 16);
            this.labelID.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "ID Interno:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(410, 537);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Notas";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.labelNoteID);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.toolStrip2);
            this.splitContainer2.Panel1.Controls.Add(this.textBoxComment);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridViewNotes);
            this.splitContainer2.Size = new System.Drawing.Size(404, 531);
            this.splitContainer2.SplitterDistance = 188;
            this.splitContainer2.TabIndex = 0;
            // 
            // labelNoteID
            // 
            this.labelNoteID.AutoSize = true;
            this.labelNoteID.Location = new System.Drawing.Point(321, 63);
            this.labelNoteID.Name = "labelNoteID";
            this.labelNoteID.Size = new System.Drawing.Size(0, 16);
            this.labelNoteID.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "ID Interno";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSaveNote,
            this.toolStripButtonAddNote,
            this.toolStripSeparator2,
            this.toolStripButtonRemoveNote,
            this.toolStripButtonUndoNote});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(404, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonSaveNote
            // 
            this.toolStripButtonSaveNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveNote.Image")));
            this.toolStripButtonSaveNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveNote.Name = "toolStripButtonSaveNote";
            this.toolStripButtonSaveNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveNote.Text = "Grabar";
            this.toolStripButtonSaveNote.ToolTipText = "Grabar";
            this.toolStripButtonSaveNote.Click += new System.EventHandler(this.toolStripButtonSaveNote_Click);
            // 
            // toolStripButtonAddNote
            // 
            this.toolStripButtonAddNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddNote.Image")));
            this.toolStripButtonAddNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddNote.Name = "toolStripButtonAddNote";
            this.toolStripButtonAddNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddNote.Text = "Nueva Nota";
            this.toolStripButtonAddNote.Click += new System.EventHandler(this.toolStripButtonAddNote_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemoveNote
            // 
            this.toolStripButtonRemoveNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemoveNote.Image")));
            this.toolStripButtonRemoveNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveNote.Name = "toolStripButtonRemoveNote";
            this.toolStripButtonRemoveNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveNote.Text = "Borrar";
            this.toolStripButtonRemoveNote.Click += new System.EventHandler(this.toolStripButtonRemoveNote_Click);
            // 
            // toolStripButtonUndoNote
            // 
            this.toolStripButtonUndoNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndoNote.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndoNote.Image")));
            this.toolStripButtonUndoNote.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndoNote.Name = "toolStripButtonUndoNote";
            this.toolStripButtonUndoNote.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndoNote.Text = "Revertir Cambios";
            this.toolStripButtonUndoNote.Click += new System.EventHandler(this.toolStripButtonUndoNote_Click);
            // 
            // textBoxComment
            // 
            this.textBoxComment.Location = new System.Drawing.Point(3, 28);
            this.textBoxComment.MaxLength = 250;
            this.textBoxComment.Multiline = true;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(311, 116);
            this.textBoxComment.TabIndex = 1;
            // 
            // dataGridViewNotes
            // 
            this.dataGridViewNotes.AllowUserToAddRows = false;
            this.dataGridViewNotes.AllowUserToDeleteRows = false;
            this.dataGridViewNotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewNotes.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewNotes.Name = "dataGridViewNotes";
            this.dataGridViewNotes.ReadOnly = true;
            this.dataGridViewNotes.Size = new System.Drawing.Size(404, 339);
            this.dataGridViewNotes.TabIndex = 0;
            this.dataGridViewNotes.SelectionChanged += new System.EventHandler(this.dataGridViewNotes_SelectionChanged);
            // 
            // FormCobradores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCobradores";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Catálogo de Cobradores";
            this.Load += new System.EventHandler(this.FormCobradoresGrid_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCollectors)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNotes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewCollectors;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxArea;
        private System.Windows.Forms.TextBox textBoxNombre;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonAdd;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.DataGridView dataGridViewNotes;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveCollector;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.Label labelNoteID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveNote;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddNote;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveNote;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndoNote;
    }
}