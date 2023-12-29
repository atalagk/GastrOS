using System;
using System.ComponentModel;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows forms based text view implementation
    /// </summary>
    public class TextWidget : ElementWidgetBase, ITextView
    {
        protected TextBox textField;

        public TextWidget()
        {
            textField = new TextBox();
            textField.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            textField.TextChanged += delegate { OnTextChanged(EventArgs.Empty); };
            textField.Dock = DockStyle.Fill;
            textField.Width = GastrOsConfig.LayoutConfig.InputWidth - 5;
            AddField(textField, 0, 100, SizeType.Percent);
        }

        protected override int FieldCount
        {
            get { return 1; }
        }

        public override string Text
        {
            get
            {
                return textField.Text;
            }
            set
            {
                if (string.Equals(Text, value))
                    return;
                textField.Text = value;
                //fire events as per interface contract
                OnTextChanged(EventArgs.Empty);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs("Text"));
        }

        public override void Reset()
        {
            textField.ResetText();
        }
    }
}