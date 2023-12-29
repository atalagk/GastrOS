using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.DataObjects;
using NHibernate;
using NHibernate.Cfg;

namespace GastrOs.Wrapper.Forms.Patient
{
    public partial class EditPatientDialog : Form
    {
        private PatientView patientView;
        private ClinicalView clinicalView;
        
        private DataObjects.Patient patient;
        private ISession session;

        public EditPatientDialog() : this(new DataObjects.Patient(),
                                          new Configuration().Configure().BuildSessionFactory().OpenSession()) { }

        public EditPatientDialog(DataObjects.Patient patient, ISession session)
        {
            this.patient = patient;
            this.session = session;

            if (patient.Clinical == null)
                patient.Clinical = new Clinical();

            IList<Insurer> insurers = session.CreateQuery("from Insurer").List<Insurer>();

            patientView = new PatientView(patient, insurers);
            patientView.Dock = DockStyle.Fill;
            clinicalView = new ClinicalView(patient.Clinical);
            clinicalView.Dock = DockStyle.Fill;

            InitializeComponent();

            groupBox1.Controls.Add(patientView);
            groupBox2.Controls.Add(clinicalView);

            Load += delegate { CenterToScreen(); };
        }

        public DataObjects.Patient EditingPatient
        {
            get { return patient; }
        }

        public bool Saved
        {
            get; private set;
        }

        private void HandleSave(object sender, EventArgs e)
        {
            string invalidProp, invalidDetails;
            bool validFields = patient.Validate(out invalidProp, out invalidDetails);
            if (!validFields)
            {
                MessageBox.Show(this, invalidDetails, "Missing or invalid details");
                return;
            }

            //Make sure no duplicate patient no. in case of new patient
            if (!session.Contains(patient))
            {
                IQuery duplicateCheck = session.CreateQuery("from Patient where PatientNo = " + patient.PatientNo);
                if (duplicateCheck.List().Count > 0)
                {
                    MessageBox.Show(this, "Patient with number '" + patient.PatientNo + "' already exists.",
                                    "Invalid patient number");
                    return;
                }
            }

            ITransaction trans = session.BeginTransaction();
            try
            {
                Saved = true;

                session.SaveOrUpdate(patient);
                session.SaveOrUpdate(patient.Clinical);
                trans.Commit();
                Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Problem saving patient information. " + ex.Message, "Saving error");
                Logger.Error("Patient saving error (id " + patient.ID + ")", ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Dispose();
        }
    }
}