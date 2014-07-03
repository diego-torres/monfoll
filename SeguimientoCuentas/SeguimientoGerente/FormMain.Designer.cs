namespace SeguimientoGerente
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cuentasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buscarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.descargarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.configuracionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.mainStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cuentasToolStripMenuItem,
            this.toolStripMenuItemConfig});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(744, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cuentasToolStripMenuItem
            // 
            this.cuentasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processToolStripMenuItem,
            this.buscarToolStripMenuItem,
            this.toolStripSeparator1,
            this.descargarToolStripMenuItem});
            this.cuentasToolStripMenuItem.Name = "cuentasToolStripMenuItem";
            this.cuentasToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.cuentasToolStripMenuItem.Text = "C&uentas";
            // 
            // processToolStripMenuItem
            // 
            this.processToolStripMenuItem.Name = "processToolStripMenuItem";
            this.processToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.processToolStripMenuItem.Text = "&Proceso";
            this.processToolStripMenuItem.Click += new System.EventHandler(this.processToolStripMenuItem_Click);
            // 
            // buscarToolStripMenuItem
            // 
            this.buscarToolStripMenuItem.Name = "buscarToolStripMenuItem";
            this.buscarToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.buscarToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.buscarToolStripMenuItem.Text = "Buscar";
            this.buscarToolStripMenuItem.Click += new System.EventHandler(this.buscarToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // descargarToolStripMenuItem
            // 
            this.descargarToolStripMenuItem.Name = "descargarToolStripMenuItem";
            this.descargarToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.descargarToolStripMenuItem.Text = "&Descargar";
            this.descargarToolStripMenuItem.Click += new System.EventHandler(this.descargarToolStripMenuItem_Click);
            // 
            // toolStripMenuItemConfig
            // 
            this.toolStripMenuItemConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configuracionToolStripMenuItem,
            this.toolStripSeparator2,
            this.acercaDeToolStripMenuItem});
            this.toolStripMenuItemConfig.Name = "toolStripMenuItemConfig";
            this.toolStripMenuItemConfig.Size = new System.Drawing.Size(83, 20);
            this.toolStripMenuItemConfig.Text = "&Herramientas";
            // 
            // configuracionToolStripMenuItem
            // 
            this.configuracionToolStripMenuItem.Name = "configuracionToolStripMenuItem";
            this.configuracionToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.configuracionToolStripMenuItem.Text = "Configuración";
            this.configuracionToolStripMenuItem.Click += new System.EventHandler(this.configuracionToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(148, 6);
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.acercaDeToolStripMenuItem.Text = "Acerca de ...";
            this.acercaDeToolStripMenuItem.Click += new System.EventHandler(this.acercaDeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStatus,
            this.mainProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 357);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(744, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // mainStatus
            // 
            this.mainStatus.Name = "mainStatus";
            this.mainStatus.Size = new System.Drawing.Size(306, 17);
            this.mainStatus.Text = "Seguimiento a cuentas por cobrar - módulo de gerente V 3.2.2";
            // 
            // mainProgressBar
            // 
            this.mainProgressBar.Name = "mainProgressBar";
            this.mainProgressBar.Size = new System.Drawing.Size(100, 16);
            this.mainProgressBar.Visible = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(744, 379);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Seguimiento Gerencia";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cuentasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem descargarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemConfig;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripProgressBar mainProgressBar;
        private System.Windows.Forms.ToolStripMenuItem configuracionToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel mainStatus;
        private System.Windows.Forms.ToolStripMenuItem buscarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

