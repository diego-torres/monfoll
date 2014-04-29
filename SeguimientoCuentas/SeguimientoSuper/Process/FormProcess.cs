using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;

namespace SeguimientoSuper.Process
{
    public partial class FormProcess : Form
    {
        private Account dbAccount = new Account();

        public FormProcess()
        {
            InitializeComponent();
        }

        private void FormProcess_Load(object sender, EventArgs e)
        {
            dataGridViewNotAssignedAccounts.DataSource = dbAccount.UnAssigned();
            dataGridViewAssignedAccounts.DataSource = dbAccount.Assigned();
            dataGridViewEscalatedAccounts.DataSource = dbAccount.Escalated();
            dataGridViewClosedAccounts.DataSource = dbAccount.Closed();
            dataGridViewCancelledAccounts.DataSource = dbAccount.Cancelled();
        }

        private void NAtoolStripButtonEscale_Click(object sender, EventArgs e)
        {
            List<int> selectedIds = SelectedIds(dataGridViewNotAssignedAccounts);

            foreach (int id in selectedIds)
            {
                dbAccount.Escale(id);
            }

            MessageBox.Show("Las cuentas seleccionadas han sido escaladas a gerencia.", "Cuentas escaladas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dataGridViewNotAssignedAccounts.DataSource = dbAccount.UnAssigned();
            dataGridViewEscalatedAccounts.DataSource = dbAccount.Escalated();
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
    }
}
