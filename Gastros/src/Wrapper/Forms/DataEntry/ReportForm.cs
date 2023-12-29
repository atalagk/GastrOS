using System;
using System.Windows.Forms;
using GastrOs.Wrapper.DataObjects;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    public partial class ReportForm : Form
    {
        public ReportForm(ReportExam exRpt)
        {
            InitializeComponent();
            // Get existing instance which contains all data needed for reporting
            
            BindingSource.DataSource = exRpt;     
            
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            this.reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            this.reportViewer1.RefreshReport();
        }
    }
}