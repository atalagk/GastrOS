using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GastrOs.Sde.Views.WinForms.Support
{
    public static class FormsHelper
    {
        /// <summary>
        /// Gets the ideal size of the control so that it can fit right around the size of
        /// specified text
        /// </summary>
        /// <param name="control">control to resize</param>
        /// <param name="maxWidth">the maximum width to which the control can be resized
        /// in order to accommodate its text</param>
        public static Size GetSizeToFitText(this Control control, int maxWidth)
        {
            return GetSizeToFitText(control, maxWidth, 0);
        }

        /// <summary>
        /// Gets the ideal size of the control so that it can fit right around the size of
        /// specified text
        /// </summary>
        /// <param name="control">control to resize</param>
        /// <param name="maxWidth">the maximum width to which the control can be resized
        /// in order to accommodate its text</param>
        /// <param name="nonTextWidth"></param>
        public static Size GetSizeToFitText(this Control control, int maxWidth, int nonTextWidth)
        {
            return GetSizeToFitText(control, maxWidth, nonTextWidth, control.Text);
        }

        /// <summary>
        /// Gets the ideal size of the control so that it can fit right around the size of
        /// specified text
        /// </summary>
        /// <param name="control"></param>
        /// <param name="maxWidth"></param>
        /// <param name="nonTextWidth"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Size GetSizeToFitText(this Control control, int maxWidth, int nonTextWidth, string text)
        {
            SizeF stringDim = control.CreateGraphics().MeasureString(text, control.Font,
                                                                     maxWidth - nonTextWidth - control.Padding.Horizontal);
            return new Size(Math.Min((int)Math.Ceiling(stringDim.Width) + nonTextWidth + control.Padding.Horizontal + 6, maxWidth),
                            (int) Math.Ceiling(stringDim.Height) + control.Padding.Vertical + 4);
        }

        /// <summary>
        /// Sets the width of the given combobox so that it just fits the
        /// display strings of its items. Can restrict the width to be one
        /// of a discrete given set of widths.
        /// </summary>
        /// <param name="combo">The combobox to resize</param>
        /// <param name="widths">discrete set of allowed widths for the combobox</param>
        public static void SetBestFitWidthForCombo(this ComboBox combo, params int[] widths)
        {
            if (combo.Items.Count < 1)
                return;

            int[] sortedWidths = new int[widths.Length];
            Array.Copy(widths, sortedWidths, widths.Length);
            Array.Sort(sortedWidths);

            //padding denotes the extra space needed for drop down items
            int padding = combo.Items.Count > combo.MaxDropDownItems
                              ? SystemInformation.VerticalScrollBarWidth
                              : 0;
            //work out the maximum display length of items
            int maxLength = 1;
            foreach (string displayItem in GetDisplayItems(combo))
            {
                int length = TextRenderer.MeasureText(displayItem, combo.Font).Width;
                maxLength = length > maxLength ? length : maxLength;
            }
            
            //if unrestricted, then resize combobox to fit max. display length
            if (sortedWidths.Length < 1)
            {
                combo.Width = maxLength + SystemInformation.VerticalScrollBarWidth;
                combo.DropDownWidth = maxLength + padding;
                return;
            }

            //Find the smallest restricted width that fits maximum display length
            for (int i = 0; i < sortedWidths.Length; i++)
            {
                int width = sortedWidths[i];
                if (width >= maxLength + SystemInformation.VerticalScrollBarWidth
                    || i >= sortedWidths.Length - 1)
                {
                    combo.Width = width;
                    combo.DropDownWidth = maxLength + padding;
                    return;
                }
            }
        }

        /// <summary>
        /// Gets an array containing the strings displayed in the given
        /// combo box
        /// 
        /// TODO robustness
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public static string[] GetDisplayItems(this ComboBox combo)
        {
            string[] displayItems = new string[combo.Items.Count];
            if (combo.DataSource == null)
            {
                //if combobox not bound to data source, then use the default
                //ToString() for each item
                for (int i = 0; i < displayItems.Length; i++)
                {
                    displayItems[i] = combo.Items[i].ToString();
                }
            }
            else
            {
                //otherwise, access the display member thru reflection
                for (int i = 0; i < displayItems.Length; i++)
                {
                    object item = combo.Items[i];
                    string display = string.IsNullOrEmpty(combo.DisplayMember)
                                         ? item.ToString()
                                         : item.GetType().InvokeMember(combo.DisplayMember,
                                                                       BindingFlags.GetProperty, null, item, null) as string;
                    displayItems[i] = display;
                }
            }
            return displayItems;
        }

        /// <summary>
        /// Returns the amount of space taken up by the frame by the given GroupBox
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Size GetFrameAllowance(this GroupBox box)
        {
            int width = box.Padding.Horizontal;
            int height = box.Padding.Vertical + box.Font.Height;
            return new Size(width, height);
        }

        public static Size ConvertZeroToUnbounded(Size size)
        {
            if (size.Width == 0)
            {
                size.Width = 0x7fffffff;
            }
            if (size.Height == 0)
            {
                size.Height = 0x7fffffff;
            }
            return size;
        }

        /// <summary>
        /// Sets the editable status to a control (and recursively to its children if applicable)
        /// </summary>
        /// <param name="control"></param>
        /// <param name="editable"></param>
        public static void SetEditable(this Control control, bool editable)
        {
            //base cases
            if (control is TextBoxBase)
            {
                (control as TextBoxBase).ReadOnly = !editable;
            }
            else if (control is ButtonBase)
            {
                (control as ButtonBase).Enabled = editable;
            }
            else if (control is LinkLabel)
            {
                (control as LinkLabel).Enabled = editable;
            }
            else if (control is UpDownBase)
            {
                //(control as UpDownBase).ReadOnly = !editable;
                (control as UpDownBase).Enabled = editable;
            }
            else if (control is ListControl)
            {
                (control as ListControl).Enabled = editable;
            }
            else if (control is DataGrid)
            {
                (control as DataGrid).ReadOnly = !editable;
            }
            else if (control is DateTimePicker)
            {
                (control as DateTimePicker).Enabled = editable;
            }
            else if (control is MonthCalendar)
            {
                (control as MonthCalendar).Enabled = editable;
            }
            else if (control is TabControl)
            {
                foreach (TabPage tab in (control as TabControl).TabPages)
                {
                    SetEditable(tab, editable);
                }
            }

            foreach (Control child in control.Controls)
            {
                SetEditable(child, editable);
            }
        }
    }
}