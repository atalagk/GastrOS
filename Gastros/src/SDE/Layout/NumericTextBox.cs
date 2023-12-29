using System;
using System.Globalization;
using System.Windows.Forms;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    /// <summary>
    /// A custom numeric text box that restricts input to numeric text.
    /// Updates value when text box loses focus.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        private bool allowDecimals = true;
        private bool allowNegatives = true;

        public decimal? decimalValue;
        public event EventHandler ValueChanged;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo formatInfo = CultureInfo.CurrentCulture.NumberFormat;
            //Allow input of digits (of course!)
            if (Char.IsDigit(e.KeyChar))
                return;
            //Allow backspaces
            if (e.KeyChar == '\b')
                return;
            string keyStr = e.KeyChar.ToString();
            //Optionally allow decimal signs
            if (AllowDecimals && keyStr.Equals(formatInfo.NumberDecimalSeparator))
                return;
            //Optionally allow negative signs
            if (AllowNegatives && keyStr.Equals(formatInfo.NegativeSign))
                return;

            //Otherwise cancel key press
            e.Handled = true;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            TryParseText();
        }

        private void TryParseText()
        {
            NumberFormatInfo formatInfo = CultureInfo.CurrentCulture.NumberFormat;
            //setup the exact expected number style based on current configuration
            NumberStyles style = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
            if (AllowDecimals)
                style |= NumberStyles.AllowDecimalPoint;
            if (AllowNegatives)
                style |= NumberStyles.AllowLeadingSign;

            decimal result;
            //try parsing what's there in the text box
            if (decimal.TryParse(Text, style, formatInfo, out result))
            {
                Value = result;
            }
            Value = null;
        }

        public decimal? Value
        {
            get
            {
                return decimalValue;
            }
            set
            {
                if (decimalValue == value)
                    return;
                decimalValue = value;
                Text = value.ToString();
                OnValueChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        public bool AllowNegatives
        {
            get { return allowNegatives; }
            set { allowNegatives = value; }
        }

        public bool AllowDecimals
        {
            get { return allowDecimals; }
            set { allowDecimals = value; }
        }
    }
}