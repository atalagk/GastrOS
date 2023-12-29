using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows form implementation of a quantity view, which in turn is an implementation
    /// of a numeric unit view. The magnitude of the quantity is displayed as a spinner
    /// text field (<see cref="NumericUpDown"/>) and the unit is displayed either as a
    /// label (in case of only one available unit) or a combo box (in case of multiple).
    /// </summary>
    public class QuantityWidget : ElementWidgetBase, INumericUnitView
    {
        public event EventHandler ValueChanged;
        public event EventHandler UnitChanged;

        private NumericUpDown numericField;
        private Control unitControl;

        public QuantityWidget()
        {
            numericField = new NumericUpDown { Minimum = decimal.MinValue, Maximum = decimal.MaxValue };
            numericField.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            numericField.DecimalPlaces = GastrOsConfig.LayoutConfig.QuantDecimalPlaces;
            numericField.ValueChanged += RelayValueChanged;
            numericField.TextChanged += HandleTextChange;
            numericField.MouseWheel += SuppressMouseWheel;
            AddField(numericField, 0, 50, SizeType.Percent);
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
                    //"silently" update text to avoid another decValuechanged event being thrown
                    numericField.TextChanged -= HandleTextChange;
                    numericField.Text = "";
                    numericField.TextChanged += HandleTextChange;
                }
                else
                {
                    if (decValue > MaxValue || decValue < MinValue)
                        return;
                    //This will implicitly throw decValuechanged event
                    numericField.Value = decValue;
                }
            }
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

        public string Unit
        {
            get
            {
                //Label = single unit; TextBox = unbounded unit
                if (unitControl is Label || unitControl is TextBox)
                    return unitControl.Text;
                //Fixed choice of units
                if (unitControl is ComboBox)
                    return ((ComboBox) unitControl).SelectedItem as string;
                return null;
            }
            set
            {
                if (string.Equals(Unit, value))
                    return;
                if (unitControl is Label || unitControl is TextBox)
                    unitControl.Text = value;
                else if (unitControl is ComboBox)
                    ((ComboBox) unitControl).SelectedItem = value;
                OnUnitChanged(EventArgs.Empty);
            }
        }

        public IList<string> AvailableUnits
        {
            get
            {
                if (unitControl is Label)
                    return new List<string> {unitControl.Text};
                if (unitControl is ComboBox)
                    return ((ComboBox) unitControl).Items.Cast<string>().ToList();
                if (unitControl is TextBox)
                    return null;
                return null;
            }
            /* Update the control for displaying/selecting units depending on the
             * available units */
            set
            {
                //remove old unit control, if exists
                if (unitControl != null)
                    RemoveField(unitControl);

                //Update the unit portion of this widget
                if (value == null || value.Count == 0)
                {
                    unitControl = new TextBox();
                    unitControl.Width = GastrOsConfig.LayoutConfig.InputWidth/3;
                    unitControl.TextChanged += delegate { OnUnitChanged(EventArgs.Empty); };
                }
                else if (value.Count == 1)
                {
                    unitControl = new Label {Text = value.First(), Padding = new Padding(0, 4, 0, 0)};
                }
                else
                {
                    unitControl = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                    ((ComboBox)unitControl).Items.AddRange(value.ToArray());
                    ((ComboBox) unitControl).SelectedValueChanged += delegate { OnUnitChanged(EventArgs.Empty); };
                }
                AddField(unitControl, 1, 50, SizeType.Percent);
            }
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

        /// <summary>
        /// Raises the <see cref="UnitChanged"/> event. Also raises
        /// <see cref="WidgetBase.PropertyChanged"/> event on the property "Unit".
        /// </summary>
        protected virtual void OnUnitChanged(EventArgs e)
        {
            if (UnitChanged != null)
            {
                UnitChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Unit"));
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

        protected override int FieldCount
        {
            get { return 2; }
        }

        public override void Reset()
        {
            numericField.ResetText();
            if (unitControl is TextBox)
            {
                unitControl.ResetText();
            }
            else if (unitControl is ComboBox)
            {
                //0 index is safe, because the fact that a combo box was generated for units means
                //there were at least 2 available units
                ((ComboBox) unitControl).SelectedIndex = 0;
            }
        }
    }
}