namespace SeguimientoCobrador.Process
{
    partial class FormProcess
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcess));
            this.dataGridViewMaster = new System.Windows.Forms.DataGridView();
            this.contextMenuStripAccountsGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.avoidValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripMaster = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFilterMaster = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControlProcess = new System.Windows.Forms.TabControl();
            this.tabPageMaster = new System.Windows.Forms.TabPage();
            this.toolStripMaster = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPrintMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSetDateMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSetObservationsMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddFollowUpMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemoveFilterMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefreshMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSaveFilterMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenFilterMaster = new System.Windows.Forms.ToolStripButton();
            this.tabPageAttended = new System.Windows.Forms.TabPage();
            this.dataGridViewAttended = new System.Windows.Forms.DataGridView();
            this.toolStripAttended = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPrintAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSetCollectDateAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSetObservationsAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAddFollowUpAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemoveFilterAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefreshAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSaveFilterAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOpenFilterAttended = new System.Windows.Forms.ToolStripButton();
            this.statusStripAttended = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFilterAttended = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTotalMaster = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTotalAttended = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaster)).BeginInit();
            this.contextMenuStripAccountsGrid.SuspendLayout();
            this.statusStripMaster.SuspendLayout();
            this.tabControlProcess.SuspendLayout();
            this.tabPageMaster.SuspendLayout();
            this.toolStripMaster.SuspendLayout();
            this.tabPageAttended.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttended)).BeginInit();
            this.toolStripAttended.SuspendLayout();
            this.statusStripAttended.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewMaster
            // 
            this.dataGridViewMaster.AllowUserToAddRows = false;
            this.dataGridViewMaster.AllowUserToDeleteRows = false;
            this.dataGridViewMaster.AllowUserToOrderColumns = true;
            this.dataGridViewMaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMaster.ContextMenuStrip = this.contextMenuStripAccountsGrid;
            this.dataGridViewMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMaster.Location = new System.Drawing.Point(3, 28);
            this.dataGridViewMaster.Name = "dataGridViewMaster";
            this.dataGridViewMaster.ReadOnly = true;
            this.dataGridViewMaster.Size = new System.Drawing.Size(799, 384);
            this.dataGridViewMaster.TabIndex = 0;
            this.dataGridViewMaster.SelectionChanged += new System.EventHandler(this.dataGridViewMaster_SelectionChanged);
            this.dataGridViewMaster.DoubleClick += new System.EventHandler(this.dataGridViewMaster_DoubleClick);
            this.dataGridViewMaster.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // contextMenuStripAccountsGrid
            // 
            this.contextMenuStripAccountsGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyFilterToolStripMenuItem,
            this.avoidValueToolStripMenuItem});
            this.contextMenuStripAccountsGrid.Name = "contextMenuStripAccountsGrid";
            this.contextMenuStripAccountsGrid.Size = new System.Drawing.Size(185, 48);
            // 
            // applyFilterToolStripMenuItem
            // 
            this.applyFilterToolStripMenuItem.Name = "applyFilterToolStripMenuItem";
            this.applyFilterToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.applyFilterToolStripMenuItem.Text = "Filtrar con este valor";
            this.applyFilterToolStripMenuItem.Click += new System.EventHandler(this.applyFilterToolStripMenuItem_Click);
            // 
            // avoidValueToolStripMenuItem
            // 
            this.avoidValueToolStripMenuItem.Name = "avoidValueToolStripMenuItem";
            this.avoidValueToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.avoidValueToolStripMenuItem.Text = "Omitir este valor";
            this.avoidValueToolStripMenuItem.Click += new System.EventHandler(this.avoidValueToolStripMenuItem_Click);
            // 
            // statusStripMaster
            // 
            this.statusStripMaster.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFilterMaster,
            this.toolStripStatusLabelTotalMaster});
            this.statusStripMaster.Location = new System.Drawing.Point(3, 412);
            this.statusStripMaster.Name = "statusStripMaster";
            this.statusStripMaster.Size = new System.Drawing.Size(799, 22);
            this.statusStripMaster.TabIndex = 1;
            // 
            // toolStripStatusFilterMaster
            // 
            this.toolStripStatusFilterMaster.Name = "toolStripStatusFilterMaster";
            this.toolStripStatusFilterMaster.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusFilterMaster.Text = "FILTRO:";
            // 
            // tabControlProcess
            // 
            this.tabControlProcess.Controls.Add(this.tabPageMaster);
            this.tabControlProcess.Controls.Add(this.tabPageAttended);
            this.tabControlProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProcess.Location = new System.Drawing.Point(0, 0);
            this.tabControlProcess.Name = "tabControlProcess";
            this.tabControlProcess.SelectedIndex = 0;
            this.tabControlProcess.Size = new System.Drawing.Size(813, 463);
            this.tabControlProcess.TabIndex = 2;
            // 
            // tabPageMaster
            // 
            this.tabPageMaster.Controls.Add(this.dataGridViewMaster);
            this.tabPageMaster.Controls.Add(this.toolStripMaster);
            this.tabPageMaster.Controls.Add(this.statusStripMaster);
            this.tabPageMaster.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaster.Name = "tabPageMaster";
            this.tabPageMaster.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaster.Size = new System.Drawing.Size(805, 437);
            this.tabPageMaster.TabIndex = 1;
            this.tabPageMaster.Text = "Hoja Maestra";
            this.tabPageMaster.UseVisualStyleBackColor = true;
            // 
            // toolStripMaster
            // 
            this.toolStripMaster.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrintMaster,
            this.toolStripSeparator1,
            this.toolStripButtonSetDateMaster,
            this.toolStripButtonSetObservationsMaster,
            this.toolStripButtonAddFollowUpMaster,
            this.toolStripSeparator3,
            this.toolStripButtonRemoveFilterMaster,
            this.toolStripButtonRefreshMaster,
            this.toolStripSeparator4,
            this.toolStripButtonSaveFilterMaster,
            this.toolStripButtonOpenFilterMaster});
            this.toolStripMaster.Location = new System.Drawing.Point(3, 3);
            this.toolStripMaster.Name = "toolStripMaster";
            this.toolStripMaster.Size = new System.Drawing.Size(799, 25);
            this.toolStripMaster.TabIndex = 2;
            this.toolStripMaster.Text = "toolStrip1";
            // 
            // toolStripButtonPrintMaster
            // 
            this.toolStripButtonPrintMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrintMaster.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPrintMaster.Image")));
            this.toolStripButtonPrintMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrintMaster.Name = "toolStripButtonPrintMaster";
            this.toolStripButtonPrintMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrintMaster.Text = "Imprimir";
            this.toolStripButtonPrintMaster.Click += new System.EventHandler(this.toolStripButtonPrintMaster_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSetDateMaster
            // 
            this.toolStripButtonSetDateMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetDateMaster.Image = global::SeguimientoCobrador.Properties.Resources.calendar_icon;
            this.toolStripButtonSetDateMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetDateMaster.Name = "toolStripButtonSetDateMaster";
            this.toolStripButtonSetDateMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSetDateMaster.Text = "Asignar Fecha de Cobro";
            this.toolStripButtonSetDateMaster.Click += new System.EventHandler(this.toolStripButtonSetDateMaster_Click);
            // 
            // toolStripButtonSetObservationsMaster
            // 
            this.toolStripButtonSetObservationsMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetObservationsMaster.Image = global::SeguimientoCobrador.Properties.Resources.Notes_icon;
            this.toolStripButtonSetObservationsMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetObservationsMaster.Name = "toolStripButtonSetObservationsMaster";
            this.toolStripButtonSetObservationsMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSetObservationsMaster.Text = "Asignar Observaciones";
            this.toolStripButtonSetObservationsMaster.Click += new System.EventHandler(this.toolStripButtonSetObservationsMaster_Click);
            // 
            // toolStripButtonAddFollowUpMaster
            // 
            this.toolStripButtonAddFollowUpMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddFollowUpMaster.Image = global::SeguimientoCobrador.Properties.Resources.Actions_document_edit_icon;
            this.toolStripButtonAddFollowUpMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddFollowUpMaster.Name = "toolStripButtonAddFollowUpMaster";
            this.toolStripButtonAddFollowUpMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddFollowUpMaster.Text = "Agregar Seguimiento";
            this.toolStripButtonAddFollowUpMaster.Click += new System.EventHandler(this.toolStripButtonAddFollowUpMaster_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemoveFilterMaster
            // 
            this.toolStripButtonRemoveFilterMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveFilterMaster.Image = global::SeguimientoCobrador.Properties.Resources.undo_icon;
            this.toolStripButtonRemoveFilterMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveFilterMaster.Name = "toolStripButtonRemoveFilterMaster";
            this.toolStripButtonRemoveFilterMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveFilterMaster.Text = "Quitar Filtro";
            this.toolStripButtonRemoveFilterMaster.Click += new System.EventHandler(this.toolStripButtonRemoveFilterMaster_Click);
            // 
            // toolStripButtonRefreshMaster
            // 
            this.toolStripButtonRefreshMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefreshMaster.Image = global::SeguimientoCobrador.Properties.Resources.refresh_icon;
            this.toolStripButtonRefreshMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefreshMaster.Name = "toolStripButtonRefreshMaster";
            this.toolStripButtonRefreshMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefreshMaster.Text = "Actualizar";
            this.toolStripButtonRefreshMaster.Click += new System.EventHandler(this.toolStripButtonRefreshMaster_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSaveFilterMaster
            // 
            this.toolStripButtonSaveFilterMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveFilterMaster.Image = global::SeguimientoCobrador.Properties.Resources.Save_icon;
            this.toolStripButtonSaveFilterMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveFilterMaster.Name = "toolStripButtonSaveFilterMaster";
            this.toolStripButtonSaveFilterMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveFilterMaster.Text = "Grabar Filtro";
            this.toolStripButtonSaveFilterMaster.Click += new System.EventHandler(this.toolStripButtonSaveFilterMaster_Click);
            // 
            // toolStripButtonOpenFilterMaster
            // 
            this.toolStripButtonOpenFilterMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenFilterMaster.Image = global::SeguimientoCobrador.Properties.Resources.open_file_icon;
            this.toolStripButtonOpenFilterMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpenFilterMaster.Name = "toolStripButtonOpenFilterMaster";
            this.toolStripButtonOpenFilterMaster.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpenFilterMaster.Text = "Abrir Filtro";
            this.toolStripButtonOpenFilterMaster.Click += new System.EventHandler(this.toolStripButtonOpenFilterMaster_Click);
            // 
            // tabPageAttended
            // 
            this.tabPageAttended.Controls.Add(this.dataGridViewAttended);
            this.tabPageAttended.Controls.Add(this.toolStripAttended);
            this.tabPageAttended.Controls.Add(this.statusStripAttended);
            this.tabPageAttended.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttended.Name = "tabPageAttended";
            this.tabPageAttended.Size = new System.Drawing.Size(805, 437);
            this.tabPageAttended.TabIndex = 2;
            this.tabPageAttended.Text = "Atendidos";
            this.tabPageAttended.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAttended
            // 
            this.dataGridViewAttended.AllowUserToAddRows = false;
            this.dataGridViewAttended.AllowUserToDeleteRows = false;
            this.dataGridViewAttended.AllowUserToOrderColumns = true;
            this.dataGridViewAttended.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAttended.ContextMenuStrip = this.contextMenuStripAccountsGrid;
            this.dataGridViewAttended.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAttended.Location = new System.Drawing.Point(0, 25);
            this.dataGridViewAttended.Name = "dataGridViewAttended";
            this.dataGridViewAttended.ReadOnly = true;
            this.dataGridViewAttended.Size = new System.Drawing.Size(805, 390);
            this.dataGridViewAttended.TabIndex = 0;
            this.dataGridViewAttended.SelectionChanged += new System.EventHandler(this.dataGridViewAttended_SelectionChanged);
            this.dataGridViewAttended.DoubleClick += new System.EventHandler(this.dataGridViewAttended_DoubleClick);
            this.dataGridViewAttended.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // toolStripAttended
            // 
            this.toolStripAttended.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrintAttended,
            this.toolStripSeparator9,
            this.toolStripButtonSetCollectDateAttended,
            this.toolStripButtonSetObservationsAttended,
            this.toolStripButtonAddFollowUpAttended,
            this.toolStripSeparator11,
            this.toolStripButtonRemoveFilterAttended,
            this.toolStripButtonRefreshAttended,
            this.toolStripSeparator12,
            this.toolStripButtonSaveFilterAttended,
            this.toolStripButtonOpenFilterAttended});
            this.toolStripAttended.Location = new System.Drawing.Point(0, 0);
            this.toolStripAttended.Name = "toolStripAttended";
            this.toolStripAttended.Size = new System.Drawing.Size(805, 25);
            this.toolStripAttended.TabIndex = 2;
            // 
            // toolStripButtonPrintAttended
            // 
            this.toolStripButtonPrintAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrintAttended.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPrintAttended.Image")));
            this.toolStripButtonPrintAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrintAttended.Name = "toolStripButtonPrintAttended";
            this.toolStripButtonPrintAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrintAttended.Text = "Imprimir";
            this.toolStripButtonPrintAttended.Click += new System.EventHandler(this.toolStripButtonPrintAttended_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSetCollectDateAttended
            // 
            this.toolStripButtonSetCollectDateAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetCollectDateAttended.Image = global::SeguimientoCobrador.Properties.Resources.calendar_icon;
            this.toolStripButtonSetCollectDateAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetCollectDateAttended.Name = "toolStripButtonSetCollectDateAttended";
            this.toolStripButtonSetCollectDateAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSetCollectDateAttended.Text = "Asignar fecha de cobro";
            this.toolStripButtonSetCollectDateAttended.Click += new System.EventHandler(this.toolStripButtonSetCollectDateAttended_Click);
            // 
            // toolStripButtonSetObservationsAttended
            // 
            this.toolStripButtonSetObservationsAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetObservationsAttended.Image = global::SeguimientoCobrador.Properties.Resources.Notes_icon;
            this.toolStripButtonSetObservationsAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetObservationsAttended.Name = "toolStripButtonSetObservationsAttended";
            this.toolStripButtonSetObservationsAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSetObservationsAttended.Text = "Capturar observaciones";
            this.toolStripButtonSetObservationsAttended.Click += new System.EventHandler(this.toolStripButtonSetObservationsAttended_Click);
            // 
            // toolStripButtonAddFollowUpAttended
            // 
            this.toolStripButtonAddFollowUpAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddFollowUpAttended.Image = global::SeguimientoCobrador.Properties.Resources.Actions_document_edit_icon;
            this.toolStripButtonAddFollowUpAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddFollowUpAttended.Name = "toolStripButtonAddFollowUpAttended";
            this.toolStripButtonAddFollowUpAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddFollowUpAttended.Text = "Agregar Seguimiento";
            this.toolStripButtonAddFollowUpAttended.Click += new System.EventHandler(this.toolStripButtonAddFollowUpAttended_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemoveFilterAttended
            // 
            this.toolStripButtonRemoveFilterAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveFilterAttended.Image = global::SeguimientoCobrador.Properties.Resources.undo_icon;
            this.toolStripButtonRemoveFilterAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveFilterAttended.Name = "toolStripButtonRemoveFilterAttended";
            this.toolStripButtonRemoveFilterAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveFilterAttended.Text = "Quitar Filtro";
            this.toolStripButtonRemoveFilterAttended.Click += new System.EventHandler(this.toolStripButtonRemoveFilterAttended_Click);
            // 
            // toolStripButtonRefreshAttended
            // 
            this.toolStripButtonRefreshAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefreshAttended.Image = global::SeguimientoCobrador.Properties.Resources.refresh_icon;
            this.toolStripButtonRefreshAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefreshAttended.Name = "toolStripButtonRefreshAttended";
            this.toolStripButtonRefreshAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefreshAttended.Text = "Actualizar";
            this.toolStripButtonRefreshAttended.Click += new System.EventHandler(this.toolStripButtonRefreshAttended_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSaveFilterAttended
            // 
            this.toolStripButtonSaveFilterAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveFilterAttended.Image = global::SeguimientoCobrador.Properties.Resources.Save_icon;
            this.toolStripButtonSaveFilterAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveFilterAttended.Name = "toolStripButtonSaveFilterAttended";
            this.toolStripButtonSaveFilterAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveFilterAttended.Text = "Grabar Filtro";
            this.toolStripButtonSaveFilterAttended.Click += new System.EventHandler(this.toolStripButtonSaveFilterAttended_Click);
            // 
            // toolStripButtonOpenFilterAttended
            // 
            this.toolStripButtonOpenFilterAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpenFilterAttended.Image = global::SeguimientoCobrador.Properties.Resources.open_file_icon;
            this.toolStripButtonOpenFilterAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpenFilterAttended.Name = "toolStripButtonOpenFilterAttended";
            this.toolStripButtonOpenFilterAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpenFilterAttended.Text = "Abrir Filtro";
            this.toolStripButtonOpenFilterAttended.Click += new System.EventHandler(this.toolStripButtonOpenFilterAttended_Click);
            // 
            // statusStripAttended
            // 
            this.statusStripAttended.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFilterAttended,
            this.toolStripStatusLabelTotalAttended});
            this.statusStripAttended.Location = new System.Drawing.Point(0, 415);
            this.statusStripAttended.Name = "statusStripAttended";
            this.statusStripAttended.Size = new System.Drawing.Size(805, 22);
            this.statusStripAttended.TabIndex = 1;
            this.statusStripAttended.Text = "statusStrip1";
            // 
            // toolStripStatusFilterAttended
            // 
            this.toolStripStatusFilterAttended.Name = "toolStripStatusFilterAttended";
            this.toolStripStatusFilterAttended.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusFilterAttended.Text = "FILTRO:";
            // 
            // toolStripStatusLabelTotalMaster
            // 
            this.toolStripStatusLabelTotalMaster.Name = "toolStripStatusLabelTotalMaster";
            this.toolStripStatusLabelTotalMaster.Size = new System.Drawing.Size(737, 17);
            this.toolStripStatusLabelTotalMaster.Spring = true;
            this.toolStripStatusLabelTotalMaster.Text = "Total:";
            this.toolStripStatusLabelTotalMaster.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStripStatusLabelTotalAttended
            // 
            this.toolStripStatusLabelTotalAttended.Name = "toolStripStatusLabelTotalAttended";
            this.toolStripStatusLabelTotalAttended.Size = new System.Drawing.Size(743, 17);
            this.toolStripStatusLabelTotalAttended.Spring = true;
            this.toolStripStatusLabelTotalAttended.Text = "Total:";
            this.toolStripStatusLabelTotalAttended.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 463);
            this.Controls.Add(this.tabControlProcess);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormProcess";
            this.Text = "Proceso";
            this.Load += new System.EventHandler(this.FormProcess_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMaster)).EndInit();
            this.contextMenuStripAccountsGrid.ResumeLayout(false);
            this.statusStripMaster.ResumeLayout(false);
            this.statusStripMaster.PerformLayout();
            this.tabControlProcess.ResumeLayout(false);
            this.tabPageMaster.ResumeLayout(false);
            this.tabPageMaster.PerformLayout();
            this.toolStripMaster.ResumeLayout(false);
            this.toolStripMaster.PerformLayout();
            this.tabPageAttended.ResumeLayout(false);
            this.tabPageAttended.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttended)).EndInit();
            this.toolStripAttended.ResumeLayout(false);
            this.toolStripAttended.PerformLayout();
            this.statusStripAttended.ResumeLayout(false);
            this.statusStripAttended.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripMaster;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFilterMaster;
        private System.Windows.Forms.DataGridView dataGridViewMaster;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAccountsGrid;
        private System.Windows.Forms.ToolStripMenuItem applyFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem avoidValueToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlProcess;
        private System.Windows.Forms.TabPage tabPageMaster;
        private System.Windows.Forms.TabPage tabPageAttended;
        private System.Windows.Forms.ToolStrip toolStripMaster;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintMaster;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetDateMaster;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetObservationsMaster;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFilterMaster;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefreshMaster;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveFilterMaster;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenFilterMaster;
        private System.Windows.Forms.ToolStrip toolStripAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintAttended;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetCollectDateAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetObservationsAttended;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFilterAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefreshAttended;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveFilterAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpenFilterAttended;
        private System.Windows.Forms.StatusStrip statusStripAttended;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFilterAttended;
        private System.Windows.Forms.DataGridView dataGridViewAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFollowUpMaster;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddFollowUpAttended;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTotalMaster;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTotalAttended;
    }
}