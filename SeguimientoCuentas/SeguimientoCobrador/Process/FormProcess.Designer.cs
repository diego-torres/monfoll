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
            this.contextMenuStripAccountsGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.avoidValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statusStripAssigned = new System.Windows.Forms.StatusStrip();
            this.assignedToolStripStatusFilter = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainerAssigned = new System.Windows.Forms.SplitContainer();
            this.dataGridViewAssignedAccounts = new System.Windows.Forms.DataGridView();
            this.dataGridViewAssignedFollowUp = new System.Windows.Forms.DataGridView();
            this.toolStripAssigned = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPrintAssigned = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.AssignedToolStripButtonSetCollectDate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemoveFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.tabControlProcess = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainerAttended = new System.Windows.Forms.SplitContainer();
            this.dataGridViewAttendedAccounts = new System.Windows.Forms.DataGridView();
            this.dataGridViewAttendedFollowUp = new System.Windows.Forms.DataGridView();
            this.toolStripAttended = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPrintAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSetCollectDateAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemmoveFilterAttended = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUpdateAttended = new System.Windows.Forms.ToolStripButton();
            this.statusStripAttended = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelAttended = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripButtonObservacionesAsignados = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAttendedObservations = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStripAccountsGrid.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.statusStripAssigned.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAssigned)).BeginInit();
            this.splitContainerAssigned.Panel1.SuspendLayout();
            this.splitContainerAssigned.Panel2.SuspendLayout();
            this.splitContainerAssigned.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedFollowUp)).BeginInit();
            this.toolStripAssigned.SuspendLayout();
            this.tabControlProcess.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAttended)).BeginInit();
            this.splitContainerAttended.Panel1.SuspendLayout();
            this.splitContainerAttended.Panel2.SuspendLayout();
            this.splitContainerAttended.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttendedAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttendedFollowUp)).BeginInit();
            this.toolStripAttended.SuspendLayout();
            this.statusStripAttended.SuspendLayout();
            this.SuspendLayout();
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.statusStripAssigned);
            this.tabPage2.Controls.Add(this.splitContainerAssigned);
            this.tabPage2.Controls.Add(this.toolStripAssigned);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(805, 437);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Asignados";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // statusStripAssigned
            // 
            this.statusStripAssigned.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.assignedToolStripStatusFilter});
            this.statusStripAssigned.Location = new System.Drawing.Point(3, 412);
            this.statusStripAssigned.Name = "statusStripAssigned";
            this.statusStripAssigned.Size = new System.Drawing.Size(799, 22);
            this.statusStripAssigned.TabIndex = 1;
            this.statusStripAssigned.Text = "statusStrip2";
            // 
            // assignedToolStripStatusFilter
            // 
            this.assignedToolStripStatusFilter.Name = "assignedToolStripStatusFilter";
            this.assignedToolStripStatusFilter.Size = new System.Drawing.Size(47, 17);
            this.assignedToolStripStatusFilter.Text = "FILTRO:";
            // 
            // splitContainerAssigned
            // 
            this.splitContainerAssigned.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAssigned.Location = new System.Drawing.Point(3, 28);
            this.splitContainerAssigned.Name = "splitContainerAssigned";
            // 
            // splitContainerAssigned.Panel1
            // 
            this.splitContainerAssigned.Panel1.Controls.Add(this.dataGridViewAssignedAccounts);
            // 
            // splitContainerAssigned.Panel2
            // 
            this.splitContainerAssigned.Panel2.Controls.Add(this.dataGridViewAssignedFollowUp);
            this.splitContainerAssigned.Size = new System.Drawing.Size(799, 406);
            this.splitContainerAssigned.SplitterDistance = 452;
            this.splitContainerAssigned.TabIndex = 2;
            // 
            // dataGridViewAssignedAccounts
            // 
            this.dataGridViewAssignedAccounts.AllowUserToAddRows = false;
            this.dataGridViewAssignedAccounts.AllowUserToDeleteRows = false;
            this.dataGridViewAssignedAccounts.AllowUserToOrderColumns = true;
            this.dataGridViewAssignedAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAssignedAccounts.ContextMenuStrip = this.contextMenuStripAccountsGrid;
            this.dataGridViewAssignedAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAssignedAccounts.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAssignedAccounts.Name = "dataGridViewAssignedAccounts";
            this.dataGridViewAssignedAccounts.ReadOnly = true;
            this.dataGridViewAssignedAccounts.Size = new System.Drawing.Size(452, 406);
            this.dataGridViewAssignedAccounts.TabIndex = 0;
            this.dataGridViewAssignedAccounts.SelectionChanged += new System.EventHandler(this.dataGridViewAssignedAccounts_SelectionChanged);
            this.dataGridViewAssignedAccounts.DoubleClick += new System.EventHandler(this.dataGridViewAssignedAccounts_DoubleClick);
            this.dataGridViewAssignedAccounts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // dataGridViewAssignedFollowUp
            // 
            this.dataGridViewAssignedFollowUp.AllowUserToAddRows = false;
            this.dataGridViewAssignedFollowUp.AllowUserToDeleteRows = false;
            this.dataGridViewAssignedFollowUp.AllowUserToOrderColumns = true;
            this.dataGridViewAssignedFollowUp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAssignedFollowUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAssignedFollowUp.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAssignedFollowUp.Name = "dataGridViewAssignedFollowUp";
            this.dataGridViewAssignedFollowUp.ReadOnly = true;
            this.dataGridViewAssignedFollowUp.Size = new System.Drawing.Size(343, 406);
            this.dataGridViewAssignedFollowUp.TabIndex = 0;
            // 
            // toolStripAssigned
            // 
            this.toolStripAssigned.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrintAssigned,
            this.toolStripSeparator4,
            this.AssignedToolStripButtonSetCollectDate,
            this.toolStripButtonObservacionesAsignados,
            this.toolStripSeparator11,
            this.toolStripButtonRemoveFilter,
            this.toolStripButtonRefresh});
            this.toolStripAssigned.Location = new System.Drawing.Point(3, 3);
            this.toolStripAssigned.Name = "toolStripAssigned";
            this.toolStripAssigned.Size = new System.Drawing.Size(799, 25);
            this.toolStripAssigned.TabIndex = 0;
            this.toolStripAssigned.Text = "toolStrip2";
            // 
            // toolStripButtonPrintAssigned
            // 
            this.toolStripButtonPrintAssigned.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPrintAssigned.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPrintAssigned.Image")));
            this.toolStripButtonPrintAssigned.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrintAssigned.Name = "toolStripButtonPrintAssigned";
            this.toolStripButtonPrintAssigned.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPrintAssigned.Text = "Imprimir";
            this.toolStripButtonPrintAssigned.Click += new System.EventHandler(this.toolStripButtonPrintAssigned_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // AssignedToolStripButtonSetCollectDate
            // 
            this.AssignedToolStripButtonSetCollectDate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AssignedToolStripButtonSetCollectDate.Image = ((System.Drawing.Image)(resources.GetObject("AssignedToolStripButtonSetCollectDate.Image")));
            this.AssignedToolStripButtonSetCollectDate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AssignedToolStripButtonSetCollectDate.Name = "AssignedToolStripButtonSetCollectDate";
            this.AssignedToolStripButtonSetCollectDate.Size = new System.Drawing.Size(23, 22);
            this.AssignedToolStripButtonSetCollectDate.Text = "Asignar Fecha de Cobro";
            this.AssignedToolStripButtonSetCollectDate.Click += new System.EventHandler(this.AssignedToolStripButtonSetCollectDate_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemoveFilter
            // 
            this.toolStripButtonRemoveFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemoveFilter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemoveFilter.Image")));
            this.toolStripButtonRemoveFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemoveFilter.Name = "toolStripButtonRemoveFilter";
            this.toolStripButtonRemoveFilter.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemoveFilter.Text = "Quitar Filtro";
            this.toolStripButtonRemoveFilter.Click += new System.EventHandler(this.toolStripButtonRemoveFilter_Click);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "Actualizar";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // tabControlProcess
            // 
            this.tabControlProcess.Controls.Add(this.tabPage2);
            this.tabControlProcess.Controls.Add(this.tabPage1);
            this.tabControlProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProcess.Location = new System.Drawing.Point(0, 0);
            this.tabControlProcess.Name = "tabControlProcess";
            this.tabControlProcess.SelectedIndex = 0;
            this.tabControlProcess.Size = new System.Drawing.Size(813, 463);
            this.tabControlProcess.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainerAttended);
            this.tabPage1.Controls.Add(this.toolStripAttended);
            this.tabPage1.Controls.Add(this.statusStripAttended);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(805, 437);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Atendidos";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainerAttended
            // 
            this.splitContainerAttended.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAttended.Location = new System.Drawing.Point(0, 25);
            this.splitContainerAttended.Name = "splitContainerAttended";
            // 
            // splitContainerAttended.Panel1
            // 
            this.splitContainerAttended.Panel1.Controls.Add(this.dataGridViewAttendedAccounts);
            // 
            // splitContainerAttended.Panel2
            // 
            this.splitContainerAttended.Panel2.Controls.Add(this.dataGridViewAttendedFollowUp);
            this.splitContainerAttended.Size = new System.Drawing.Size(805, 390);
            this.splitContainerAttended.SplitterDistance = 511;
            this.splitContainerAttended.TabIndex = 2;
            // 
            // dataGridViewAttendedAccounts
            // 
            this.dataGridViewAttendedAccounts.AllowUserToAddRows = false;
            this.dataGridViewAttendedAccounts.AllowUserToDeleteRows = false;
            this.dataGridViewAttendedAccounts.AllowUserToOrderColumns = true;
            this.dataGridViewAttendedAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAttendedAccounts.ContextMenuStrip = this.contextMenuStripAccountsGrid;
            this.dataGridViewAttendedAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAttendedAccounts.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAttendedAccounts.Name = "dataGridViewAttendedAccounts";
            this.dataGridViewAttendedAccounts.ReadOnly = true;
            this.dataGridViewAttendedAccounts.Size = new System.Drawing.Size(511, 390);
            this.dataGridViewAttendedAccounts.TabIndex = 0;
            this.dataGridViewAttendedAccounts.SelectionChanged += new System.EventHandler(this.dataGridViewAttendedAccounts_SelectionChanged);
            this.dataGridViewAttendedAccounts.DoubleClick += new System.EventHandler(this.dataGridViewAttendedAccounts_DoubleClick);
            this.dataGridViewAttendedAccounts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // dataGridViewAttendedFollowUp
            // 
            this.dataGridViewAttendedFollowUp.AllowUserToAddRows = false;
            this.dataGridViewAttendedFollowUp.AllowUserToDeleteRows = false;
            this.dataGridViewAttendedFollowUp.AllowUserToOrderColumns = true;
            this.dataGridViewAttendedFollowUp.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAttendedFollowUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAttendedFollowUp.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAttendedFollowUp.Name = "dataGridViewAttendedFollowUp";
            this.dataGridViewAttendedFollowUp.ReadOnly = true;
            this.dataGridViewAttendedFollowUp.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAttendedFollowUp.Size = new System.Drawing.Size(290, 390);
            this.dataGridViewAttendedFollowUp.TabIndex = 0;
            // 
            // toolStripAttended
            // 
            this.toolStripAttended.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrintAttended,
            this.toolStripSeparator1,
            this.toolStripButtonSetCollectDateAttended,
            this.toolStripButtonAttendedObservations,
            this.toolStripSeparator2,
            this.toolStripButtonRemmoveFilterAttended,
            this.toolStripButtonUpdateAttended});
            this.toolStripAttended.Location = new System.Drawing.Point(0, 0);
            this.toolStripAttended.Name = "toolStripAttended";
            this.toolStripAttended.Size = new System.Drawing.Size(805, 25);
            this.toolStripAttended.TabIndex = 1;
            this.toolStripAttended.Text = "toolStrip1";
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSetCollectDateAttended
            // 
            this.toolStripButtonSetCollectDateAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetCollectDateAttended.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSetCollectDateAttended.Image")));
            this.toolStripButtonSetCollectDateAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetCollectDateAttended.Name = "toolStripButtonSetCollectDateAttended";
            this.toolStripButtonSetCollectDateAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSetCollectDateAttended.Text = "Asignar Fecha De Cobro";
            this.toolStripButtonSetCollectDateAttended.Click += new System.EventHandler(this.toolStripButtonSetCollectDateAttended_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRemmoveFilterAttended
            // 
            this.toolStripButtonRemmoveFilterAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRemmoveFilterAttended.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRemmoveFilterAttended.Image")));
            this.toolStripButtonRemmoveFilterAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRemmoveFilterAttended.Name = "toolStripButtonRemmoveFilterAttended";
            this.toolStripButtonRemmoveFilterAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRemmoveFilterAttended.Text = "Quitar Filtro";
            this.toolStripButtonRemmoveFilterAttended.Click += new System.EventHandler(this.toolStripButtonRemmoveFilterAttended_Click);
            // 
            // toolStripButtonUpdateAttended
            // 
            this.toolStripButtonUpdateAttended.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUpdateAttended.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUpdateAttended.Image")));
            this.toolStripButtonUpdateAttended.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUpdateAttended.Name = "toolStripButtonUpdateAttended";
            this.toolStripButtonUpdateAttended.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUpdateAttended.Text = "Actualizar";
            this.toolStripButtonUpdateAttended.Click += new System.EventHandler(this.toolStripButtonUpdateAttended_Click);
            // 
            // statusStripAttended
            // 
            this.statusStripAttended.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelAttended});
            this.statusStripAttended.Location = new System.Drawing.Point(0, 415);
            this.statusStripAttended.Name = "statusStripAttended";
            this.statusStripAttended.Size = new System.Drawing.Size(805, 22);
            this.statusStripAttended.TabIndex = 0;
            this.statusStripAttended.Text = "statusStrip1";
            // 
            // toolStripStatusLabelAttended
            // 
            this.toolStripStatusLabelAttended.Name = "toolStripStatusLabelAttended";
            this.toolStripStatusLabelAttended.Size = new System.Drawing.Size(47, 17);
            this.toolStripStatusLabelAttended.Text = "FILTRO:";
            // 
            // toolStripButtonObservacionesAsignados
            // 
            this.toolStripButtonObservacionesAsignados.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonObservacionesAsignados.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonObservacionesAsignados.Image")));
            this.toolStripButtonObservacionesAsignados.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonObservacionesAsignados.Name = "toolStripButtonObservacionesAsignados";
            this.toolStripButtonObservacionesAsignados.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonObservacionesAsignados.Text = "Capturar Observaciones";
            this.toolStripButtonObservacionesAsignados.Click += new System.EventHandler(this.toolStripButtonObservacionesAsignados_Click);
            // 
            // toolStripButtonAttendedObservations
            // 
            this.toolStripButtonAttendedObservations.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAttendedObservations.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAttendedObservations.Image")));
            this.toolStripButtonAttendedObservations.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAttendedObservations.Name = "toolStripButtonAttendedObservations";
            this.toolStripButtonAttendedObservations.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAttendedObservations.Text = "Capturar Observaciones";
            this.toolStripButtonAttendedObservations.Click += new System.EventHandler(this.toolStripButtonAttendedObservations_Click);
            // 
            // FormProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 463);
            this.Controls.Add(this.tabControlProcess);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormProcess";
            this.Text = "FormProcess";
            this.Load += new System.EventHandler(this.FormProcess_Load);
            this.contextMenuStripAccountsGrid.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.statusStripAssigned.ResumeLayout(false);
            this.statusStripAssigned.PerformLayout();
            this.splitContainerAssigned.Panel1.ResumeLayout(false);
            this.splitContainerAssigned.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAssigned)).EndInit();
            this.splitContainerAssigned.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedFollowUp)).EndInit();
            this.toolStripAssigned.ResumeLayout(false);
            this.toolStripAssigned.PerformLayout();
            this.tabControlProcess.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.splitContainerAttended.Panel1.ResumeLayout(false);
            this.splitContainerAttended.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAttended)).EndInit();
            this.splitContainerAttended.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttendedAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAttendedFollowUp)).EndInit();
            this.toolStripAttended.ResumeLayout(false);
            this.toolStripAttended.PerformLayout();
            this.statusStripAttended.ResumeLayout(false);
            this.statusStripAttended.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripAccountsGrid;
        private System.Windows.Forms.ToolStripMenuItem applyFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem avoidValueToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStripAssigned;
        private System.Windows.Forms.ToolStripStatusLabel assignedToolStripStatusFilter;
        private System.Windows.Forms.SplitContainer splitContainerAssigned;
        private System.Windows.Forms.DataGridView dataGridViewAssignedAccounts;
        private System.Windows.Forms.DataGridView dataGridViewAssignedFollowUp;
        private System.Windows.Forms.ToolStrip toolStripAssigned;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintAssigned;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton AssignedToolStripButtonSetCollectDate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.TabControl tabControlProcess;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStrip toolStripAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintAttended;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetCollectDateAttended;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemmoveFilterAttended;
        private System.Windows.Forms.ToolStripButton toolStripButtonUpdateAttended;
        private System.Windows.Forms.StatusStrip statusStripAttended;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelAttended;
        private System.Windows.Forms.SplitContainer splitContainerAttended;
        private System.Windows.Forms.DataGridView dataGridViewAttendedAccounts;
        private System.Windows.Forms.DataGridView dataGridViewAttendedFollowUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonObservacionesAsignados;
        private System.Windows.Forms.ToolStripButton toolStripButtonAttendedObservations;
    }
}