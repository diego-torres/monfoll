using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SeguimientoGerente.Collectable;
using SeguimientoGerente.Config;
using SeguimientoGerente.Process;
using SeguimientoGerente.Collectable.PostgresImpl;
using CommonAdminPaq;
using System.Threading;
using Npgsql;
using SeguimientoGerente.Properties;

namespace SeguimientoGerente
{
    public partial class FormMain : Form
    {

        public DataTable DtCustomer { get; set; }
        public DataTable DtSeries { get; set; }
        public DataTable DtFolios { get; set; }

        private AdminPaqImp api;

        private FormConfig fConfig;
        private AboutBox about;
        private FormDownload fDownload;
        private Dictionary<int, FormFollowup> followups = new Dictionary<int, FormFollowup>();
        List<Collectable.Account> adminPaqAccounts = new List<Collectable.Account>();

        readonly object stateLock = new object();
        Collectable.PostgresImpl.Account AccountInterface = new Collectable.PostgresImpl.Account();
        Customer CustomerInterface = new Customer();

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

        public void RefreshProcessAccounts()
        {
            if (IsProcessOpen)
                fProcess.Refresh();
        }

        public void ShowFollowUp(SeguimientoGerente.Collectable.Account account)
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

        private void FormMain_Load(object sender, EventArgs e)
        {
            api = new AdminPaqImp();
            Enterprise dbEnterprise = new Enterprise();
            foreach (Empresa enterprise in api.Empresas)
            {
                try
                {
                    if (ValidateDBConfig())
                    {
                        dbEnterprise.SaveEnterprise(enterprise);
                        DtCustomer = CustomerInterface.ReadCustomers();
                        DtSeries = AccountInterface.ReadSeries();
                        DtFolios = AccountInterface.ReadFolios();
                    }   
                    else
                        OpenConfig();
                }
                catch (Exception ex)
                {
                    ErrLogger.Log(ex.Message);
                }
            }
        }

        private bool ValidateDBConfig()
        {
            try
            {
                NpgsqlConnection conn;
                Settings set = Settings.Default;

                string connString = String.Format("Server={0};Port={1};" +
                        "User Id={2};Password={3};Database={4};",
                        set.server, set.port, set.user,
                        set.password, set.database);
                conn = new NpgsqlConnection(connString);
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("No fue posible establecer una conexión con la base de datos:\n" + err.Message,
                    "Sin conexion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
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

        private void OpenConfig()
        {
            if (fConfig == null || fConfig.IsDisposed)
            {
                fConfig = new FormConfig();
                fConfig.API = api;
                fConfig.MdiParent = this;
            }
            fConfig.Show();
        }

        private void configuracionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConfig();
        }

        private void descargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ShowDownload();
                List<Collectable.Account> adminPaqAccounts = api.DownloadCollectables();
                AccountInterface.UploadAccounts(adminPaqAccounts, api.Cancelados, api.Saldados, api.Conceptos);
                AccountInterface.SetCollectDate(api);

                DtCustomer = CustomerInterface.ReadCustomers();
                DtSeries = AccountInterface.ReadSeries();
                DtFolios = AccountInterface.ReadFolios();
            }
            catch (Exception ex)
            {
                ErrLogger.Log("Unable to download data from AdminPaq: \n" +
                    ex.StackTrace);
                MessageBox.Show("Se ha detectado un error al intentar descargar las cuentas de AdminPaq:\n"
                    + ex.Message + "\n"
                    + ex.StackTrace,
                    "Error al descargar de AdminPaq",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally 
            {
                CloseDownload();
            }
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

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogSearch search = new DialogSearch();
            search.DtCustomer = DtCustomer;
            search.DtSeries = DtSeries;
            search.DtFolios = DtFolios;
            search.ShowDialog();

            if (search.DialogResult == DialogResult.Cancel) return;

            if (!IsProcessOpen)
            {
                fProcess = new FormProcess();
                fProcess.API = api;
                fProcess.MdiParent = this;
            }
            fProcess.Show();
            fProcess.SearchData(search.comboBoxClient.Text, search.comboBoxSerie.Text, search.textBoxFolio.Text);
        }

    }
}
