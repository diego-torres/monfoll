using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;
using SeguimientoSuper.Collectable;
using SeguimientoSuper.Properties;

namespace SeguimientoSuper.Process
{
    public partial class FormProcess : Form
    {
        private AdminPaqImp api;

        private SeguimientoSuper.Collectable.PostgresImpl.Account dbAccount = new SeguimientoSuper.Collectable.PostgresImpl.Account();

        private DataTable dtBlackList, dtMaster, dtAttended,dtEscalated, dtUncollectable;

        public AdminPaqImp API { get { return api; } set { api = value; } }

        public FormProcess()
        {
            InitializeComponent();
        }

        public void SearchData(String client, String serie, String folio)
        {
            if (string.Empty.Equals(client.Trim()) && string.Empty.Equals(serie.Trim()) && string.Empty.Equals(folio)) return;
            string filterToApply = string.Empty;

            if(!string.Empty.Equals(client.Trim()))
                filterToApply = FilterString("cd_cliente", client);

            if (string.Empty.Equals(filterToApply))
            {
                if (!string.Empty.Equals(serie.Trim()))
                    filterToApply = FilterString("serie_doco", serie);
            }
            else 
            {
                if (!string.Empty.Equals(serie.Trim()))
                    filterToApply += " AND " + FilterString("serie_doco", serie);
            }

            if (string.Empty.Equals(filterToApply))
            {
                if (!string.Empty.Equals(folio.Trim()))
                    filterToApply = FilterString("folio_doco", folio);
            }
            else
            {
                if (!string.Empty.Equals(folio.Trim()))
                    filterToApply += " AND " + FilterString("folio_doco", folio);
            }

            toolStripStatusFilterBlackList.Text = "FILTRO: " + filterToApply;
            dtBlackList.DefaultView.RowFilter = filterToApply;
            dataGridViewBlackList.DataSource = dtBlackList;
            FormatAccountsGridView(dataGridViewBlackList);

            toolStripStatusFilterMaster.Text = "FILTRO: " + filterToApply;
            dtMaster.DefaultView.RowFilter = filterToApply;
            dataGridViewMaster.DataSource = dtMaster;
            FormatAccountsGridView(dataGridViewMaster);

            toolStripStatusFilterAttended.Text = "FILTRO: " + filterToApply;
            dtAttended.DefaultView.RowFilter = filterToApply;
            dataGridViewAttended.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttended);

            toolStripStatusFilterEscalated.Text = "FILTRO: " + filterToApply;
            dtEscalated.DefaultView.RowFilter = filterToApply;
            dataGridViewEscalated.DataSource = dtEscalated;
            FormatAccountsGridView(dataGridViewEscalated);

            toolStripStatusFilterUncollectable.Text = "FILTRO: " + filterToApply;
            dtUncollectable.DefaultView.RowFilter = filterToApply;
            dataGridViewUncollectable.DataSource = dtUncollectable;
            FormatAccountsGridView(dataGridViewUncollectable);

            if (dtBlackList.DefaultView.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 0;
                return;
            }

            if (dtMaster.DefaultView.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 1;
                return;
            }

            if (dtAttended.DefaultView.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 2;
                return;
            }

            if (dtEscalated.DefaultView.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 3;
                return;
            }

            if (dtUncollectable.DefaultView.Count >= 1)
            {
                tabControlProcess.SelectedIndex = 4;
                return;
            }

