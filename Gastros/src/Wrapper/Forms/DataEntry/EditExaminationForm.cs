using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Views.WinForms.Support;
using GastrOs.Wrapper.DataObjects;
using GastrOs.Wrapper.Forms.Patient;
using GastrOs.Wrapper.Helpers;
using GastrOs.Wrapper.Reports;
using NHibernate;

namespace GastrOs.Wrapper.Forms.DataEntry
{
    public partial class EditExaminationForm : Form
    {
        private bool editable;
        private Examination examination;
        private ISession session;

        private PatientView patientView;
        private ClinicalView clinicalView;
        private ExaminationView examView;

        private EditExaminationForm() { }

        public EditExaminationForm(Examination exam, ISession session, IReportFormatter reportFormatter) : this(exam, session, reportFormatter, true) { }

        public EditExaminationForm(Examination exam, ISession session, IReportFormatter reportFormatter, bool editable)
        {
            examination = exam;
            this.session = session;

            InitializeComponent();

            IList<Insurer> insurers = session.CreateQuery("from " + typeof(Insurer).Name).List<Insurer>();
            
            patientView = new PatientView(examination.Patient, insurers);
            clinicalView = new ClinicalView(examination.Patient.Clinical);
            examView = new ExaminationView(examination, session, reportFormatter);
            examView.Size = new Size(792, 380);
            
            Editable = editable;

            Closing += HandleClose;
        }

        private void HandleClose(object sender, CancelEventArgs e)
        {
            if (session.IsDirty())
            {
                DialogResult result = MessageBox.Show(this, "Save changes before closing?", "Save?",
                                                      MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        PersistHelper.Save(this, examination, session);
                        break;
                    case DialogResult.No:
                        session.Refresh(examination);
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            patientView.Dock = DockStyle.Fill;
            clinicalView.Dock = DockStyle.Fill;

            patientBox.Controls.Add(patientView);
            clinicalBox.Controls.Add(clinicalView);
            mainInputPanel.Controls.Add(examView, 0, 2);
            txtNotes.Text = examination.Notes;  //koray check if it loads from DB into control properly

            CenterToScreen();
        }

        public bool Editable
        {
            get
            {
                return editable;
            }
            set
            {
                editable = value;
                btnToggleEdit.Text = editable ? "Save" : "Edit";
                patientView.SetEditable(editable);
                clinicalView.SetEditable(editable);
                examView.SetEditable(editable);
                txtNotes.SetEditable(editable);
            }
        }

        private void btnToggleEdit_Click(object sender, EventArgs e)
        {
            Editable = !Editable;
            if (!Editable)
            { // means save :)
                DoSave();
            }
        }

        private void DoSave()
        {
            examination.Notes = txtNotes.Text;

            PersistHelper.Save(this, examination, session);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (examination.EndoscopyType == null)
            {
                MessageBox.Show(this, "You must select an examination type first.", "Select examination type",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(examination.LogText))
                MessageBox.Show(this, "This record has been previously signed off.", "Notification", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            SignoutPrintForm sigPrFrm = new SignoutPrintForm(examination,session);
            sigPrFrm.Show();
        }
    }
}