using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SeguimientoSuper.Catalogs;
using SeguimientoSuper.Collectable;
using SeguimientoSuper.Config;

namespace SeguimientoSuper
{
    public partial class FormMain : Form
    {
        private AdminPaqImp api;

        private FormCobradores fCobradores;
        private FormClientes fClientes;

        private FormConfig fConfig;
        private AboutBox about;
        private FormDownload fDownload;
        
        public FormMain()
        {
            InitializeComponent();
        }

        public void ShowDownload()
        {
            if (fDownload == null || fDownload.IsDisposed)
            {
                fDownload = new FormDownload();
                fDownload.MdiParent = this;
            }
            fDownload.Show();
        }

        internal void CloseDownload()
        {
            fDownload.Close();
        }

        private void cobradoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fCobradores == null || fCobradores.IsDisposed)
            {
                fCobradores = new FormCobradores();
                fCobradores.MdiParent = this;
            }
            fCobradores.Show();
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fClientes == null || fClientes.IsDisposed)
            {
                fClientes = new FormClientes();
                fClientes.MdiParent = this;
            }

            fClientes.Show();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            mainStatus.Text = "Cargando información general de AdminPaq ...";
            api = new AdminPaqImp();
            mainStatus.Text = "Listo";
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (about == null || about.IsDisposed)
            {
                about = new AboutBox();
                about.MdiParent = this;
            }

            about.Show();
        }

        private void configuracionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fConfig == null || fConfig.IsDisposed)
            {
                fConfig = new FormConfig();
                fConfig.API = api;
                fConfig.MdiParent = this;
            }
            fConfig.Show();
        }

        private void descargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowDownload();

            List<Collectable.Account> adminPaqAccounts = api.DownloadCollectables(mainStatus);
            mainStatus.Text = "Uploading accounts to databasel";
            Collectable.PostgresImpl.Account AccountInterface = new Collectable.PostgresImpl.Account();
            AccountInterface.UploadAccounts(adminPaqAccounts);
            mainStatus.Text = "Listo";
            CloseDownload();
        }

        
    }
}
