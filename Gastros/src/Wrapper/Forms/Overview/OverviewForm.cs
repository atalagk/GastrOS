using System;
using System.Windows.Forms;
using GastrOs.Wrapper.Reports;
using OpenEhr.DesignByContract;
using GastrOs.Wrapper.DataObjects;
using GastrOs.Wrapper.Forms.DataEntry;
using GastrOs.Wrapper.Forms.Patient;
using NHibernate;

namespace GastrOs.Wrapper.Forms.Overview
{
    public partial class OverviewForm : Form
    {
        private ISession session;
        private PatientListView patientListView;
        private ExaminationListView examsView;
        private IReportFormatter reportFormatter;

        private OverviewForm() { }

        public OverviewForm(ISession session, IReportFormatter reportFormatter)
        {
            this.session = session;
            this.reportFormatter = reportFormatter;

            InitializeComponent();
            CenterToScreen();

            patientListView = new PatientListView(session);
            patientListView.NewPatient += HandleNewPatient;
            patientListView.EditPatient += HandleEditPatient;
            patientListView.DeletePatient += HandleDeletePatient;
            patientListView.PatientSelectionChange += HandlePatientSelectionChange;

            patientListView.Dock = DockStyle.Fill;
            patientsContainer.Controls.Add(patientListView);
        }

        private void HandlePatientSelectionChange(object sender, EventArgs e)
        {
            DataObjects.Patient patient = patientListView.SelectedPatient;
            if (patient != null)
            {
                if (examsView == null)
                {
                    examsView = new ExaminationListView(patient, session);
                    examsView.Dock = DockStyle.Fill;
                    examsContainer.Controls.Add(examsView);

                    examsView.NewExam += examsView_NewExam;
                    examsView.EditExam += examsView_EditExam;
                    examsView.DeleteExam += examsView_DeleteExam;
                }
                else
                {
                    examsView.Patient = patient;
                }
            }

            if (examsView != null)
            {
                examsView.Enabled = patient != null;
            }
        }

        private void HandleEditPatient(object sender, EventArgs e)
        {
            DataObjects.Patient patient = patientListView.SelectedPatient;
            Check.Require(patient != null);
            
            OpenPatientDialog(patient);
        }

        private void HandleNewPatient(object sender, EventArgs e)
        {
            DataObjects.Patient patient = new DataObjects.Patient();
            bool saved = OpenPatientDialog(patient);
            //if new patient saved, then take user straight to examination form
            if (saved)
            {
                Examination exam = new Examination {Patient = patient};
                OpenMainForm(exam, true);
            }
        }

        private void examsView_EditExam(object sender, ExamEventArgs e)
        {
            Check.Require(e.Exam != null);
            OpenMainForm(e.Exam, false);
        }

        private void examsView_NewExam(object sender, EventArgs e)
        {
            DataObjects.Patient patient = patientListView.SelectedPatient;
            Check.Require(patient != null);

            Examination exam = new Examination {Patient = patient};
            OpenMainForm(exam, true);
        }

        void examsView_DeleteExam(object sender, ExamEventArgs e)
        {
            Check.Require(e.Exam != null);

            if (MessageBox.Show(this, "Are you sure you want to delete this examination?",
                                "Confirm delete examination", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            
            ITransaction trans = session.BeginTransaction();
            e.Exam.Patient.Examinations.Remove(e.Exam);
            e.Exam.Patient = null;
            //session.Save(exam.Patient);
            session.Delete(e.Exam);
            trans.Commit();
            examsView.RefreshView();
        }

        private void HandleDeletePatient(object sender, EventArgs e)
        {
            DataObjects.Patient patient = patientListView.SelectedPatient;
            Check.Require(patient != null);

            if (MessageBox.Show(this, "Are you sure you want to delete ALL records associated with " +
                                      "patient " + patient.FullName + "?",
                                "Confirm delete patient", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            ITransaction trans = session.BeginTransaction();
            foreach (Examination exam in patient.Examinations)
                exam.Patient = null;
            session.Delete(patient);
            trans.Commit();
            patientListView.RefreshView();
        }

        private bool OpenPatientDialog(DataObjects.Patient patient)
        {
            EditPatientDialog editDialog = new EditPatientDialog(patient, session);
            editDialog.ShowDialog(this);
            editDialog.Dispose();

            if (editDialog.Saved)
                patientListView.RefreshView();
            return editDialog.Saved;
        }

        private void OpenMainForm(Examination exam, bool editable)
        {
            EditExaminationForm form;
            try
            {
                form = new EditExaminationForm(exam, session, reportFormatter, editable);
            } catch(Exception e)
            {
                MessageBox.Show(this, "Examination form couldn't be created. " + e.Message);
                return;
            }

            form.Show();
            form.Closed += delegate
                               {
                                   Show();
                                   examsView.RefreshView();
                               };
            Hide();
        }
    }
}