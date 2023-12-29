using System;
using System.Windows.Forms;
using GastrOs.Wrapper.DataObjects;

namespace GastrOs.Wrapper.Forms.Patient
{
    public partial class ClinicalView : UserControl
    {
        private Clinical clinical;

        public ClinicalView() : this(new Clinical()) { }

        public ClinicalView(Clinical clinical)
        {
            this.clinical = clinical;

            InitializeComponent();
        }

        private void ClinicalView_Load(object sender, EventArgs e)
        {
            clinicalBindingSource.Add(clinical);
            clinicalBindingSource.MoveFirst();
        }

        public Clinical Clinical
        {
            get { return clinical; }
        }
    }
}