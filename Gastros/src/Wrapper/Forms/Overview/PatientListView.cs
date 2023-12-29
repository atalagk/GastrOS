using System;
using System.Windows.Forms;
using NHibernate;

namespace GastrOs.Wrapper.Forms.Overview
{
    public partial class PatientListView : UserControl
    {
        private ISession session;

        public event EventHandler NewPatient;
        public event EventHandler EditPatient;
        public event EventHandler DeletePatient;
        public event EventHandler PatientSelectionChange;

        private SearchDialog searchDialog;
        private DataObjects.Patient lastSearchedPatient;

        private PatientListView() { }

        public PatientListView(ISession session)
        {
            this.session = session;

            InitializeComponent();
        }

        public DataObjects.Patient SelectedPatient
        {
            get
            {
                if (patientGrid.SelectedRows.Count > 0)
                    return patientGrid.SelectedRows[0].DataBoundItem as DataObjects.Patient;
                return null;
            }
        }

        private void PatientListView_Load(object sender, EventArgs e)
        {
            patientGrid.AutoGenerateColumns = false;
            patientGrid.AutoSize = true;
            patientGrid.AllowUserToAddRows = false;
            patientGrid.MultiSelect = false;
            patientGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            patientGrid.ReadOnly = true;
            patientGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            RefreshView();

            AddTextColumn("Patient No.", "PatientNo");
            AddTextColumn("Name", "FullName");
            AddTextColumn("Birthdate", "Birthdate");

            btnEdit.Enabled = patientGrid.SelectedRows.Count > 0;

            patientGrid.SelectionChanged += patientGrid_SelectionChanged;
        }

        public void RefreshView()
        {
            //TODO hmm, performance cost?? Possible to refresh list via hibernate?
            if (lastSearchedPatient != null)
            {
                patientGrid.DataSource = new [] {lastSearchedPatient};
            }
            else
            {
                patientGrid.DataSource = session.CreateQuery("from Patient").List<DataObjects.Patient>();
            }
        }

        private void AddTextColumn(string header, string property)
        {
            DataGridViewTextBoxColumn idCol = new DataGridViewTextBoxColumn();
            idCol.HeaderText = header;
            idCol.DataPropertyName = property;
            patientGrid.Columns.Add(idCol);
        }

        private void patientGrid_SelectionChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled = patientGrid.SelectedRows.Count > 0;

            if (PatientSelectionChange != null) PatientSelectionChange(sender, e);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (NewPatient != null) NewPatient(this, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (EditPatient != null) EditPatient(this, e);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (DeletePatient != null) DeletePatient(this, e);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searchDialog == null)
            {
                searchDialog = new SearchDialog(null);
                searchDialog.SearchClick += searchPatients;
            }
            searchDialog.Show(this);
        }

        private void searchPatients(object sender, SearchEventArgs e)
        {
            //First of all, make sure search string is a valid number
            int id;
            if (int.TryParse(e.SearchString, out id))
            {
                lastSearchedPatient = session.CreateQuery("from Patient where PatientNo=" + id).UniqueResult<DataObjects.Patient>();
                if (lastSearchedPatient != null)
                {
                    searchDialog.Hide();
                    RefreshView();
                }
                else
                {
                    searchDialog.AutoHide = false;
                    MessageBox.Show(searchDialog, "No matching patient found");
                    searchDialog.AutoHide = true;
                }
            }
            else if (string.IsNullOrEmpty(e.SearchString))
            { //if search box is empty, then treat it as "show all"
                searchDialog.Hide();
                lastSearchedPatient = null;
                RefreshView();
            }
            else
            {
                searchDialog.AutoHide = false;
                MessageBox.Show(searchDialog, "Only numbers are allowed for searches");
                searchDialog.AutoHide = true;
            }
        }
    }
}