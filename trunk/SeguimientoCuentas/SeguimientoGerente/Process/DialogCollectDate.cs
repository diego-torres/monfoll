using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SeguimientoGerente.Process
{
    public partial class DialogCollectDate : Form
    {
        public DialogCollectDate()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void DialogCollectDate_Load(object sender, EventArgs e)
        {
            dateTimePickerCollectDate.MinDate = DateTime.Today;
            dateTimePickerCollectDate.Value = DateTime.Today;
        }
    }
}
