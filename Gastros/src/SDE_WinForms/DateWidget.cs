using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows forms based text view implementation
    /// </summary>
    public class DateWidget : ElementWidgetBase, IDateView
    {
        public event EventHandler DateChanged;
        private const string DateFormat = "dd/MM/yyyy hh:mm:ss";

        protected TextBox textField;
        private DateTime? dateValue;

        public DateWidget()
        {
            textField = new TextBox();
            textField.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            textField.TextChanged += handleTextChanged;
            textField.Dock = DockStyle.Fill;
            textField.Width = GastrOsConfig.LayoutConfig.InputWidth - 5;
            AddField(textField, 0, 100, SizeType.Percent);
        }

        protected override int FieldCount
        {
            get { return 1; }
        }

        public DateTime? Date
        {
            get
            {
                return dateValue;
            }
            set
            {
                if (Equals(dateValue, value))
                    return;
                dateValue = value;
                //Silently update the text field
                textField.TextChanged -= handleTextChanged;
                textField.Text = dateValue.HasValue ? dateValue.Value.ToString(DateFormat) : null;
                textField.TextChanged += handleTextChanged;
                //fire events as per interface contract
                OnDateChanged(EventArgs.Empty);
            }
        }

        public override object Value
        {
            get
            {
                return Date;
            }
            set
            {
                Date = value as DateTime?;
            }
        }

        private void handleTextChanged(object sender, EventArgs e)
        {
            DateTime dt;
            if (DateTime.TryParse(textField.Text, out dt))
            {
                ForeColor = SystemColors.ControlText;
                if (!Equals(dateValue, dt))
                {
                    dateValue = dt;
                    OnDateChanged(EventArgs.Empty);
                }
            }
            else
            {
                ForeColor = Color.Red;
            }
        }

        protected void OnDateChanged(EventArgs e)
        {
            if (DateChanged != null)
                DateChanged(this, EventArgs.Empty);
            OnPropertyChanged(new PropertyChangedEventArgs("Text"));
        }

        public override void Reset()
        {
            textField.ResetText();
        }
    }
}
