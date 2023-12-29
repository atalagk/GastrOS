using System;
using System.Windows.Forms;

namespace GastrOs.Wrapper.Forms
{
    public partial class CopyrightDialog : Form
    {
        public CopyrightDialog()
        {
            InitializeComponent();
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}