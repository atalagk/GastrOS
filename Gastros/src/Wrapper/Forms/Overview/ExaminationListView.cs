using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.DataObjects;
using NHibernate;

namespace GastrOs.Wrapper.Forms.Overview
{
    public class ExamEventArgs : EventArgs
    {
        public Examination Exam { get; private set; }
        public ExamEventArgs(Examination exam)
        {
            Exam = exam;
        }
    }

    public partial class ExaminationListView : UserControl
    {
        private DataObjects.Patient patient;
        private ISession session;

        public event EventHandler NewExam;
        public event EventHandler<ExamEventArgs> EditExam;
        public event EventHandler<ExamEventArgs> DeleteExam;

        private SearchDialog searchDialog;
//        private long? searchExamNo;

        private ExaminationListView() { }

        public ExaminationListView(DataObjects.Patient patient, ISession session)
        {
            this.session = session;
            this.patient = patient;

            //availableDepts = new List<Department>(session.CreateQuery("from Department").Enumerable<Department>());
            
            InitializeComponent();
            titleLabel.Text = "Examinations for " + patient.FullName + ":";
        }

        private void ExamsGridView_Load(object sender, EventArgs e)
        {
            examsGrid.AutoGenerateColumns = false;
            examsGrid.AutoSize = true;
            examsGrid.AllowUserToAddRows = false;
            examsGrid.MultiSelect = false;
            examsGrid.ReadOnly = true;
            //examsGrid.CellValueChanged += examsGrid_CellValueChanged;
            //examsGrid.DataError += dataError;
            examsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            RefreshView();

            btnEditExam.Enabled = examsGrid.SelectedRows.Count > 0;
            btnDeleteExam.Enabled = examsGrid.SelectedRows.Count > 0;

            InitialiseColumns();

            examsGrid.SelectionChanged += examsGrid_SelectionChanged;
            examsGrid.CellDoubleClick += examsGrid_CellDoubleClick;
        }

        public Examination SelectedExam
        {
            get
            {
                if (examsGrid.SelectedRows.Count > 0)
                    return examsGrid.SelectedRows[0].DataBoundItem as Examination;
                return null;
            }
        }

        public DataObjects.Patient Patient
        {
            get {
                return patient;
            }
            set
            {
                patient = value;
                RefreshView();
                titleLabel.Text = "Examinations for " + patient.FullName + ":";
            }
        }

        public void RefreshView()
        {
            session.Refresh(patient);
//            if (searchExamNo.HasValue)
//            {
//                //search for specific exam
//                examsGrid.DataSource =
//                    session.CreateQuery("from Examination where ID = " + searchExamNo).List<Examination>();
//            }
//            else
//            {
            examsGrid.DataSource = new List<Examination>(patient.Examinations);
//            }
        }

        private void InitialiseColumns()
        {
            AddTextColumn("ID", "ID", DataGridViewAutoSizeColumnMode.AllCells);
            AddTextColumn("Endoscopy type", "EndoscopyType", DataGridViewAutoSizeColumnMode.AllCells);
            AddTextColumn("Doctor", "Doctor", DataGridViewAutoSizeColumnMode.Fill);
            AddTextColumn("Report date", "ReportDate", DataGridViewAutoSizeColumnMode.AllCells);
            AddTextColumn("Endoscopy date", "EndoscopyDate", DataGridViewAutoSizeColumnMode.AllCells);
        }

        private void AddTextColumn(string header, string property, DataGridViewAutoSizeColumnMode autoSize)
        {
            DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
            idCol.HeaderText = header;
            idCol.DataPropertyName = property;
            idCol.AutoSizeMode = autoSize;
            idCol.FillWeight = 1;
            examsGrid.Columns.Add(idCol);
        }

        private void examsGrid_SelectionChanged(object sender, EventArgs e)
        {
            btnEditExam.Enabled = examsGrid.SelectedRows.Count > 0;
            btnDeleteExam.Enabled = examsGrid.SelectedRows.Count > 0;
        }

        private void btnNewExam_Click(object sender, EventArgs e)
        {
            if (NewExam != null) NewExam(this, EventArgs.Empty);
        }

        private void btnEditExam_Click(object sender, EventArgs e)
        {
            if (EditExam != null) EditExam(this, new ExamEventArgs(SelectedExam));
        }

        private void examsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (EditExam != null) EditExam(this, new ExamEventArgs(SelectedExam));
        }

        private void btnDeleteExam_Click(object sender, EventArgs e)
        {
            if (DeleteExam != null) DeleteExam(this, new ExamEventArgs(SelectedExam));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searchDialog == null)
            {
                searchDialog = new SearchDialog(null);
                searchDialog.SearchClick += searchExams;
            }
            searchDialog.Show(this);
        }

        private void searchExams(object sender, SearchEventArgs e)
        {
            //First of all, make sure search string is a valid number
            long searchId;
            if (long.TryParse(e.SearchString, out searchId))
            {
//                searchExamNo = parsed;
//                RefreshView();
                try
                {
                    //perform actual search
                    Examination match = session.Get<Examination>(searchId);
                    if (match != null)
                    {
                        searchDialog.Hide();
                        if (EditExam != null) EditExam(this, new ExamEventArgs(match));
                    }
                    else
                    {
                        searchDialog.AutoHide = false;
                        MessageBox.Show(searchDialog, "No matching examination found");
                        searchDialog.AutoHide = true;
                    }
                }
                catch (HibernateException ex)
                {
                    //This technically should not happen, since we're searching for primary key
                    Logger.Error(
                        "Searching examination by (supposed) primary key " + searchId +
                        " returned multiple results. Recheck schema!", ex);
                }
            }
//            else if (string.IsNullOrEmpty(e.SearchString))
//            { //if search box is empty, then treat it as "show all"
//                searchDialog.Hide();
//                searchExamNo = null;
//                RefreshView();
//            }
            else
            {
                searchDialog.AutoHide = false;
                MessageBox.Show(searchDialog, "Only numbers are allowed for searches");
                searchDialog.AutoHide = true;
            }
        }
    }
}