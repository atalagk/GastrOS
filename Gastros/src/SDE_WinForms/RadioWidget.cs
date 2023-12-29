using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms
{
    public class RadioWidget : ElementWidgetBase, IListView
    {
        private FlowLayoutPanel basePanel;
        private List<RadioButton> choices;

        private ShowValueContextMode showValueContext;
        private IList<OntologyItem> choiceList;
        
        public RadioWidget()
        {
            basePanel = new FlowLayoutPanel();

            basePanel.Dock = DockStyle.Fill;
            AddField(basePanel, 0, 100, SizeType.Percent);
            basePanel.WrapContents = true;

            choices = new List<RadioButton>();
        }

        protected override int FieldCount
        {
            get { return 1; }
        }

        protected override int InputMinHeight
        {
            get
            {
                //Basically as high as its needs to accommodate all radio buttons
                return
                    basePanel.GetPreferredSize(new Size(GastrOsConfig.LayoutConfig.InputWidth, base.InputMinHeight)).
                        Height;
            }
        }

        /// <summary>
        /// Getting text returns the at-code associated with the currently selected
        /// radio button. Setting text results in the radio button with a matching
        /// at-code being selected.
        /// </summary>
        public override string Text
        {
            get
            {
                RadioButton selected = choices.FirstOrDefault(r => r.Checked);
                return selected != null ? selected.Name : RmFactory.DummyCodedValue;
            }
            set
            {
                if (string.Equals(Text, value))
                    return;
                RadioButton toSelect = choices.FirstOrDefault(r => r.Name == value);
                if (toSelect != null)
                {
                    toSelect.Checked = true; //implicitly fires TextChanged event
                }
                else //otherwise deselect all
                {
                    foreach (RadioButton deselect in choices)
                    {
                        deselect.Checked = false;
                    }
                    base.OnTextChanged(EventArgs.Empty);
                    OnPropertyChanged(new PropertyChangedEventArgs("Text"));
                }
            }
        }

        public override object Value
        {
            get { return Text; }
            set { Text = value as string; }
        }

        private void UpdateRadio()
        {
            foreach (OntologyItem item in choiceList)
            {
                if (string.Equals(item.ID, RmFactory.DummyCodedValue))
                    continue;
                RadioButton radio = new RadioButton();
                radio.Text = item.Text;
                radio.Name = item.ID; //Identify radio by at-code
                radio.CheckedChanged += handleRadioCheck;
                radio.Size = FormsHelper.GetSizeToFitText(radio, GastrOsConfig.LayoutConfig.InputWidth/2, 20);
                choices.Add(radio);
                basePanel.Controls.Add(radio);
            }
        }

        private void handleRadioCheck(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio == null) return;
            if (radio.Checked)
            {
                base.OnTextChanged(e);
                OnPropertyChanged(new PropertyChangedEventArgs("Text"));
            }
        }

        public override void Reset()
        {
            Text = RmFactory.DummyCodedValue;
        }

        public IList<OntologyItem> ChoiceList
        {
            get
            {
                return choiceList;
            }
            set
            {
                //TODO de-register listeners
                choiceList = value;
                UpdateRadio();
            }
        }

        //TODO Doesn't apply to radio at the moment
        public ShowValueContextMode ShowValueContext
        {
            get
            {
                return showValueContext;
            }
            set
            {
                showValueContext = value;
            }
        }
    }
}
