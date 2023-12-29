using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using GastrOs.Sde.Helpers;
using GastrOs.Wrapper.DataObjects;
//using GastrOs.Wrapper.View;
using NHibernate;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    public partial class SignoutPrintForm : Form
    {
        private Examination examination;
        private ReportExam repEx;
        private ISession session;

        public SignoutPrintForm(Examination exam, ISession session)
        {
            examination = exam;
            this.session = session;
            
            InitializeComponent();

            //NOTE tip from HY
            //Pass in this variable below to the form that displays the list of endoscopists
            //Then assign examination.Signout to the selected SignoutEndoscopist object

            IList<SignoutEndoscopist> endoscopists =
                session.CreateQuery("from " + typeof(SignoutEndoscopist).Name).List<SignoutEndoscopist>();

            dataEndoscopist1.DataSource = endoscopists;
            dataEndoscopist2.DataSource = endoscopists;
            dataEndoscopist3.DataSource = endoscopists;
            dataEndoscopist4.DataSource = endoscopists;
                        
            repEx = new ReportExam();
            repEx.ID = examination.ID;
            repEx.Patient = examination.Patient;
            //full name comes from get
            //age comes from get
            
            repEx.EndoscopyDate = examination.EndoscopyDate;
            repEx.ReportDate = examination.ReportDate;
            //repEx.RepPatientID = examination.Patient.ID;
            repEx.Device = examination.Device;
            repEx.Doctor = examination.Doctor;
            repEx.Dept = examination.Dept;
            repEx.Premedication = examination.Premedication;
            repEx.ReportText = examination.ReportText;
            repEx.DiagnosesText = examination.DiagnosesText;
            
            // need to set gender text (the code is written to DB and hence examination object)

            switch (examination.Patient.Gender)
            {
                case "m":
                    repEx.RepGender = "Male";
                    break;
                case "f":
                    repEx.RepGender = "Female";
                    break;
                case "i":
                    repEx.RepGender = "Indeterminate";
                    break;
            }

            comboBox1.SelectedItem = examination.Signout1;
            comboBox2.SelectedItem = examination.Signout2;
            comboBox3.SelectedItem = examination.Signout3;
            comboBox4.SelectedItem = examination.Signout4;
        
        }

        private void cmd_Print_Click(object sender, EventArgs e)
        {
            SignoutEndoscopist endo1 = comboBox1.SelectedItem as SignoutEndoscopist;
            SignoutEndoscopist endo2 = comboBox2.SelectedItem as SignoutEndoscopist;
            SignoutEndoscopist endo3 = comboBox3.SelectedItem as SignoutEndoscopist;
            SignoutEndoscopist endo4 = comboBox4.SelectedItem as SignoutEndoscopist;
            
            //Here do the check for null/duplicate endoscopists
            if (IsEmptySignout(endo1) && IsEmptySignout(endo2) && IsEmptySignout(endo3) && IsEmptySignout(endo4))
            {
                MessageBox.Show(this, "Please select at least one signout endoscopist.");
                return;
            }

            if (DuplicateSignout(endo1, endo2) || DuplicateSignout(endo1, endo3)
                || DuplicateSignout(endo1, endo4) || DuplicateSignout(endo2, endo3)
                || DuplicateSignout(endo2, endo4) || DuplicateSignout(endo3, endo4))
            {
                MessageBox.Show(this, "Duplicate signout endoscopist selected.");
                return;
            }

            ITransaction trans = session.BeginTransaction();

            examination.Signout1 = endo1;
            examination.Signout2 = endo2;
            examination.Signout3 = endo3;
            examination.Signout4 = endo4;

            if (examination.LogText != null)
                examination.LogText += "\r\n";
            

            if (examination.Signout1 != null)
            {
                repEx.RepSignoutText1 = examination.Signout1.Name;
                examination.LogText += examination.Signout1.ID;
            }

            if (examination.Signout2 != null)
            {
                repEx.RepSignoutText2 = examination.Signout2.Name;
                examination.LogText += "," + examination.Signout2.ID;
            }
            if (examination.Signout3 != null)
            {
                repEx.RepSignoutText3 = examination.Signout3.Name;
                examination.LogText += "," + examination.Signout3.ID;
            }
            if (examination.Signout4 != null)
            {
                repEx.RepSignoutText4 = examination.Signout4.Name;
                examination.LogText += "," + examination.Signout4.ID;
            }

            examination.LogText += "@" + DateTime.Now.ToShortDateString();

            if (!examination.ReportDate.HasValue)
            {
                //Set to now only if it has not been set in form
                examination.ReportDate = DateTime.Now;
                repEx.ReportDate = DateTime.Now;
            }

            session.Save(examination);
            trans.Commit();

            ReportForm frmRpt = new ReportForm(repEx);
            frmRpt.Closed += Reopen;
            Hide();
            frmRpt.Show();
        }

        private bool IsEmptySignout(SignoutEndoscopist endo)
        {
            return endo == null || string.IsNullOrEmpty(endo.Name);
        }

        private bool DuplicateSignout(SignoutEndoscopist endo1, SignoutEndoscopist endo2)
        {
            if (!IsEmptySignout(endo1) && !IsEmptySignout(endo2))
            {
                return endo1.Equals(endo2);
            }
            return false;
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void Reopen(object sender, EventArgs e)
        {
            Show();
        }
    }
}