            MessageBox.Show("No fue posible encontrar la cuenta con los siguientes datos: \n" +
                "Cliente: " + client + "\n" +
                "Serie: " + serie + "\n" +
                "Folio: " + folio,
                "Documento no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }

        
        private void FormProcess_Load(object sender, EventArgs e)
        {
            dtBlackList = dbAccount.BlackListed();
            dataGridViewBlackList.DataSource = dtBlackList;
            FormatAccountsGridView(dataGridViewBlackList);

            dtMaster = dbAccount.MasterTable();
            dataGridViewMaster.DataSource = dtMaster;
            FormatAccountsGridView(dataGridViewMaster);

            //attended
            dtAttended = dbAccount.Attended();
            dataGridViewAttended.DataSource = dtAttended;
            FormatAccountsGridView(dataGridViewAttended);
            
            //escalated
            dtEscalated = dbAccount.Escalated();
            dataGridViewEscalated.DataSource = dtEscalated;
            FormatAccountsGridView(dataGridViewEscalated);

            //uncollectable
            dtUncollectable = dbAccount.Uncollectable();
            dataGridViewUncollectable.DataSource = dtUncollectable;
            FormatAccountsGridView(dataGridViewUncollectable);
        }

        private Payment PaymentFromDataRow(DataRow paymentRow)
        {
            Payment payment = new Payment();
            payment.PaymentId = int.Parse(paymentRow["id_abono"].ToString());
            payment.PaymentType = paymentRow["tipo_pago"].ToString();
            payment.Amount = double.Parse(paymentRow["importe_pago"].ToString());
            payment.Folio = int.Parse(paymentRow["folio"].ToString());
            payment.Concept = paymentRow["concepto"].ToString();
            payment.DepositDate = DateTime.Parse(paymentRow["fecha_deposito"].ToString());
            payment.Account = paymentRow["cuenta"].ToString();

            return payment;
        }

        private int ConfiguredCompanyId()
        {
            Settings set = Settings.Default;
            return set.empresa;
        }

        private Collectable.Account AccountFromGrid(DataGridView dgv)
        {
            Collectable.Account account = new Collectable.Account();
            account.DocId = int.Parse(dgv.CurrentRow.Cells["id_doco"].Value.ToString());
            account.ApId = int.Parse(dgv.CurrentRow.Cells["ap_id"].Value.ToString());
            account.DocDate = DateTime.Parse(dgv.CurrentRow.Cells["f_documento"].Value.ToString());
            account.DueDate = DateTime.Parse(dgv.CurrentRow.Cells["f_vencimiento"].Value.ToString());

            if (!string.Empty.Equals(dgv.CurrentRow.Cells["f_cobro"].Value.ToString()))
                account.CollectDate = DateTime.Parse(dgv.CurrentRow.Cells["f_cobro"].Value.ToString());

            account.Serie = dgv.CurrentRow.Cells["serie_doco"].Value.ToString();
            account.Folio = int.Parse(dgv.CurrentRow.Cells["folio_doco"].Value.ToString());
            account.DocType = dgv.CurrentRow.Cells["tipo_documento"].Value.ToString();
            account.CollectType = dgv.CurrentRow.Cells["tipo_cobro"].Value.ToString();
            account.Amount = double.Parse(dgv.CurrentRow.Cells["facturado"].Value.ToString());
            account.Balance = double.Parse(dgv.CurrentRow.Cells["saldo"].Value.ToString());
            account.Currency = dgv.CurrentRow.Cells["moneda"].Value.ToString();
            account.Note = dgv.CurrentRow.Cells["observaciones"].Value.ToString();

            Collectable.Company company = new Collectable.Company();
            company.Id = int.Parse(dgv.CurrentRow.Cells["id_cliente"].Value.ToString());
            company.Code = dgv.CurrentRow.Cells["cd_cliente"].Value.ToString();
            company.Name = dgv.CurrentRow.Cells["nombre_cliente"].Value.ToString();
            company.AgentCode = dgv.CurrentRow.Cells["ruta"].Value.ToString();
            company.PaymentDay = dgv.CurrentRow.Cells["dia_pago"].Value.ToString();
            company.EnterpriseId = int.Parse(dgv.CurrentRow.Cells["id_empresa"].Value.ToString());
            company.EnterprisePath = dgv.CurrentRow.Cells["ruta_e"].Value.ToString();
            account.Company = company;

            return account;
        }

        private void FormatAccountsGridView(DataGridView dgv)
        {
            dgv.Columns["id_cliente"].Visible = false;
            dgv.Columns["id_doco"].Visible = false;
            dgv.Columns["ap_id"].Visible = false;
            dgv.Columns["id_empresa"].Visible = false;
            dgv.Columns["ruta_e"].Visible = false;

            FixColumn(dgv.Columns["f_documento"], 0, "Fecha Documento", 80);
            FixColumn(dgv.Columns["f_vencimiento"], 1, "Fecha Vencimiento", 80);
            FixColumn(dgv.Columns["dias_vencido"], 2, "Dias Vencimiento", 80);
            FixColumn(dgv.Columns["f_cobro"], 3, "Fecha Cobro", 80);

            FixColumn(dgv.Columns["ruta"], 4, "Ruta", 80);
            FixColumn(dgv.Columns["serie_doco"], 5, "Serie", 40);
            FixColumn(dgv.Columns["folio_doco"], 6, "Doc #", 80);

            FixColumn(dgv.Columns["cd_cliente"], 7, "# Cliente", 80);
            FixColumn(dgv.Columns["nombre_cliente"], 8, "Nombre del Cliente", 180);

            FixColumn(dgv.Columns["tipo_cobro"], 9, "Tipo Cobro", 150);
            FixColumn(dgv.Columns["facturado"], 10, "Total Facturado", 80);
            FixColumn(dgv.Columns["saldo"], 11, "Saldo", 80);
            FixColumn(dgv.Columns["moneda"], 12, "Moneda", 80);
            FixColumn(dgv.Columns["observaciones"], 13, "Observaciones", 150);

            FixColumn(dgv.Columns["tipo_documento"], 14, "Tipo Doc", 150);
            FixColumn(dgv.Columns["dia_pago"], 15, "Dia de Pago", 120);
            FixColumn(dgv.Columns["lista_negra"], 16, "Cuenta Especial", 40);

            dgv.Columns["f_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dgv.Columns["tipo_cobro"].DefaultCellStyle.BackColor = Color.Beige;
            dgv.Columns["observaciones"].DefaultCellStyle.BackColor = Color.Beige;

            dgv.Columns["facturado"].DefaultCellStyle.Format = "c";
            dgv.Columns["saldo"].DefaultCellStyle.Format = "c";

        }

        private void FixColumn(DataGridViewColumn column, int displayedIndex, string HeaderText, int width)
        {
            if (column == null) return;
            column.DisplayIndex = displayedIndex;
            column.HeaderText = HeaderText;
            column.Width = width;
        }

        private string NegativeFilterString(string columnName, string columnValue)
        {
            if (IsNumericColumn(columnName))
                return string.Format("{0}<>{1}", columnName, columnValue);

            if (IsDateColumn(columnName))
            {
                if (string.Empty.Equals(columnValue.Trim()))
                    return string.Format("CONVERT(Isnull({0},''), System.String) <> ''", columnName);
                DateTime dValue = DateTime.Parse(columnValue);
                return string.Format("{0}<>#{1}#", columnName, dValue.ToString("MM/dd/yyyy"));
            }

            return string.Format("{0}<>'{1}'", columnName, columnValue);
        }

        private string FilterString(string columnName, string columnValue)
        {
            if (IsNumericColumn(columnName))
                return string.Format("{0}={1}", columnName, columnValue);

            if (IsDateColumn(columnName))
            {
                if (string.Empty.Equals(columnValue.Trim()))
                    return string.Format("CONVERT(Isnull({0},''), System.String) = ''", columnName);
                DateTime dValue = DateTime.Parse(columnValue);
                return string.Format("{0}=#{1}#", columnName, dValue.ToString("MM/dd/yyyy"));
            }


            return string.Format("{0}='{1}'", columnName, columnValue);
        }

        private bool IsDateColumn(string columnName)
        {
            string[] datedColumns = { "f_documento", "f_vencimiento", "f_cobro", "f_cobro_esperada" };
            return datedColumns.Contains(columnName);
        }

        private bool IsNumericColumn(string columnName)
        {
            string[] numericColumns = { "id_doco", "id_cliente", "folio_doco", "facturado", "saldo", "dias_vencido" };
            return numericColumns.Contains(columnName);
        }

        private List<int> SelectedIds(DataGridView dgv)
        {
            List<int> selectedIds = new List<int>();

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                DataGridViewRow selectedRow = dgv.Rows[cell.RowIndex];
                int selectedId = int.Parse(selectedRow.Cells["id_doco"].Value.ToString());
                if (!selectedIds.Contains(selectedId))
                    selectedIds.Add(selectedId);
            }
            return selectedIds;
        }

        private List<Collectable.Account> SelectedAdminId(DataGridView dgv)
        {
            List<Collectable.Account> selectedAdminId = new List<Collectable.Account>();

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                DataGridViewRow selectedRow = dgv.Rows[cell.RowIndex];
                int selectedId = int.Parse(selectedRow.Cells["ap_id"].Value.ToString());
                int selectedPgDocId = int.Parse(selectedRow.Cells["id_doco"].Value.ToString());
                string rutaEmpresa = selectedRow.Cells["ruta_e"].Value.ToString();

                Collectable.Account pgDoco = new Collectable.Account();
                pgDoco.ApId = selectedId;
                pgDoco.DocId = selectedPgDocId;
                Company co = new Company();
                co.EnterprisePath = rutaEmpresa;
                pgDoco.Company = co;

                if (!selectedAdminId.Contains(pgDoco))
                    selectedAdminId.Add(pgDoco);
            }
            return selectedAdminId;
        }

