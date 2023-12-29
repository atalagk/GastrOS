using System;
using System.Windows.Forms;

namespace GastrOs.Sde.Views.WinForms
{
    public class CheckboxWidget : ElementWidgetBase, ICheckView
    {
        public event EventHandler CheckedChanged;

        private CheckBox check;

        public CheckboxWidget()
        {
            check = new CheckBox();
            check.CheckedChanged += OnCheckedChanged;
            AddField(check, 0, 100, SizeType.Percent);
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(this, EventArgs.Empty);
        }

        public bool Checked
        {
            get
            {
                return check.Checked;
            }
            set
            {
                if (Checked == value)
                    return;
                check.Checked = value;
                OnCheckedChanged(this, EventArgs.Empty);
            }
        }

        public override object Value
        {
            get { return Checked; }
            set { Checked = Convert.ToBoolean(value); }
        }

        protected override int FieldCount
        {
            get { return 1; }
        }

        public override void Reset()
        {
            Checked = false;
        }
    }
}