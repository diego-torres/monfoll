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
            this.tabControlProcess = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainerAssigned = new System.Windows.Forms.SplitContainer();
            this.dataGridViewAssignedAccounts = new System.Windows.Forms.DataGridView();
            this.contextMenuStripAccountsGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.avoidValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewAssignedFollowUp = new System.Windows.Forms.DataGridView();
            this.statusStripAssigned = new System.Windows.Forms.StatusStrip();
            this.assignedToolStripStatusFilter = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripAssigned = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonPrintAssigned = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.AssignedToolStripButtonSetCollectDate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRemoveFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.tabControlProcess.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAssigned)).BeginInit();
            this.splitContainerAssigned.Panel1.SuspendLayout();
            this.splitContainerAssigned.Panel2.SuspendLayout();
            this.splitContainerAssigned.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedAccounts)).BeginInit();
            this.contextMenuStripAccountsGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedFollowUp)).BeginInit();
            this.statusStripAssigned.SuspendLayout();
            this.toolStripAssigned.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlProcess
            // 
            this.tabControlProcess.Controls.Add(this.tabPage2);
            this.tabControlProcess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProcess.Location = new System.Drawing.Point(0, 0);
            this.tabControlProcess.Name = "tabControlProcess";
            this.tabControlProcess.SelectedIndex = 0;
            this.tabControlProcess.Size = new System.Drawing.Size(813, 463);
            this.tabControlProcess.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainerAssigned);
            this.tabPage2.Controls.Add(this.statusStripAssigned);
            this.tabPage2.Controls.Add(this.toolStripAssigned);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(805, 437);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Asignados";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.splitContainerAssigned.Size = new System.Drawing.Size(799, 384);
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
            this.dataGridViewAssignedAccounts.Size = new System.Drawing.Size(452, 384);
            this.dataGridViewAssignedAccounts.TabIndex = 0;
            this.dataGridViewAssignedAccounts.SelectionChanged += new System.EventHandler(this.dataGridViewAssignedAccounts_SelectionChanged);
            this.dataGridViewAssignedAccounts.DoubleClick += new System.EventHandler(this.dataGridViewAssignedAccounts_DoubleClick);
            this.dataGridViewAssignedAccounts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
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
            this.dataGridViewAssignedFollowUp.Size = new System.Drawing.Size(343, 384);
            this.dataGridViewAssignedFollowUp.TabIndex = 0;
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
            // toolStripAssigned
            // 
            this.toolStripAssigned.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPrintAssigned,
            this.toolStripSeparator4,
            this.AssignedToolStripButtonSetCollectDate,
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
            this.tabControlProcess.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainerAssigned.Panel1.ResumeLayout(false);
            this.splitContainerAssigned.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAssigned)).EndInit();
            this.splitContainerAssigned.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedAccounts)).EndInit();
            this.contextMenuStripAccountsGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssignedFollowUp)).EndInit();
            this.statusStripAssigned.ResumeLayout(false);
            this.statusStripAssigned.PerformLayout();
            this.toolStripAssigned.ResumeLayout(false);
            this.toolStripAssigned.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlProcess;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripAccountsGrid;
        private System.Windows.Forms.ToolStripMenuItem applyFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem avoidValueToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerAssigned;
        private System.Windows.Forms.DataGridView dataGridViewAssignedAccounts;
        private System.Windows.Forms.DataGridView dataGridViewAssignedFollowUp;
        private System.Windows.Forms.StatusStrip statusStripAssigned;
        private System.Windows.Forms.ToolStrip toolStripAssigned;
        private System.Windows.Forms.ToolStripStatusLabel assignedToolStripStatusFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrintAssigned;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonRemoveFilter;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripButton AssignedToolStripButtonSetCollectDate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
    }
}