        #region CONTEXT_MENU

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (!(sender is DataGridView)) return;

                DataGridView dgvSender = (DataGridView)sender;
                var hti = dgvSender.HitTest(e.X, e.Y);
                if (hti.RowIndex >= 0 && hti.ColumnIndex >= 0)
                {
                    dgvSender.ClearSelection();
                    dgvSender.Rows[hti.RowIndex].Cells[hti.ColumnIndex].Selected = true;
                    dgvSender.CurrentCell = dgvSender.Rows[hti.RowIndex].Cells[hti.ColumnIndex];
                }
            }
        }

        private void applyFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DataGridView dgvSender = null;
            DataTable dtSender = null;
            ToolStripStatusLabel status = null;

            switch (tabControlProcess.SelectedIndex)
            {
                case 0:
                    dgvSender = dataGridViewBlackList;
                    dtSender = dtBlackList;
                    status = toolStripStatusFilterBlackList;
                    break;
                case 1:
                    dgvSender = dataGridViewMaster;
                    dtSender = dtMaster;
                    status = toolStripStatusFilterMaster;
                    break;
                case 2:
                    dgvSender = dataGridViewAttended;
                    dtSender = dtAttended;
                    status = toolStripStatusFilterAttended;
                    break;
                case 3:
                    dgvSender = dataGridViewEscalated;
                    dtSender = dtEscalated;
                    status = toolStripStatusFilterEscalated;
                    break;
                case 4:
                    dgvSender = dataGridViewUncollectable;
                    dtSender = dtUncollectable;
                    status = toolStripStatusFilterUncollectable;
                    break;
                default:
                    return;
            }

            string filterToApply = dtSender.DefaultView.RowFilter;
            string columnName = dgvSender.Columns[dgvSender.CurrentCell.ColumnIndex].Name;
            string filterValue = dgvSender.CurrentCell.Value.ToString();


            if (string.Empty.Equals(filterToApply))
                filterToApply = FilterString(columnName, filterValue);
            else
                filterToApply += " AND " + FilterString(columnName, filterValue);
            
            status.Text = "FILTRO: " + filterToApply;

            dtSender.DefaultView.RowFilter = filterToApply;
            dgvSender.DataSource = dtSender;
            FormatAccountsGridView(dgvSender);
        }

        private void avoidValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridView dgvSender = null;
            DataTable dtSender = null;
            ToolStripStatusLabel status = null;

            switch (tabControlProcess.SelectedIndex)
            {
                case 0:
                    dgvSender = dataGridViewBlackList;
                    dtSender = dtBlackList;
                    status = toolStripStatusFilterBlackList;
                    break;
                case 1:
                    dgvSender = dataGridViewMaster;
                    dtSender = dtMaster;
                    status = toolStripStatusFilterMaster;
                    break;
                case 2:
                    dgvSender = dataGridViewAttended;
                    dtSender = dtAttended;
                    status = toolStripStatusFilterAttended;
                    break;
                case 3:
                    dgvSender = dataGridViewEscalated;
                    dtSender = dtEscalated;
                    status = toolStripStatusFilterEscalated;
                    break;
                case 4:
                    dgvSender = dataGridViewUncollectable;
                    dtSender = dtUncollectable;
                    status = toolStripStatusFilterUncollectable;
                    break;
                default:
                    return;
            }

            string filterToApply = dtSender.DefaultView.RowFilter;
            string columnName = dgvSender.Columns[dgvSender.CurrentCell.ColumnIndex].Name;
            string filterValue = dgvSender.CurrentCell.Value.ToString();

            if (string.Empty.Equals(filterToApply))
                filterToApply = NegativeFilterString(columnName, filterValue);
            else
                filterToApply += " AND " + NegativeFilterString(columnName, filterValue);

            status.Text = "FILTRO: " + filterToApply;

            dtSender.DefaultView.RowFilter = filterToApply;
            dgvSender.DataSource = dtSender;
            FormatAccountsGridView(dgvSender);

        }

        #endregion

        #region PRINTERS

        private List<Reports.Account> ReportAccountsFromGrid(DataGridView dgv)
        {
            List<Reports.Account> rAccounts = new List<Reports.Account>();

            foreach (DataGridViewRow documentRow in dgv.Rows)
            {
                Reports.Account ra = new Reports.Account();
                ra.Name = documentRow.Cells["nombre_cliente"].Value.ToString();
                ra.AgentCode = documentRow.Cells["ruta"].Value.ToString();

                ra.Currency = documentRow.Cells["moneda"].Value.ToString();
                if (ra.Currency.ToUpper().Contains("PESO"))
                {
                    ra.Balance = double.Parse(documentRow.Cells["saldo"].Value.ToString());
                    ra.Amount = double.Parse(documentRow.Cells["facturado"].Value.ToString());
                    ra.Dolares = 0;
                    ra.TotalDolares = 0;
                }
                else
                {
                    ra.Balance = 0;
                    ra.Amount = 0;
                    ra.Dolares = double.Parse(documentRow.Cells["saldo"].Value.ToString());
                    ra.TotalDolares = double.Parse(documentRow.Cells["facturado"].Value.ToString());
                }

                if (documentRow.Cells["f_cobro"].Value.ToString() == "")
                {
                    ra.CollectDate = new DateTime(1899, 12, 30);
                }
                else
                {
                    ra.CollectDate = DateTime.Parse(documentRow.Cells["f_cobro"].Value.ToString());
                }
                ra.CollectType = documentRow.Cells["tipo_cobro"].Value.ToString();
                ra.CompanyCode = documentRow.Cells["cd_cliente"].Value.ToString();
                ra.DocDate = DateTime.Parse(documentRow.Cells["f_documento"].Value.ToString());
                ra.DocId = int.Parse(documentRow.Cells["id_doco"].Value.ToString());
                ra.DocType = documentRow.Cells["tipo_documento"].Value.ToString();
                ra.DueDate = DateTime.Parse(documentRow.Cells["f_vencimiento"].Value.ToString());
                ra.Folio = int.Parse(documentRow.Cells["folio_doco"].Value.ToString());
                ra.Note = documentRow.Cells["observaciones"].Value.ToString();
                ra.PaymentDay = documentRow.Cells["dia_pago"].Value.ToString();
                ra.Serie = documentRow.Cells["serie_doco"].Value.ToString();

                rAccounts.Add(ra);
            }
            return rAccounts;
        }

        private void ShowReportFromGrid(DataGridView dgv)
        {
            Reports.ReportViewer rv = new Reports.ReportViewer();
            rv.ReportAccounts = ReportAccountsFromGrid(dgv);
            rv.Show();
        }

        private void toolStripButtonPrintBlackList_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewBlackList);
        }

        private void toolStripButtonPrintMaster_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewMaster);
        }

        private void toolStripButtonPrintAttended_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewAttended);
        }

        private void toolStripButtonPrintEscalated_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewEscalated);
        }

        private void toolStripButtonPrintUncollectable_Click(object sender, EventArgs e)
        {
            ShowReportFromGrid(dataGridViewUncollectable);
        }
        #endregion

        #region GRID_REFRESHERS
        private void RefreshBlackList()
        {
            string prevFilter = dtBlackList.DefaultView.RowFilter;
            dtBlackList = dbAccount.BlackListed();
            dtBlackList.DefaultView.RowFilter = prevFilter;
            dataGridViewBlackList.DataSource = dtBlackList;

            FormatAccountsGridView(dataGridViewBlackList);
        }

        private void RefreshMaster()
        {
            string prevFilter = dtMaster.DefaultView.RowFilter;
            dtMaster = dbAccount.MasterTable();
            dtMaster.DefaultView.RowFilter = prevFilter;
            dataGridViewMaster.DataSource = dtMaster;

            FormatAccountsGridView(dataGridViewMaster);
        }

        private void RefreshAttended()
        {
            string prevFilter = dtAttended.DefaultView.RowFilter;
            dtAttended = dbAccount.Attended();
            dtAttended.DefaultView.RowFilter = prevFilter;
            dataGridViewAttended.DataSource = dtAttended;

            FormatAccountsGridView(dataGridViewAttended);
        }

        private void RefreshEscalated()
        {
            string prevFilter = dtEscalated.DefaultView.RowFilter;
            dtEscalated = dbAccount.Escalated();
            dtEscalated.DefaultView.RowFilter = prevFilter;
            dataGridViewEscalated.DataSource = dtEscalated;

            FormatAccountsGridView(dataGridViewEscalated);
        }

        private void RefreshUncollectable()
        {
            string prevFilter = dtUncollectable.DefaultView.RowFilter;
            dtUncollectable = dbAccount.Uncollectable();
            dtUncollectable.DefaultView.RowFilter = prevFilter;
            dataGridViewUncollectable.DataSource = dtUncollectable;

            FormatAccountsGridView(dataGridViewUncollectable);
        }

        #endregion

        #region BLACK_LISTERS
        private void toolStripButtonUnlockBlackList_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewBlackList);

            foreach (int id in selectedIds)
            {
                dbAccount.UnEspecialize(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido removidas de las cuentas especiales, encuentrelas ahora en la lista maestra",
                "Cuentas Especiales", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
            RefreshMaster();
            RefreshAttended();
        }

        private void toolStripButtonLockMaster_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewMaster);

            foreach (int id in selectedIds)
            {
                dbAccount.Especialize(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido enviadas a cuentas especiales y no estarán disponibles para los cobradores en la lista maestra",
                "Cuentas Especiales", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
            RefreshMaster();
        }

        private void toolStripButtonLockAttended_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAttended);

            foreach (int id in selectedIds)
            {
                dbAccount.Especialize(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido enviadas a cuentas especiales y no estarán disponibles para los cobradores en la lista maestra",
                "Cuentas Especiales", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
            RefreshAttended();
        }
        #endregion

        # region ESCALATORS
        private void toolStripButtonEscaleBlackList_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewBlackList);

            foreach (int id in selectedIds)
            {
                dbAccount.Escale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.",
                "Cuentas Escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
            RefreshEscalated();
        }

        private void toolStripButtonEscaleMaster_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewMaster);

            foreach (int id in selectedIds)
            {
                dbAccount.Escale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.",
                "Cuentas Escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshMaster();
            RefreshEscalated();
        }

        private void toolStripButtonEscaleAttended_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAttended);

            foreach (int id in selectedIds)
            {
                dbAccount.Escale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.",
                "Cuentas Escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
            RefreshEscalated();
        }

        private void toolStripButtonRemoveEscalated_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewEscalated);

            foreach (int id in selectedIds)
            {
                dbAccount.Unescale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido removidas de la lista de escalación a gerencia.",
                "Cuentas Escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
            RefreshMaster();
            RefreshBlackList();
            RefreshEscalated();
        }
        #endregion

        #region UNCOLLECTORS
        private void toolStripButtonUncollectableBlackList_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewBlackList);

            foreach (int id in selectedIds)
            {
                dbAccount.Uncollectable(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido marcadas como incobrables.",
                "Cuentas Incobrables", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
            RefreshUncollectable();
        }

        private void toolStripButtonUncollectableMaster_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewMaster);

            foreach (int id in selectedIds)
            {
                dbAccount.Uncollectable(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido marcadas como incobrables.",
                "Cuentas Incobrables", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshMaster();
            RefreshUncollectable();
        }

        private void toolStripButtonUncollectableAttended_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAttended);

            foreach (int id in selectedIds)
            {
                dbAccount.Uncollectable(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido marcadas como incobrables.",
                "Cuentas Incobrables", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
            RefreshUncollectable();
        }

        private void toolStripButtonUncollectableEscalated_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewEscalated);

            foreach (int id in selectedIds)
            {
                dbAccount.Uncollectable(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido marcadas como incobrables.",
                "Cuentas Incobrables", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshEscalated();
            RefreshUncollectable();
        }

        private void toolStripButtonRemoveUncollectable_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewUncollectable);

            foreach (int id in selectedIds)
            {
                dbAccount.Collectable(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido recuperadas para las listas de cobranza.",
                "Cuentas Incobrables", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUncollectable();
            RefreshBlackList();
            RefreshMaster();
            RefreshAttended();
            RefreshEscalated();
        }
        #endregion

        #region DATE_SETTERS
        private void toolStripButtonSetCollectDateBlackList_Click(object sender, EventArgs e)
        {
            DialogCollectDate dlgCollectDate = new DialogCollectDate();
            dlgCollectDate.ShowDialog();
            if (dlgCollectDate.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewBlackList);

            foreach(Collectable.Account account in selectedIds)
            {
                if (!dlgCollectDate.dateTimePickerCollectDate.Checked)
                {
                    //18991230
                    DateTime dt = new DateTime(1899, 12, 30);
                    dbAccount.SetCollectDate(account.DocId, dt);
                    api.SetCollectDate(account.ApId, dt, account.Company.EnterprisePath);
                }
                else
                {
                    dbAccount.SetCollectDate(account.DocId, dlgCollectDate.dateTimePickerCollectDate.Value);
                    api.SetCollectDate(account.ApId, dlgCollectDate.dateTimePickerCollectDate.Value, account.Company.EnterprisePath);
                }
            }

            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
        }

        private void toolStripButtonSetDateMaster_Click(object sender, EventArgs e)
        {
            DialogCollectDate dlgCollectDate = new DialogCollectDate();
            dlgCollectDate.ShowDialog();
            if (dlgCollectDate.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewMaster);

            foreach (Collectable.Account account in selectedIds)
            {
                if (!dlgCollectDate.dateTimePickerCollectDate.Checked)
                {
                    //18991230
                    DateTime dt = new DateTime(1899, 12, 30);
                    dbAccount.SetCollectDate(account.DocId, dt);
                    api.SetCollectDate(account.ApId, dt, account.Company.EnterprisePath);
                }
                else
                {
                    dbAccount.SetCollectDate(account.DocId, dlgCollectDate.dateTimePickerCollectDate.Value);
                    api.SetCollectDate(account.ApId, dlgCollectDate.dateTimePickerCollectDate.Value, account.Company.EnterprisePath);
                }
            }

            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshMaster();
        }

        private void toolStripButtonSetCollectDateAttended_Click(object sender, EventArgs e)
        {
            DialogCollectDate dlgCollectDate = new DialogCollectDate();
            dlgCollectDate.ShowDialog();
            if (dlgCollectDate.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAttended);

            foreach (Collectable.Account account in selectedIds)
            {
                if (!dlgCollectDate.dateTimePickerCollectDate.Checked)
                {
                    //18991230
                    DateTime dt = new DateTime(1899, 12, 30);
                    dbAccount.SetCollectDate(account.DocId, dt);
                    api.SetCollectDate(account.ApId, dt, account.Company.EnterprisePath);
                }
                else
                {
                    dbAccount.SetCollectDate(account.DocId, dlgCollectDate.dateTimePickerCollectDate.Value);
                    api.SetCollectDate(account.ApId, dlgCollectDate.dateTimePickerCollectDate.Value, account.Company.EnterprisePath);
                }
            }

            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
        }

        private void toolStripButtonSetCollectDateEscalated_Click(object sender, EventArgs e)
        {
            DialogCollectDate dlgCollectDate = new DialogCollectDate();
            dlgCollectDate.ShowDialog();
            if (dlgCollectDate.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewEscalated);

            foreach (Collectable.Account account in selectedIds)
            {
                if (!dlgCollectDate.dateTimePickerCollectDate.Checked)
                {
                    //18991230
                    DateTime dt = new DateTime(1899, 12, 30);
                    dbAccount.SetCollectDate(account.DocId, dt);
                    api.SetCollectDate(account.ApId, dt, account.Company.EnterprisePath);
                }
                else
                {
                    dbAccount.SetCollectDate(account.DocId, dlgCollectDate.dateTimePickerCollectDate.Value);
                    api.SetCollectDate(account.ApId, dlgCollectDate.dateTimePickerCollectDate.Value, account.Company.EnterprisePath);
                }
            }

            MessageBox.Show("Fecha de cobro actualizada en AdminPaq.", "Fecha de cobro asignada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshEscalated();
        }
        #endregion

        #region NOTE_SETTERS
        private void toolStripButtonSetObservationsBlackList_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewBlackList);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text, account.Company.EnterprisePath);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshBlackList();
        }

        private void toolStripButtonSetObservationsMaster_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewMaster);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text, account.Company.EnterprisePath);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshMaster();
        }

        private void toolStripButtonSetObservationsAttended_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewAttended);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text, account.Company.EnterprisePath);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
        }

        private void toolStripButtonSetObservationsEscalated_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewEscalated);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text, account.Company.EnterprisePath);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshEscalated();
        }

        private void toolStripButtonSetObservationsUncollectable_Click(object sender, EventArgs e)
        {
            DialogObservations dlgObs = new DialogObservations();
            dlgObs.ShowDialog();
            if (dlgObs.DialogResult == DialogResult.Cancel) return;

            List<Collectable.Account> selectedIds = SelectedAdminId(dataGridViewUncollectable);

            foreach (Collectable.Account account in selectedIds)
            {
                dbAccount.SetObservations(account.DocId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text);
                api.SetObservations(account.ApId, dlgObs.textBoxCollectType.Text, dlgObs.textBoxObservations.Text, account.Company.EnterprisePath);
            }
            MessageBox.Show("Observaciones actualizadas exitosamente en AdminPaq.", "Observaciones actualizadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshUncollectable();
        }
        #endregion

        private void toolStripButtonCheckAttended_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewAttended);

            foreach (int id in selectedIds)
            {
                dbAccount.Review(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido marcadas como revisadas.",
                "Cuentas revisadas", MessageBoxButtons.OK, MessageBoxIcon.Information);

            RefreshAttended();
            RefreshMaster();
            RefreshBlackList();
        }

        #region FILTER_REMOVERS
        private void toolStripButtonRemoveFilterBlackList_Click(object sender, EventArgs e)
        {
            dtBlackList = dbAccount.BlackListed();
            dtBlackList.DefaultView.RowFilter = string.Empty;
            dataGridViewBlackList.DataSource = dtBlackList;
            toolStripStatusFilterBlackList.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilterMaster_Click(object sender, EventArgs e)
        {
            dtMaster = dbAccount.MasterTable();
            dtMaster.DefaultView.RowFilter = string.Empty;
            dataGridViewMaster.DataSource = dtMaster;
            toolStripStatusFilterMaster.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilterAttended_Click(object sender, EventArgs e)
        {
            dtAttended = dbAccount.Attended();
            dtAttended.DefaultView.RowFilter = string.Empty;
            dataGridViewAttended.DataSource = dtAttended;
            toolStripStatusFilterAttended.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilterEscalated_Click(object sender, EventArgs e)
        {
            dtEscalated = dbAccount.Escalated();
            dtEscalated.DefaultView.RowFilter = string.Empty;
            dataGridViewEscalated.DataSource = dtEscalated;
            toolStripStatusFilterEscalated.Text = "FILTRO:";
        }

        private void toolStripButtonRemoveFilterUncollectable_Click(object sender, EventArgs e)
        {
            dtUncollectable = dbAccount.Uncollectable();
            dtUncollectable.DefaultView.RowFilter = string.Empty;
            dataGridViewUncollectable.DataSource = dtUncollectable;
            toolStripStatusFilterUncollectable.Text = "FILTRO:";
        }
        #endregion

        # region REFRESH_BUTTONS
        private void toolStripButtonRefreshBlackList_Click(object sender, EventArgs e)
        {
            RefreshBlackList();
        }

        private void toolStripButtonRefreshMaster_Click(object sender, EventArgs e)
        {
            RefreshMaster();
        }

        private void toolStripButtonRefreshAttended_Click(object sender, EventArgs e)
        {
            RefreshAttended();
        }

        private void toolStripButtonUpdateEscalated_Click(object sender, EventArgs e)
        {
            RefreshEscalated();
        }

        private void toolStripButtonRefreshUncollectable_Click(object sender, EventArgs e)
        {
            RefreshUncollectable();
        }
        #endregion

        #region FOLLOW_UP
        private void dataGridViewBlackList_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewBlackList.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewBlackList);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewMaster_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewMaster.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewMaster);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewAttended_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewAttended.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewAttended);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewEscalated_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewEscalated.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewEscalated);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }

        private void dataGridViewUncollectable_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewUncollectable.CurrentRow == null) return;
            FormMain parent = (FormMain)this.MdiParent;
            Collectable.Account account = AccountFromGrid(dataGridViewUncollectable);

            DataTable dtPayments = dbAccount.ReadPayments(account.DocId);

            foreach (DataRow paymentRow in dtPayments.Rows)
            {
                Payment payment = PaymentFromDataRow(paymentRow);
                payment.DocId = account.DocId;
                account.Payments.Add(payment);
            }

            parent.ShowFollowUp(account);
        }
        #endregion

        #region ADMIN_PAQ_DOWNLOADERS
        private void UpdateFromAdminPaq(DataGridView dgv)
        {
            Settings set = Settings.Default;
            SeguimientoSuper.Collectable.PostgresImpl.Enterprise dbEnterprise = new SeguimientoSuper.Collectable.PostgresImpl.Enterprise();

            foreach (DataGridViewCell cell in dgv.SelectedCells)
            {
                DataGridViewRow selectedRow = dgv.Rows[cell.RowIndex];

                int selectedId = int.Parse(selectedRow.Cells["ap_id"].Value.ToString());
                int selectedPgDocId = int.Parse(selectedRow.Cells["id_doco"].Value.ToString());

                Collectable.Account pgDoco = new Collectable.Account();
                pgDoco.ApId = selectedId;
                pgDoco.DocId = selectedPgDocId;
                
                bool cancelled = false;

                api.DownloadCollectable(ref pgDoco, dbEnterprise.ConceptosPago(set.empresa), out cancelled);
                //Collectable.PostgresImpl.Account dbAccount = new Collectable.PostgresImpl.Account();
                dbAccount.SaveAccount(pgDoco);

                foreach (Collectable.Payment payment in pgDoco.Payments)
                {
                    payment.DocId = pgDoco.DocId;
                    dbAccount.SavePayment(payment);
                }

                if (cancelled) 
                    dbAccount.CancelAccount(pgDoco.DocId);
            }
        }

        private void toolStripButtonAdminPaqDownloadBlackList_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewBlackList);
            RefreshBlackList();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButtonAdminPaqDownloadMaster_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewMaster);
            List<Collectable.Account> adminPaqAccounts = api.DownloadCollectables(DateTime.Today, DateTime.Today);
            dbAccount.UploadAccounts(adminPaqAccounts, api.Cancelados, api.Saldados, api.Conceptos);
            dbAccount.SetCollectDate(api);
            RefreshMaster();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButtonAdminPaqDownloadAttended_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewAttended);
            RefreshAttended();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButtonDownloadEscalated_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewEscalated);
            RefreshEscalated();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripButtonDownloadUncollectable_Click(object sender, EventArgs e)
        {
            UpdateFromAdminPaq(dataGridViewUncollectable);
            RefreshUncollectable();
            MessageBox.Show("Datos extraidos de adminPaq existosamente.", "Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region FILTER_SAVERS
        private void SaveFilter(string filter)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            saveFileDialog1.Title = "Grabar Filtro";
            //saveFileDialog1.CheckFileExists = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "ftr";
            saveFileDialog1.Filter = "Filtros (*.ftr)|*.ftr";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!saveFileDialog1.FileName.Equals(string.Empty))
                    System.IO.File.WriteAllText(saveFileDialog1.FileName, filter);
            }
        }

        private void toolStripButtonSaveFilterBlackList_Click(object sender, EventArgs e)
        {
            if(!dtBlackList.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtBlackList.DefaultView.RowFilter);
        }

        private void toolStripButtonSaveFilterMaster_Click(object sender, EventArgs e)
        {
            if(!dtMaster.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtMaster.DefaultView.RowFilter);
        }

        private void toolStripButtonSaveFilterAttended_Click(object sender, EventArgs e)
        {
            if (!dtAttended.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtAttended.DefaultView.RowFilter);
        }

        private void toolStripButtonSaveFilterEscalated_Click(object sender, EventArgs e)
        {
            if (!dtEscalated.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtEscalated.DefaultView.RowFilter);
        }

        private void toolStripButtonSaveFilterUncollectable_Click(object sender, EventArgs e)
        {
            if (!dtUncollectable.DefaultView.RowFilter.Equals(string.Empty))
                SaveFilter(dtUncollectable.DefaultView.RowFilter);
        }
        #endregion

        private string ReadFilter()
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "Filtros (*.ftr)|*.ftr";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            ;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open the selected file to read.
                string filter = System.IO.File.ReadAllText(openFileDialog1.FileName);
                return filter;
            }
            return string.Empty;
        }

        private void toolStripButtonOpenFilterBlackList_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtBlackList.DefaultView.RowFilter = filter;
                dataGridViewBlackList.DataSource = dtBlackList;

                FormatAccountsGridView(dataGridViewBlackList);
                toolStripStatusFilterBlackList.Text = "FILTRO: " + filter;
            }
        }

        private void toolStripButtonOpenFilterMaster_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtMaster.DefaultView.RowFilter = filter;
                dataGridViewMaster.DataSource = dtMaster;

                FormatAccountsGridView(dataGridViewMaster);
                toolStripStatusFilterMaster.Text = "FILTRO: " + filter;
            }
        }

        private void toolStripButtonOpenFilterAttended_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtAttended.DefaultView.RowFilter = filter;
                dataGridViewAttended.DataSource = dtAttended;

                FormatAccountsGridView(dataGridViewAttended);
                toolStripStatusFilterAttended.Text = "FILTRO: " + filter;
            }
        }

        private void toolStripButtonOpenFilterEscalated_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtEscalated.DefaultView.RowFilter = filter;
                dataGridViewEscalated.DataSource = dtEscalated;

                FormatAccountsGridView(dataGridViewEscalated);
                toolStripStatusFilterEscalated.Text = "FILTRO: " + filter;
            }
        }

        private void toolStripButtonOpenFilterUncollectable_Click(object sender, EventArgs e)
        {
            string filter = ReadFilter();
            if (!string.Empty.Equals(filter))
            {
                dtUncollectable.DefaultView.RowFilter = filter;
                dataGridViewUncollectable.DataSource = dtUncollectable;

                FormatAccountsGridView(dataGridViewUncollectable);
                toolStripStatusFilterUncollectable.Text = "FILTRO: " + filter;
            }
        }

    }
}
