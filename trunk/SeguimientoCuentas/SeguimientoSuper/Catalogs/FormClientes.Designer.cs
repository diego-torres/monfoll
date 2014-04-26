namespace SeguimientoSuper.Catalogs
{
    partial class FormClientes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClientes));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.dataGridViewCustomers = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.splitContainerGeneralDetails = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.labelDiaPago = new System.Windows.Forms.Label();
            this.labelIDCliente = new System.Windows.Forms.Label();
            this.labelNombre = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelRuta = new System.Windows.Forms.Label();
            this.labelCodigo = new System.Windows.Forms.Label();
            this.splitContainerAccounts = new System.Windows.Forms.SplitContainer();
            this.label6 = new System.Windows.Forms.Label();
            this.dataGridViewPayments = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPageNotes = new System.Windows.Forms.TabPage();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.dataGridViewAccounts = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustomers)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGeneralDetails)).BeginInit();
            this.splitContainerGeneralDetails.Panel1.SuspendLayout();
            this.splitContainerGeneralDetails.Panel2.SuspendLayout();
            this.splitContainerGeneralDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAccounts)).BeginInit();
            this.splitContainerAccounts.Panel1.SuspendLayout();
            this.splitContainerAccounts.Panel2.SuspendLayout();
            this.splitContainerAccounts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPayments)).BeginInit();
            this.tabPageNotes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.dataGridViewCustomers);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerMain.Size = new System.Drawing.Size(931, 566);
            this.splitContainerMain.SplitterDistance = 386;
            this.splitContainerMain.TabIndex = 0;
            // 
            // dataGridViewCustomers
            // 
            this.dataGridViewCustomers.AllowUserToAddRows = false;
            this.dataGridViewCustomers.AllowUserToDeleteRows = false;
            this.dataGridViewCustomers.AllowUserToOrderColumns = true;
            this.dataGridViewCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCustomers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCustomers.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewCustomers.Name = "dataGridViewCustomers";
            this.dataGridViewCustomers.ReadOnly = true;
            this.dataGridViewCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewCustomers.Size = new System.Drawing.Size(386, 566);
            this.dataGridViewCustomers.TabIndex = 0;
            this.dataGridViewCustomers.SelectionChanged += new System.EventHandler(this.dataGridViewCustomers_SelectionChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageNotes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(541, 566);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.toolStrip1);
            this.tabPageGeneral.Controls.Add(this.splitContainerGeneralDetails);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(533, 537);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(527, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Imprimir Cuenta";
            // 
            // splitContainerGeneralDetails
            // 
            this.splitContainerGeneralDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerGeneralDetails.Location = new System.Drawing.Point(3, 3);
            this.splitContainerGeneralDetails.Name = "splitContainerGeneralDetails";
            this.splitContainerGeneralDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerGeneralDetails.Panel1
            // 
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.label1);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.labelDiaPago);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.labelIDCliente);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.labelNombre);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.label5);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.label3);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.label2);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.label4);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.labelRuta);
            this.splitContainerGeneralDetails.Panel1.Controls.Add(this.labelCodigo);
            // 
            // splitContainerGeneralDetails.Panel2
            // 
            this.splitContainerGeneralDetails.Panel2.Controls.Add(this.splitContainerAccounts);
            this.splitContainerGeneralDetails.Size = new System.Drawing.Size(527, 531);
            this.splitContainerGeneralDetails.SplitterDistance = 196;
            this.splitContainerGeneralDetails.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "ID Interno:";
            // 
            // labelDiaPago
            // 
            this.labelDiaPago.AutoSize = true;
            this.labelDiaPago.Location = new System.Drawing.Point(87, 158);
            this.labelDiaPago.Name = "labelDiaPago";
            this.labelDiaPago.Size = new System.Drawing.Size(110, 16);
            this.labelDiaPago.TabIndex = 11;
            this.labelDiaPago.Text = "[DIAS DE PAGO]";
            // 
            // labelIDCliente
            // 
            this.labelIDCliente.AutoSize = true;
            this.labelIDCliente.Location = new System.Drawing.Point(87, 36);
            this.labelIDCliente.Name = "labelIDCliente";
            this.labelIDCliente.Size = new System.Drawing.Size(29, 16);
            this.labelIDCliente.TabIndex = 3;
            this.labelIDCliente.Text = "[ID]";
            // 
            // labelNombre
            // 
            this.labelNombre.AutoSize = true;
            this.labelNombre.Location = new System.Drawing.Point(87, 100);
            this.labelNombre.Name = "labelNombre";
            this.labelNombre.Size = new System.Drawing.Size(75, 16);
            this.labelNombre.TabIndex = 7;
            this.labelNombre.Text = "[NOMBRE]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Día Pago:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Nombre:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Código:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Ruta:";
            // 
            // labelRuta
            // 
            this.labelRuta.AutoSize = true;
            this.labelRuta.Location = new System.Drawing.Point(87, 129);
            this.labelRuta.Name = "labelRuta";
            this.labelRuta.Size = new System.Drawing.Size(54, 16);
            this.labelRuta.TabIndex = 9;
            this.labelRuta.Text = "[RUTA]";
            // 
            // labelCodigo
            // 
            this.labelCodigo.AutoSize = true;
            this.labelCodigo.Location = new System.Drawing.Point(87, 68);
            this.labelCodigo.Name = "labelCodigo";
            this.labelCodigo.Size = new System.Drawing.Size(68, 16);
            this.labelCodigo.TabIndex = 5;
            this.labelCodigo.Text = "[CODIGO]";
            // 
            // splitContainerAccounts
            // 
            this.splitContainerAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAccounts.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAccounts.Name = "splitContainerAccounts";
            // 
            // splitContainerAccounts.Panel1
            // 
            this.splitContainerAccounts.Panel1.Controls.Add(this.dataGridViewAccounts);
            this.splitContainerAccounts.Panel1.Controls.Add(this.label6);
            // 
            // splitContainerAccounts.Panel2
            // 
            this.splitContainerAccounts.Panel2.Controls.Add(this.dataGridViewPayments);
            this.splitContainerAccounts.Panel2.Controls.Add(this.label7);
            this.splitContainerAccounts.Size = new System.Drawing.Size(527, 331);
            this.splitContainerAccounts.SplitterDistance = 268;
            this.splitContainerAccounts.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Location = new System.Drawing.Point(0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(268, 28);
            this.label6.TabIndex = 0;
            this.label6.Text = "Documentos por cobrar";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGridViewPayments
            // 
            this.dataGridViewPayments.AllowUserToAddRows = false;
            this.dataGridViewPayments.AllowUserToDeleteRows = false;
            this.dataGridViewPayments.AllowUserToOrderColumns = true;
            this.dataGridViewPayments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPayments.Location = new System.Drawing.Point(0, 28);
            this.dataGridViewPayments.Name = "dataGridViewPayments";
            this.dataGridViewPayments.ReadOnly = true;
            this.dataGridViewPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewPayments.Size = new System.Drawing.Size(255, 303);
            this.dataGridViewPayments.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label7.Location = new System.Drawing.Point(0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(255, 28);
            this.label7.TabIndex = 1;
            this.label7.Text = "Abonos";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPageNotes
            // 
            this.tabPageNotes.Controls.Add(this.toolStrip3);
            this.tabPageNotes.Location = new System.Drawing.Point(4, 25);
            this.tabPageNotes.Name = "tabPageNotes";
            this.tabPageNotes.Size = new System.Drawing.Size(533, 537);
            this.tabPageNotes.TabIndex = 2;
            this.tabPageNotes.Text = "Notas";
            this.tabPageNotes.UseVisualStyleBackColor = true;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(533, 25);
            this.toolStrip3.TabIndex = 0;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // dataGridViewAccounts
            // 
            this.dataGridViewAccounts.AllowUserToAddRows = false;
            this.dataGridViewAccounts.AllowUserToDeleteRows = false;
            this.dataGridViewAccounts.AllowUserToOrderColumns = true;
            this.dataGridViewAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewAccounts.Location = new System.Drawing.Point(0, 28);
            this.dataGridViewAccounts.MultiSelect = false;
            this.dataGridViewAccounts.Name = "dataGridViewAccounts";
            this.dataGridViewAccounts.ReadOnly = true;
            this.dataGridViewAccounts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAccounts.Size = new System.Drawing.Size(268, 303);
            this.dataGridViewAccounts.TabIndex = 1;
            this.dataGridViewAccounts.SelectionChanged += new System.EventHandler(this.dataGridViewAccounts_SelectionChanged);
            this.dataGridViewAccounts.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridViewAccounts_MouseDoubleClick);
            // 
            // FormClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 566);
            this.Controls.Add(this.splitContainerMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormClientes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Catálogo de Clientes";
            this.Load += new System.EventHandler(this.FormClientes_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCustomers)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainerGeneralDetails.Panel1.ResumeLayout(false);
            this.splitContainerGeneralDetails.Panel1.PerformLayout();
            this.splitContainerGeneralDetails.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGeneralDetails)).EndInit();
            this.splitContainerGeneralDetails.ResumeLayout(false);
            this.splitContainerAccounts.Panel1.ResumeLayout(false);
            this.splitContainerAccounts.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAccounts)).EndInit();
            this.splitContainerAccounts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPayments)).EndInit();
            this.tabPageNotes.ResumeLayout(false);
            this.tabPageNotes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageNotes;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.Label labelNombre;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCodigo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelIDCliente;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelDiaPago;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelRuta;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainerGeneralDetails;
        private System.Windows.Forms.SplitContainer splitContainerAccounts;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dataGridViewPayments;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.DataGridView dataGridViewCustomers;
        private System.Windows.Forms.DataGridView dataGridViewAccounts;
    }
}