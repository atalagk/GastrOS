using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Engine;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Views.WinForms.Support;
using OpenEhr.RM.DataTypes.Basic;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Provides a base class for a widget that contains a label and a set of
    /// input fields. Suitable for representing OpenEHR Elements.
    /// </summary>
    public abstract class ElementWidgetBase : WidgetBase, IScalarView
    {
        private TableLayoutPanel basePanel;

        private Label normalTitleLabel;
        private LinkLabel linkTitleLabel;

        protected Label TitleLabel
        {
            get;
            private set;
        }

        private ContextMenuStrip instanceMenu;
        private ToolStripMenuItem addInstanceMenu;
        private ToolStripMenuItem removeInstanceMenu;

        private List<Control> inputFields;
        private bool showTitle = true;

        public ElementWidgetBase()
        {
            Initialise();

            SuspendLayout();

            inputFields = new List<Control>();

            basePanel = new TableLayoutPanel();
            basePanel.Padding = new Padding(0);
            basePanel.Margin = new Padding(0);
            basePanel.Dock = DockStyle.Fill;
            basePanel.RowCount = 1;
            basePanel.ColumnCount = 1 + FieldCount;
            basePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, GastrOsConfig.LayoutConfig.LabelWidth));
            for (int i = 1; i < basePanel.ColumnCount; i++ )
            {
                basePanel.ColumnStyles.Add(new ColumnStyle());
            }
            
            Controls.Add(basePanel);

            linkTitleLabel = new LinkLabel();
            linkTitleLabel.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            linkTitleLabel.TextAlign = ContentAlignment.TopRight;
            linkTitleLabel.Padding = new Padding(0, 3, 3, 0);
            linkTitleLabel.Margin = new Padding(0);
            linkTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            linkTitleLabel.LinkClicked += ShowPopup;
            
            normalTitleLabel = new Label();
            normalTitleLabel.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            normalTitleLabel.TextAlign = ContentAlignment.TopRight;
            normalTitleLabel.Padding = new Padding(0, 3, 3, 0);
            normalTitleLabel.Margin = new Padding(0);
            normalTitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            linkTitleLabel.TextChanged += delegate { normalTitleLabel.Text = linkTitleLabel.Text; };
            normalTitleLabel.TextChanged += delegate { linkTitleLabel.Text = normalTitleLabel.Text; };

            InitialiseMenus();

            //This will appropriately add the right title label to the left hand side,
            //as well as enable/disable menu items for adding/removing instance
            OnCanAddRemoveInstanceChange();

            ResumeLayout(true);
        }
        
        private void InitialiseMenus()
        {
            instanceMenu = new ContextMenuStrip();
            addInstanceMenu = new ToolStripMenuItem();
            removeInstanceMenu = new ToolStripMenuItem();
            instanceMenu.SuspendLayout();

            instanceMenu.Items.AddRange(new ToolStripItem[]
                                            {
                                                addInstanceMenu,
                                                removeInstanceMenu
                                            });
            instanceMenu.Name = "instanceMenu";
            instanceMenu.Size = new Size(153, 70);

            addInstanceMenu.Name = "addInstanceMenu";
            addInstanceMenu.Size = new Size(152, 22);
            addInstanceMenu.Text = "Add another...";

            removeInstanceMenu.Name = "removeInstanceMenu";
            removeInstanceMenu.Size = new Size(152, 22);
            removeInstanceMenu.Text = "Remove this...";

            addInstanceMenu.Click += delegate { OnNewInstanceRequest(EventArgs.Empty); };
            removeInstanceMenu.Click += delegate { OnRemoveInstanceRequest(EventArgs.Empty); };

            instanceMenu.ResumeLayout(false);
        }

        private void ShowPopup(object sender, LinkLabelLinkClickedEventArgs e)
        {
            instanceMenu.Show(TitleLabel, new Point(TitleLabel.Right - 7, TitleLabel.Top + 5));
        }

        public override string Title
        {
            get { return TitleLabel.Text; }
            set
            {
                if (string.Equals(Title, value))
                    return;
                TitleLabel.Text = value;
                TitleLabel.Size = TitleLabel.GetSizeToFitText(GastrOsConfig.LayoutConfig.LabelWidth - 3);

                //update location of fields to be vertically centred
                foreach (Control inputField in inputFields)
                {
                    AdjustVertically(inputField);
                }

                //Notify listeners of change
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public override bool ShowTitle
        {
            get
            {
                return showTitle;
            }
            set
            {
                showTitle = value;
                normalTitleLabel.Visible = showTitle;
                linkTitleLabel.Visible = showTitle;
                //TODO A little hack to adjust the left margin for input controls
                basePanel.ColumnStyles[0].Width = GastrOsConfig.LayoutConfig.LabelWidth / (showTitle ? 1 : 2);
            }
        }

        public override string FurtherInformation
        {
            get
            {
                return GetToolTip(TitleLabel);
            }
            set
            {
                SetToolTip(TitleLabel, value);
                //TitleLabel.Font = new Font(TitleLabel.Font, string.IsNullOrEmpty(value) ? FontStyle.Regular : FontStyle.Italic);
            }
        }

        /// <summary>
        /// To be called by subclass to add specific input fields to this widget.
        /// </summary>
        /// <param name="field">the input field to add</param>
        /// <param name="index">the horizontal index (zero-based) at which the field should be placed.
        /// MUST be less than <see cref="FieldCount"/></param>
        /// <param name="width">the width of the field</param>
        /// <param name="sizeType">indicates how to interpret width (as absolute, percentage or auto-Size;
        /// in the lattermost case width is ignored)</param>
        protected void AddField(Control field, int index, float width, SizeType sizeType)
        {
            Check.Require(index < FieldCount);
            basePanel.ColumnStyles[index + 1] = new ColumnStyle(sizeType, width);
            field.Anchor = AnchorStyles.Left;
            basePanel.Controls.Add(field, index + 1, 0);
            field.Margin = GastrOsConfig.LayoutConfig.DefaultFieldMargin.Value;
            inputFields.Add(field);
            AdjustVertically(field);
        }

        protected void RemoveField(Control field)
        {
            basePanel.Controls.Remove(field);
            inputFields.Remove(field);
        }

        private void AdjustVertically(Control inputField)
        {
//            int idealHeight = IdealSize.Height;
//            int heightOffset = Math.Max(0, (idealHeight - inputField.Height) / 2);
//            inputField.Location += new Size(0, heightOffset);
        }

        /// <summary>
        /// Can be modified by subclass if need be
        /// </summary>
        protected virtual int InputMinHeight
        {
            get
            {
                return 24;
            }
        }

        /// <summary>
        /// Called at the very beginning of the super-constructor, and hence gives
        /// subclasses an opportunity to initialise any variables (for example
        /// determining field count dynamically) before the rest of the super-constructor
        /// is executed.
        /// </summary>
        protected virtual void Initialise() { }
        
        /// <summary>
        /// The number of input fields this widget should have.
        /// </summary>
        protected abstract int FieldCount
        {
            get;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(GastrOsConfig.LayoutConfig.LabelWidth + GastrOsConfig.LayoutConfig.InputWidth, 40); //CFG
            }
        }

        public override Size IdealSize
        {
            get
            {
                return new Size(GastrOsConfig.LayoutConfig.LabelWidth + GastrOsConfig.LayoutConfig.InputWidth, Math.Max(TitleLabel.Height, InputMinHeight));
            }
        }

        protected override sealed void OnCanAddRemoveInstanceChange()
        {
            //update title label and context menu items based on whether
            //new instances can be added and this instance can be removed
            Label appropriateLabel = CanAddNewInstance || CanRemoveInstance ? linkTitleLabel : normalTitleLabel;
            //remove and add appropriate label if different from existing label
            if (appropriateLabel != TitleLabel)
            {
                if (TitleLabel != null)
                    basePanel.Controls.Remove(TitleLabel);
                TitleLabel = appropriateLabel;
                basePanel.Controls.Add(TitleLabel, 0, 0);
            }
            //update context menus
            addInstanceMenu.Enabled = CanAddNewInstance;
            removeInstanceMenu.Enabled = CanRemoveInstance;
        }

        /// <summary>
        /// Basically intercepts mouse wheel events on the combo box and suppresses it.
        /// Then sneakily sets the focus on the closest available container, so that
        /// it can respond to subsequence mouse wheel events :)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SuppressMouseWheel(object sender, MouseEventArgs e)
        {
            HandledMouseEventArgs he = e as HandledMouseEventArgs;
            if (he != null)
            {
                he.Handled = true;
                //A little trick for setting focus on the nearest available container
                for (Control focusTarget = this; focusTarget != null && !focusTarget.Focus(); focusTarget = focusTarget.Parent) { }
            }
        }

        public IDataValueProvider DataValueProvider { get; set; }
        public abstract object Value { get; set; }
    }
}