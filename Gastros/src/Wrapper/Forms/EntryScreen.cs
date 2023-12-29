using System;
using System.Windows.Forms;
using System.Diagnostics;
using GastrOs.Wrapper.Forms.Overview;
using GastrOs.Wrapper.Reports;
using NHibernate;

namespace GastrOs.Wrapper.Forms
{
    public partial class EntryScreen : Form
    {
        private ISession session;
        private IReportFormatter formatter;

        public EntryScreen(ISession session, IReportFormatter formatter)
        {
            this.session = session;
            this.formatter = formatter;

            InitializeComponent();
            CenterToScreen();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void btnDataEntry_Click(object sender, EventArgs e)
        {
            OverviewForm dataEntry = new OverviewForm(session, formatter);
            dataEntry.Closed += Reopen;
            Hide();
            dataEntry.Show();
        }

        private void lnkDisclaimer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CopyrightDialog cd = new CopyrightDialog();
            cd.ShowDialog(this);
            cd.Dispose();
        }

        private void lnkWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://gastros.sourceforge.net");
        }

        private void Reopen(object sender, EventArgs e)
        {
            Show();
        }
    }
}