namespace SeguimientoCobrador.Config
{
    partial class FormConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfig));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxDB = new System.Windows.Forms.TextBox();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSaveDB = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndoChanges = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCheckDB = new System.Windows.Forms.ToolStripButton();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonRemoveAbono = new System.Windows.Forms.Button();
            this.listBoxConceptosAbono = new System.Windows.Forms.ListBox();
            this.buttonRemoveFactura = new System.Windows.Forms.Button();
            this.buttonAddAbono = new System.Windows.Forms.Button();
            this.textBoxConceptoAbono = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonAddFactura = new System.Windows.Forms.Button();
            this.textBoxConceptoFactura = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.listBoxConceptosFactura = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonAllDocuments = new System.Windows.Forms.RadioButton();
            this.radioButtonWithAmount = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonUseDocDate = new System.Windows.Forms.RadioButton();
            this.radioButtonUseCollectDate = new System.Windows.Forms.RadioButton();
            this.dateTimePickerTo = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerFrom = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelRutaEmpresa = new System.Windows.Forms.Label();
            this.comboBoxEmpresas = new System.Windows.Forms.ComboBox();
            this.bindingSourceEmpresas = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceEmpresas)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(683, 435);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBoxPassword);
            this.tabPage2.Controls.Add(this.textBoxUser);
            this.tabPage2.Controls.Add(this.textBoxDB);
            this.tabPage2.Controls.Add(this.textBoxPort);
            this.tabPage2.Controls.Add(this.textBoxServer);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.toolStrip2);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(675, 406);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Base de Datos";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(118, 174);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 22);
            this.textBoxPassword.TabIndex = 10;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(118, 142);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(100, 22);
            this.textBoxUser.TabIndex = 8;
            this.textBoxUser.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxDB
            // 
            this.textBoxDB.Location = new System.Drawing.Point(118, 110);
            this.textBoxDB.Name = "textBoxDB";
            this.textBoxDB.Size = new System.Drawing.Size(128, 22);
            this.textBoxDB.TabIndex = 6;
            this.textBoxDB.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(118, 78);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 22);
            this.textBoxPort.TabIndex = 4;
            this.textBoxPort.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(118, 46);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(207, 22);
            this.textBoxServer.TabIndex = 2;
            this.textBoxServer.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(62, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(50, 16);
            this.label11.TabIndex = 3;
            this.label11.Text = "Puerto:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(32, 177);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(80, 16);
            this.label10.TabIndex = 9;
            this.label10.Text = "Contraseña:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(54, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 16);
            this.label9.TabIndex = 7;
            this.label9.Text = "Usuario:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 113);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(99, 16);
            this.label8.TabIndex = 5;
            this.label8.Text = "Base de datos:";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSaveDB,
            this.toolStripButtonUndoChanges,
            this.toolStripButtonCheckDB});
            this.toolStrip2.Location = new System.Drawing.Point(4, 4);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(667, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButtonSaveDB
            // 
            this.toolStripButtonSaveDB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveDB.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveDB.Image")));
            this.toolStripButtonSaveDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveDB.Name = "toolStripButtonSaveDB";
            this.toolStripButtonSaveDB.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveDB.Text = "Grabar";
            this.toolStripButtonSaveDB.Click += new System.EventHandler(this.toolStripButtonSaveDB_Click);
            // 
            // toolStripButtonUndoChanges
            // 
            this.toolStripButtonUndoChanges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndoChanges.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndoChanges.Image")));
            this.toolStripButtonUndoChanges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndoChanges.Name = "toolStripButtonUndoChanges";
            this.toolStripButtonUndoChanges.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndoChanges.Text = "Reestablecer Cambios";
            this.toolStripButtonUndoChanges.Click += new System.EventHandler(this.toolStripButtonUndoChanges_Click);
            // 
            // toolStripButtonCheckDB
            // 
            this.toolStripButtonCheckDB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCheckDB.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCheckDB.Image")));
            this.toolStripButtonCheckDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCheckDB.Name = "toolStripButtonCheckDB";
            this.toolStripButtonCheckDB.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCheckDB.Text = "Validar Conexión";
            this.toolStripButtonCheckDB.Click += new System.EventHandler(this.toolStripButtonCheckDB_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(50, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "Servidor:";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(675, 406);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "AdminPaq";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonRemoveAbono);
            this.groupBox4.Controls.Add(this.listBoxConceptosAbono);
            this.groupBox4.Controls.Add(this.buttonRemoveFactura);
            this.groupBox4.Controls.Add(this.buttonAddAbono);
            this.groupBox4.Controls.Add(this.textBoxConceptoAbono);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.buttonAddFactura);
            this.groupBox4.Controls.Add(this.textBoxConceptoFactura);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.listBoxConceptosFactura);
            this.groupBox4.Location = new System.Drawing.Point(336, 152);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(311, 229);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Conceptos";
            // 
            // buttonRemoveAbono
            // 
            this.buttonRemoveAbono.Location = new System.Drawing.Point(115, 180);
            this.buttonRemoveAbono.Name = "buttonRemoveAbono";
            this.buttonRemoveAbono.Size = new System.Drawing.Size(60, 23);
            this.buttonRemoveAbono.TabIndex = 8;
            this.buttonRemoveAbono.Text = "<<";
            this.buttonRemoveAbono.UseVisualStyleBackColor = true;
            this.buttonRemoveAbono.Click += new System.EventHandler(this.buttonRemoveAbono_Click);
            // 
            // listBoxConceptosAbono
            // 
            this.listBoxConceptosAbono.FormattingEnabled = true;
            this.listBoxConceptosAbono.ItemHeight = 16;
            this.listBoxConceptosAbono.Location = new System.Drawing.Point(181, 131);
            this.listBoxConceptosAbono.Name = "listBoxConceptosAbono";
            this.listBoxConceptosAbono.Size = new System.Drawing.Size(120, 84);
            this.listBoxConceptosAbono.TabIndex = 9;
            // 
            // buttonRemoveFactura
            // 
            this.buttonRemoveFactura.Location = new System.Drawing.Point(115, 79);
            this.buttonRemoveFactura.Name = "buttonRemoveFactura";
            this.buttonRemoveFactura.Size = new System.Drawing.Size(60, 23);
            this.buttonRemoveFactura.TabIndex = 3;
            this.buttonRemoveFactura.Text = "<<";
            this.buttonRemoveFactura.UseVisualStyleBackColor = true;
            this.buttonRemoveFactura.Click += new System.EventHandler(this.buttonRemoveFactura_Click);
            // 
            // buttonAddAbono
            // 
            this.buttonAddAbono.Location = new System.Drawing.Point(115, 147);
            this.buttonAddAbono.Name = "buttonAddAbono";
            this.buttonAddAbono.Size = new System.Drawing.Size(60, 23);
            this.buttonAddAbono.TabIndex = 7;
            this.buttonAddAbono.Text = ">>";
            this.buttonAddAbono.UseVisualStyleBackColor = true;
            this.buttonAddAbono.Click += new System.EventHandler(this.buttonAddAbono_Click);
            // 
            // textBoxConceptoAbono
            // 
            this.textBoxConceptoAbono.Location = new System.Drawing.Point(9, 148);
            this.textBoxConceptoAbono.MaxLength = 6;
            this.textBoxConceptoAbono.Name = "textBoxConceptoAbono";
            this.textBoxConceptoAbono.Size = new System.Drawing.Size(100, 22);
            this.textBoxConceptoAbono.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(18, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "Conceptos de Abono:";
            // 
            // buttonAddFactura
            // 
            this.buttonAddFactura.Location = new System.Drawing.Point(115, 50);
            this.buttonAddFactura.Name = "buttonAddFactura";
            this.buttonAddFactura.Size = new System.Drawing.Size(60, 23);
            this.buttonAddFactura.TabIndex = 2;
            this.buttonAddFactura.Text = ">>";
            this.buttonAddFactura.UseVisualStyleBackColor = true;
            this.buttonAddFactura.Click += new System.EventHandler(this.buttonAddFactura_Click);
            // 
            // textBoxConceptoFactura
            // 
            this.textBoxConceptoFactura.Location = new System.Drawing.Point(9, 51);
            this.textBoxConceptoFactura.MaxLength = 6;
            this.textBoxConceptoFactura.Name = "textBoxConceptoFactura";
            this.textBoxConceptoFactura.Size = new System.Drawing.Size(100, 22);
            this.textBoxConceptoFactura.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Conceptos de Factura:";
            // 
            // listBoxConceptosFactura
            // 
            this.listBoxConceptosFactura.FormattingEnabled = true;
            this.listBoxConceptosFactura.ItemHeight = 16;
            this.listBoxConceptosFactura.Location = new System.Drawing.Point(181, 34);
            this.listBoxConceptosFactura.Name = "listBoxConceptosFactura";
            this.listBoxConceptosFactura.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxConceptosFactura.Size = new System.Drawing.Size(124, 84);
            this.listBoxConceptosFactura.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonAllDocuments);
            this.groupBox3.Controls.Add(this.radioButtonWithAmount);
            this.groupBox3.Location = new System.Drawing.Point(8, 311);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(317, 70);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Saldos";
            // 
            // radioButtonAllDocuments
            // 
            this.radioButtonAllDocuments.AutoSize = true;
            this.radioButtonAllDocuments.Location = new System.Drawing.Point(143, 21);
            this.radioButtonAllDocuments.Name = "radioButtonAllDocuments";
            this.radioButtonAllDocuments.Size = new System.Drawing.Size(55, 17);
            this.radioButtonAllDocuments.TabIndex = 1;
            this.radioButtonAllDocuments.Text = "Todos";
            this.radioButtonAllDocuments.UseVisualStyleBackColor = true;
            this.radioButtonAllDocuments.CheckedChanged += new System.EventHandler(this.adminPaqconfig_ValueChanged);
            // 
            // radioButtonWithAmount
            // 
            this.radioButtonWithAmount.AutoSize = true;
            this.radioButtonWithAmount.Checked = true;
            this.radioButtonWithAmount.Location = new System.Drawing.Point(14, 21);
            this.radioButtonWithAmount.MinimumSize = new System.Drawing.Size(100, 0);
            this.radioButtonWithAmount.Name = "radioButtonWithAmount";
            this.radioButtonWithAmount.Size = new System.Drawing.Size(100, 17);
            this.radioButtonWithAmount.TabIndex = 0;
            this.radioButtonWithAmount.TabStop = true;
            this.radioButtonWithAmount.Text = "Con Saldo";
            this.radioButtonWithAmount.UseVisualStyleBackColor = true;
            this.radioButtonWithAmount.CheckedChanged += new System.EventHandler(this.adminPaqconfig_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonUseDocDate);
            this.groupBox2.Controls.Add(this.radioButtonUseCollectDate);
            this.groupBox2.Controls.Add(this.dateTimePickerTo);
            this.groupBox2.Controls.Add(this.dateTimePickerFrom);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(8, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 153);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Fechas";
            // 
            // radioButtonUseDocDate
            // 
            this.radioButtonUseDocDate.AutoSize = true;
            this.radioButtonUseDocDate.Checked = true;
            this.radioButtonUseDocDate.Location = new System.Drawing.Point(153, 106);
            this.radioButtonUseDocDate.Name = "radioButtonUseDocDate";
            this.radioButtonUseDocDate.Size = new System.Drawing.Size(130, 17);
            this.radioButtonUseDocDate.TabIndex = 5;
            this.radioButtonUseDocDate.TabStop = true;
            this.radioButtonUseDocDate.Text = "Fecha del Documento";
            this.radioButtonUseDocDate.UseVisualStyleBackColor = true;
            this.radioButtonUseDocDate.CheckedChanged += new System.EventHandler(this.adminPaqconfig_ValueChanged);
            // 
            // radioButtonUseCollectDate
            // 
            this.radioButtonUseCollectDate.AutoSize = true;
            this.radioButtonUseCollectDate.Location = new System.Drawing.Point(6, 106);
            this.radioButtonUseCollectDate.MinimumSize = new System.Drawing.Size(100, 0);
            this.radioButtonUseCollectDate.Name = "radioButtonUseCollectDate";
            this.radioButtonUseCollectDate.Size = new System.Drawing.Size(101, 17);
            this.radioButtonUseCollectDate.TabIndex = 4;
            this.radioButtonUseCollectDate.Text = "Fecha de Cobro";
            this.radioButtonUseCollectDate.UseVisualStyleBackColor = true;
            this.radioButtonUseCollectDate.CheckedChanged += new System.EventHandler(this.adminPaqconfig_ValueChanged);
            // 
            // dateTimePickerTo
            // 
            this.dateTimePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerTo.Location = new System.Drawing.Point(98, 62);
            this.dateTimePickerTo.Name = "dateTimePickerTo";
            this.dateTimePickerTo.Size = new System.Drawing.Size(102, 22);
            this.dateTimePickerTo.TabIndex = 3;
            this.dateTimePickerTo.ValueChanged += new System.EventHandler(this.adminPaqconfig_ValueChanged);
            // 
            // dateTimePickerFrom
            // 
            this.dateTimePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerFrom.Location = new System.Drawing.Point(98, 31);
            this.dateTimePickerFrom.Name = "dateTimePickerFrom";
            this.dateTimePickerFrom.Size = new System.Drawing.Size(102, 22);
            this.dateTimePickerFrom.TabIndex = 1;
            this.dateTimePickerFrom.Value = new System.DateTime(2013, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerFrom.ValueChanged += new System.EventHandler(this.adminPaqconfig_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "Fecha Final:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Fecha Inicial:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSave,
            this.toolStripButtonUndo});
            this.toolStrip1.Location = new System.Drawing.Point(4, 4);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(667, 25);
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
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUndo.Text = "Reestablecer configuración";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelRutaEmpresa);
            this.groupBox1.Controls.Add(this.comboBoxEmpresas);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 46);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(639, 100);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Empresa";
            // 
            // labelRutaEmpresa
            // 
            this.labelRutaEmpresa.AutoSize = true;
            this.labelRutaEmpresa.Location = new System.Drawing.Point(122, 60);
            this.labelRutaEmpresa.Name = "labelRutaEmpresa";
            this.labelRutaEmpresa.Size = new System.Drawing.Size(0, 16);
            this.labelRutaEmpresa.TabIndex = 3;
            // 
            // comboBoxEmpresas
            // 
            this.comboBoxEmpresas.DataSource = this.bindingSourceEmpresas;
            this.comboBoxEmpresas.FormattingEnabled = true;
            this.comboBoxEmpresas.Location = new System.Drawing.Point(125, 24);
            this.comboBoxEmpresas.Name = "comboBoxEmpresas";
            this.comboBoxEmpresas.Size = new System.Drawing.Size(347, 24);
            this.comboBoxEmpresas.TabIndex = 1;
            this.comboBoxEmpresas.SelectionChangeCommitted += new System.EventHandler(this.comboBoxEmpresas_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ruta de Archivos:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre:";
            // 
            // FormConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 435);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormConfig";
            this.Text = "Configuración";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfig_FormClosing);
            this.Load += new System.EventHandler(this.FormConfig_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceEmpresas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonRemoveAbono;
        private System.Windows.Forms.ListBox listBoxConceptosAbono;
        private System.Windows.Forms.Button buttonRemoveFactura;
        private System.Windows.Forms.Button buttonAddAbono;
        private System.Windows.Forms.TextBox textBoxConceptoAbono;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonAddFactura;
        private System.Windows.Forms.TextBox textBoxConceptoFactura;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox listBoxConceptosFactura;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonAllDocuments;
        private System.Windows.Forms.RadioButton radioButtonWithAmount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonUseDocDate;
        private System.Windows.Forms.RadioButton radioButtonUseCollectDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerTo;
        private System.Windows.Forms.DateTimePicker dateTimePickerFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelRutaEmpresa;
        private System.Windows.Forms.ComboBox comboBoxEmpresas;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveDB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxDB;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.BindingSource bindingSourceEmpresas;
        private System.Windows.Forms.ToolStripButton toolStripButtonCheckDB;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndoChanges;
    }
}