using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GastrOs.Wrapper.DataObjects;

namespace GastrOs.Wrapper.Forms.Patient
{
    public partial class PatientView : UserControl
    {
        private DataObjects.Patient patient;
        private IList<Insurer> insurers;

        public PatientView(): this(new DataObjects.Patient(), new List<Insurer>()) { }

        public PatientView(DataObjects.Patient patient, IList<Insurer> insurers)
        {
            this.patient = patient;
            this.insurers = insurers;

            InitializeComponent();
        }

        public DataObjects.Patient Patient
        {
            get { return patient; }
            set
            {
                if (patient == value)
                    return;
                patient = value;
                patientBindingSource.Clear();
                patientBindingSource.Add(patient);
                patientBindingSource.MoveFirst();
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            patientBindingSource.Add(patient);
            patientBindingSource.MoveFirst();

            dobChooser.MaxDate = DateTime.Now;
            
            genderChooser.DataSource = Gender.GenderSet;
            genderChooser.ValueMember = "Code";
            genderChooser.DisplayMember = "Text";

            insurerChooser.DataSource = insurers;
            insurerChooser.ValueMember = "This";
            insurerChooser.DisplayMember = "Name";
        }
    }
}