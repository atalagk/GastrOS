using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Views.WinForms.Generic;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows forms representation of a multi-choice view. Each choice
    /// is represented as a checkbox.
    /// </summary>
    public class MultiChoiceWidget : WidgetBase, IMultiChoiceView
    {
        public const int MaxCheckboxWidth = 200; //CFG

        public event EventHandler<SelectionEventArgs> ItemSelectionChanged;
        
        private ToString displayFunction = obj => obj.ToString();
        private Dictionary<object, CheckBox> itemMap;
        private BoxPanel basePanel;
        private string title;
        private bool showTitle = true;
        private bool canSelectMore = true;

        public MultiChoiceWidget()
        {
            //item map stores the association between an item and the
            //checkbox that represents it
            itemMap = new Dictionary<object, CheckBox>();

            basePanel = new BoxPanel();
            basePanel.Margin = new Padding(0);
            basePanel.Dock = DockStyle.Fill;
            basePanel.AutoSize = true;
            
            Controls.Add(basePanel);
        }

        public override string Title
        {
            get { return title; }
            set
            {
                title = value;
                if (ShowTitle)
                    basePanel.Text = title;
                //Notify listeners of change
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public void SetDisplayFunction(ToString func)
        {
            displayFunction = func;
        }

        public bool AddSelectableItem(object item)
        {
            if (itemMap.ContainsKey(item))
                return false;

            CheckBox codeCheck = new CheckBox();
            codeCheck.Text = displayFunction(item);
            codeCheck.Name = item.ToString();
            codeCheck.Margin = new Padding(2, 1, 2, 1); //CFG
            codeCheck.CheckedChanged += RespondToSelection;
            codeCheck.KeyPress += RelayKeyPress;
            basePanel.AddChild(codeCheck);

            codeCheck.Size = codeCheck.GetSizeToFitText(MaxCheckboxWidth, 20); //CFG
            //A hack to ensure the checkbox has enough vertical space to accommodate
            //for >1-line text.... WinForms for the lose :(
            if (codeCheck.Height > 25)
                codeCheck.Size += new Size(0, 4);
            
            itemMap[item] = codeCheck;
            UpdateSelectability();

            return true;
        }

        public bool RemoveSelectableItem(object item)
        {
            if (itemMap.ContainsKey(item))
            {
                CheckBox checkBox = itemMap[item];
                itemMap.Remove(item);
                checkBox.CheckedChanged -= RespondToSelection;
                basePanel.RemoveChild(checkBox);
                return true;
            }
            return false;
        }

        public IEnumerable<object> SelectedItems
        {
            get
            {
                return itemMap.Keys.Where(item => itemMap[item].Checked);
            }
        }

        public bool SetSelected(object item, bool selected)
        {
            Check.Require(itemMap.ContainsKey(item));
            CheckBox checkBox = itemMap[item];
            if (checkBox.Checked == selected) //i.e. no change
                return false;
            //this will first trigger a CheckedChanged event, which will call back the method
            //RespondToSelection, which in turn will call the ItemSelectionChanged event
            checkBox.Checked = selected;
            return true;
        }

        public bool CanSelectMore
        {
            get { return canSelectMore; }
            set
            {
                if (canSelectMore == value)
                    return;
                canSelectMore = value;
                UpdateSelectability();
            }
        }

        public bool Framed
        {
            get { return basePanel.Framed; }
            set { basePanel.Framed = value; }
        }

        public bool ShowTitle
        {
            get { return showTitle; }
            set
            {
                showTitle = value;
                basePanel.Text = showTitle ? title : null;
            }
        }

        public override Size IdealSize
        {
            get
            {
                return basePanel.PreferredSize;
            }
        }

        private void UpdateSelectability()
        {
            foreach(CheckBox check in itemMap.Values)
            {
                if (!check.Checked)
                {
                    check.Enabled = canSelectMore;
                }
            }
        }

        private void RelayKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'a' || e.KeyChar == 'A')
            {
                CheckAll(true);
            }
            else if (e.KeyChar == 'n' || e.KeyChar == 'N')
            {
                CheckAll(false);
            }
            OnKeyPress(e);
        }

        private void CheckAll(bool isChecked)
        {
            foreach (CheckBox check in itemMap.Values)
            {
                if (check.Enabled && check.Checked != isChecked)
                    check.Checked = isChecked;
            }
        }

        /// <summary>
        /// Fire off the event!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RespondToSelection(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            Check.Invariant(check != null);
            //reverse-lookup the item associated with the checkbox
            object item = itemMap.Keys.FirstOrDefault(k => itemMap[k] == check);
            if (item != null)
                OnItemSelectionChanged(new SelectionEventArgs(item, check.Checked));
        }

        /// <summary>
        ///  Raises the <see cref="ItemSelectionChanged"/> event. Also raises
        /// <see cref="WidgetBase.PropertyChanged"/> event on the property "SelectedItems".
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnItemSelectionChanged(SelectionEventArgs e)
        {
            if (ItemSelectionChanged != null)
            {
                ItemSelectionChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
        }

        public override void Reset()
        {
            CheckAll(false);
            canSelectMore = true;
            UpdateSelectability();
        }
    }
}