using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeguimientoSuper.Collectable.PostgresImpl;

namespace SeguimientoSuper.Catalogs
{
    public partial class DialogAssign : Form
    {
        private Collector dbCollector = new Collector();

        public DialogAssign()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void DialogAssign_Load(object sender, EventArgs e)
        {
            bindingSource1.DataSource = dbCollector.ReadCollectors();
            comboBoxCollector.DataSource = bindingSource1;
            comboBoxCollector.DisplayMember = "nombre_cobrador";
            comboBoxCollector.ValueMember = "id_cobrador";
        }
    }
}
