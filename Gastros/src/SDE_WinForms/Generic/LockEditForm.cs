using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    [Flags]
    public enum ButtonsConfig {None = 0x000, EditSave = 0x001, Delete = 0x010, Report = 0x100}

    /// <summary>
    /// A reusable form for housing controls that support a certain editing
    /// protocol used by the original GST. Basically the fields inside the
    /// control are only editable when the <see cref="Editing"/> switch is
    /// turned on.
    /// </summary>
    public class LockEditForm : Form
    {
        public event EventHandler Save;
        /// <summary>
        /// Handlers of this event can set the "Cancel" flag to true in order to prevent the
        /// form from being disposed.
        /// </summary>
        public event EventHandler<CancelEventArgs> Delete;
        /// <summary>
        /// Handlers of this event can set the "Cancel" flag to true in order to prevent the
        /// form from being disposed.
        /// </summary>
        public event EventHandler<CancelEventArgs> Report;

        private bool editing;
        private bool closeSilently;
        private TableLayoutPanel basePanel;
        private Button closeButton, editButton, deleteButton, reportButton;
        
        private Control content;
        private readonly ButtonsConfig buttonsConfig;
        private int extraButtonSlots = 2;

        public LockEditForm(Control content, bool autoScroll, ButtonsConfig buttonsConfig)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            this.content = content;
            this.buttonsConfig = buttonsConfig;

            basePanel = new TableLayoutPanel { RowCount = 2, ColumnCount = 7 };
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); //horizontal "glue"
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); //extra slot #1
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); //extra slot #2
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            
            basePanel.Dock = DockStyle.Fill;
            Controls.Add(basePanel);

            closeButton = new Button {Text = GuiDictionary.Lookup("okOption")};
            editButton = new Button { Text = GuiDictionary.Lookup("editOption") };
            deleteButton = new Button { Text = GuiDictionary.Lookup("deleteOption") };
            reportButton = new Button { Text = GuiDictionary.Lookup("reportOption") };
            deleteButton.Enabled = false;

            closeButton.Click += HandleCloseClick;
            editButton.Click += HandleEditSave;
            reportButton.Click += HandleReport;
            deleteButton.Click += HandleDelete;
            
            if ((buttonsConfig & ButtonsConfig.Delete) > 0)
                basePanel.Controls.Add(deleteButton, 0, 1);
            if ((buttonsConfig & ButtonsConfig.Report) > 0)
                basePanel.Controls.Add(reportButton, 4, 1);
            if ((buttonsConfig & ButtonsConfig.EditSave) > 0)
                basePanel.Controls.Add(editButton, 5, 1);
            basePanel.Controls.Add(closeButton, 6, 1);

            Load += delegate
                        {
                            InitContent(autoScroll);
                            UpdateViewEditable();
                            CenterToScreen();
                        };
        }

        protected override void Dispose(bool disposing)
        {
            content.Resize -= UpdateAutoscroll;
            closeButton.Click -= HandleCloseClick;
            editButton.Click -= HandleEditSave;
            reportButton.Click -= HandleReport;
            deleteButton.Click -= HandleDelete;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Override default closing behaviour - only raise the <see cref="Form.Closing"/>
        /// event if it's explicitly allowed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closeSilently)
            {
                base.OnClosing(e);
            }
        }

        private void InitContent(bool autoScroll)
        {
            content.AutoSize = true;
            content.Dock = DockStyle.Fill;

            //Basically set the control up so that it auto-scrolls within this form
            if (content is ScrollableControl)
            {
                if (autoScroll)
                {
                    (content as ScrollableControl).AutoScrollMinSize = content.PreferredSize;
                    content.Resize += UpdateAutoscroll; //temp
                }
                basePanel.Controls.Add(content, 0, 0);
                basePanel.SetColumnSpan(content, basePanel.ColumnCount);
            }
            else if (autoScroll)
            {
                Panel scroller = new Panel();
                scroller.Dock = DockStyle.Fill;
                scroller.AutoScrollMinSize = content.PreferredSize;
                scroller.Controls.Add(content);
                basePanel.Controls.Add(scroller, 0, 0);
                basePanel.SetColumnSpan(scroller, basePanel.ColumnCount);
            }
        }

        //temp
        void UpdateAutoscroll(object sender, EventArgs e)
        {
            (content as ScrollableControl).AutoScrollMinSize = content.PreferredSize;
        }

        public bool AddButton(Control button)
        {
            if (extraButtonSlots == 0)
                return false;
            basePanel.Controls.Add(button, 1 + extraButtonSlots, 1);
            extraButtonSlots--;
            return true;
        }

        public Control Content
        {
            get { return content; }
        }

        /// <summary>
        /// "Internally" closes this form - suppresses the <see cref="Form.Closing"/>
        /// event
        /// </summary>
        private void CloseInternally()
        {
            //translation: if edit button doesn't exist, then treat close as
            //"save AND close"
            if (Editing && (buttonsConfig & ButtonsConfig.EditSave) == 0)
            {
                OnSave();
            }
            closeSilently = true;
            Close();
        }

        private void HandleEditSave(object sender, EventArgs e)
        {
            Editing = !Editing;
            if (!Editing)
            { //means previously editing - means save
                Cursor prevCursor = Cursor;
                Cursor = Cursors.WaitCursor;
                OnSave();
                Cursor = prevCursor;
            }
        }

        private void HandleDelete(object sender, EventArgs e)
        {
            if (Delete != null)
            {
                CancelEventArgs args = new CancelEventArgs(false);
                //Raise event - the handlers of this event have the option of canceling the event
                Delete(this, args);
                if (!args.Cancel)
                    CloseInternally();
            }
        }

        private void HandleReport(object sender, EventArgs e)
        {
            if (Report != null)
            {
                CancelEventArgs args = new CancelEventArgs(false);
                //Raise event - the handlers of this event have the option of canceling the event
                Report(this, args);
                if (!args.Cancel)
                    CloseInternally();
            }
        }

        private void HandleCloseClick(object sender, EventArgs e)
        {
            CloseInternally();
        }

        private void OnSave()
        {
            if (Save != null)
            {
                Save(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets whether editing is allowed for the control
        /// </summary>
        public bool Editing
        {
            get{ return editing; }
            set
            {
                if (editing == value)
                    return;
                editing = value;
                UpdateViewEditable();
            }
        }

        private void UpdateViewEditable()
        {
            deleteButton.Enabled = editing;
            reportButton.Enabled = !editing;
            //translation: only display "Cancel" if the editing status is true AND
            //the edit button actually exists :)
            closeButton.Text = editing && (buttonsConfig & ButtonsConfig.EditSave) > 0
                                   ? GuiDictionary.Lookup("cancelOption")
                                   : GuiDictionary.Lookup("okOption");
            editButton.Text = editing ? GuiDictionary.Lookup("saveOption") : GuiDictionary.Lookup("editOption");
            content.SetEditable(editing);
        }

        protected override Size DefaultSize
        {
            get
            {
                return GastrOsConfig.LayoutConfig.DefaultFormSize.Value;
            }
        }
    }
}