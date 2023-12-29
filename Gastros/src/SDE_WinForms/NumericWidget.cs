using System;
using System.ComponentModel;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;

namespace GastrOs.Sde.Views.WinForms
{
    public class NumericWidget : ElementWidgetBase, INumericView
    {
        public event EventHandler ValueChanged;
        
        private NumericUpDown numericField;

        public NumericWidget()
        {
            numericField = new NumericUpDown {Minimum = decimal.MinValue, Maximum = decimal.MaxValue};
            numericField.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            numericField.ValueChanged += RelayValueChanged;
            numericField.TextChanged += HandleTextChange;
            numericField.Width = 50; //CFG
            numericField.MouseWheel += SuppressMouseWheel;
            AddField(numericField, 0, 100, SizeType.Percent);
        }
        
        public override object Value
        {
            get
            {
                //if text is empty, means null
                if (string.IsNullOrEmpty(numericField.Text))
                    return null;
                return numericField.Value;
            }
            set
            {
                decimal decValue = Convert.ToDecimal(value);

                if (Equals(Value, decValue))
                    return;
                //if null then set text to empty
                if (decValue == 0)
                {
                    //"silently" update text to avoid another valuechanged event being thrown
                    numericField.TextChanged -= HandleTextChange;
                    numericField.Text = "";
                    numericField.TextChanged += HandleTextChange;
                }
                else
                {
                    if (decValue > MaxValue || decValue < MinValue)
                        return;
                    numericField.TextChanged -= HandleTextChange;
                    numericField.Value = decValue;
                    //Hack? Sometimes the above doesn't force text to change...
                    numericField.Text = decValue.ToString();
                    numericField.TextChanged += HandleTextChange;
                }
                OnValueChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event. Also raises
        /// <see cref="WidgetBase.PropertyChanged"/> event on the property "Value".
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        }

        private void RelayValueChanged(object sender, EventArgs e)
        {
            OnValueChanged(EventArgs.Empty);
        }

        private void HandleTextChange(object sender, EventArgs e)
        {
            //Means text has changed from 'something' to 'empty'. Semantically, it means
            //'some value' to 'null'. So fire a value-changed event
            if (string.IsNullOrEmpty(numericField.Text))
                OnValueChanged(EventArgs.Empty);
        }

        public decimal MinValue
        {
            get { return numericField.Minimum; }
            set { numericField.Minimum = value; }
        }

        public decimal MaxValue
        {
            get { return numericField.Maximum; }
            set { numericField.Maximum = value; }
        }

        protected override int FieldCount
        {
            get { return 1; }
        }

        public override void Reset()
        {
            numericField.ResetText();
        }
    }
}