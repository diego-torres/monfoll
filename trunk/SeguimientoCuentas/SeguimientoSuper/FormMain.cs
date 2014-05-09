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
using SeguimientoSuper.Process;
using SeguimientoSuper.Collectable.PostgresImpl;
using CommonAdminPaq;

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
        private Dictionary<int, FormFollowup> followups = new Dictionary<int, FormFollowup>();

        private FormProcess fProcess;
        
        public FormMain()
        {
            InitializeComponent();
        }

        public bool IsProcessOpen
        {
            get
            {
                return !(fProcess == null || fProcess.IsDisposed);
            }
        }

        public bool IsClientesOpen
        {
            get
            {
                return !(fClientes == null || fClientes.IsDisposed);
            }
        }

        public bool IsCollectorsOpen
        {
            get 
            {
                return !(fCobradores == null || fCobradores.IsDisposed);
            }
        }

        public void RefreshProcessAccounts()
        {
            if (IsCollectorsOpen)
                fProcess.Refresh();
        }

        public void RefreshAccountsInCollectors()
        {
            if (IsCollectorsOpen)
                fCobradores.RefreshAccounts();
        }

        public void RefreshAccountsInClientes()
        {
            if (IsClientesOpen)
                fClientes.RefreshAccounts();
        }

        public void ShowFollowUp(SeguimientoSuper.Collectable.Account account)
        {
            FormFollowup currentFollowing;
            bool following = followups.TryGetValue(account.DocId, out currentFollowing); 

            if (!following)
            {
                currentFollowing = new FormFollowup();
                currentFollowing.MdiParent = this;
                currentFollowing.API = api;
                currentFollowing.Account = account;
                followups.Add(account.DocId, currentFollowing);
            }

            if (currentFollowing.IsDisposed)
            {
                currentFollowing = new FormFollowup();
                currentFollowing.MdiParent = this;
                currentFollowing.API = api;
                currentFollowing.Account = account;
            }

            currentFollowing.Show();
            currentFollowing.Focus();
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
            api = new AdminPaqImp();
            Enterprise dbEnterprise = new Enterprise();
            foreach (Empresa enterprise in api.Empresas)
            {
                try 
                {
                    dbEnterprise.SaveEnterprise(enterprise);
                }
                catch (Exception ex) 
                {
                    ErrLogger.Log(ex.Message);
                }
            }
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
            List<Collectable.Account> adminPaqAccounts = api.DownloadCollectables();
            Collectable.PostgresImpl.Account AccountInterface = new Collectable.PostgresImpl.Account();
            AccountInterface.UploadAccounts(adminPaqAccounts, api.Cancelados, api.Conceptos);
            CloseDownload();
        }

        private void processToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IsProcessOpen)
            {
                fProcess = new FormProcess();
                fProcess.API = api;
                fProcess.MdiParent = this;
            }
            fProcess.Show();
        }
    }
}
