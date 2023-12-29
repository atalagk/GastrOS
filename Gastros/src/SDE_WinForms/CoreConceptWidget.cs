using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Support;
using GastrOs.Sde.Views.WinForms.Generic;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Windows forms implementation of a core concept view. "Presence" is
    /// represented by a checkbox, which either shows or hides the child
    /// views depending on presence value.
    /// </summary>
    public class CoreConceptWidget : ContainerWidgetBase, ICoreConceptView
    {
        /// <summary>
        /// Event raised whenever the presence value (i.e. checked value) is changed
        /// either by user or internally
        /// </summary>
        public event EventHandler PresenceChanged;

        private static readonly string MenuLabelForAdd = GuiDictionary.Lookup("addInstance");
        private static readonly string MenuLabelForDelete = GuiDictionary.Lookup("deleteInstance");
        private static readonly string MenuLabelForClear = GuiDictionary.Lookup("clearInstance");

        private TableLayoutPanel basePanel;
        private MultiStateCheck presenceCheck;
        private Panel mainContentPanel, sideContentPanel;

        private ToolStripMenuItem newContextMenuItem, removeContextMenuItem;
        
        public CoreConceptWidget()
        {
            SuspendLayout();

            basePanel = new TableLayoutPanel();
            basePanel.Margin = new Padding(0);
            basePanel.RowCount = 2;
            basePanel.ColumnCount = 2;

            basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            basePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            basePanel.Dock = DockStyle.Fill;
            Controls.Add(basePanel);

            presenceCheck = new MultiStateCheck();
            presenceCheck.Font = GastrOsConfig.LayoutConfig.DefaultFont.Value;
            presenceCheck.StateChanged += delegate { OnPresenceChanged(EventArgs.Empty); };
            
            //Set up the context menu for checkbox
            newContextMenuItem = new ToolStripMenuItem(MenuLabelForAdd) {Size = new Size(152, 22)};
            removeContextMenuItem = new ToolStripMenuItem(MenuLabelForClear) { Size = new Size(152, 22) };
            presenceCheck.ContextMenuStrip.Items.Add(newContextMenuItem);
            presenceCheck.ContextMenuStrip.Items.Add(removeContextMenuItem);
            newContextMenuItem.Enabled = false;
            newContextMenuItem.Click += HandleNewInstanceRequest;
            removeContextMenuItem.Click += HandleDeleteInstanceRequest;

            basePanel.Controls.Add(presenceCheck, 0, 0);

            mainContentPanel = ConstructMainContentPanel();
            mainContentPanel.Dock = DockStyle.Fill;
            sideContentPanel = ConstructSideContentPanel();
            sideContentPanel.Dock = DockStyle.Fill;
            
            basePanel.SetColumnSpan(mainContentPanel, 2);
            basePanel.Controls.Add(mainContentPanel, 0, 1);
            basePanel.Controls.Add(sideContentPanel, 1, 0);

            UpdateContextMenu();

            ResumeLayout(false);
        }

        public override string Title
        {
            get
            {
                return presenceCheck.Text;
            }
            set
            {
                if (string.Equals(Title, value))
                    return;
                presenceCheck.Text = value;
                //Notify listeners of change as per interface contract
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public override string FurtherInformation
        {
            get
            {
                return presenceCheck.ToolTip;
            }
            set
            {
                presenceCheck.ToolTip = value;
            }
        }

        public void SetAvailablePresenceStates(PresenceState states)
        {
            presenceCheck.SetAvailableStates(states);
        }

        public PresenceState Presence
        {
            get
            {
                return presenceCheck.State;
            }
            set
            {
                if (Presence == value)
                    return;
                //This will implicitly notify listeners
                presenceCheck.State = value;
            }
        }

        protected override void AddChildView(IView child, int index)
        {
            Control childControl = child as Control;
            
            //search for the "showAithParent" directive
            bool addToSide = child.Directives.HasDirectiveOfType<ShowWithParentDirective>();
            ControlCollection controls = addToSide ? sideContentPanel.Controls : mainContentPanel.Controls;
            childControl.Margin = GastrOsConfig.LayoutConfig.DefaultMargin.Value;
            controls.Add(childControl);
            controls.SetChildIndex(childControl, index);
            UpdateChildrenAccordingToPresence();
        }

        protected override void RemoveChildView(IView child)
        {
            Control childControl = child as Control;
            mainContentPanel.Controls.Remove(childControl);
        }

        /// <summary>
        /// Fires the <see cref="IView.NewInstanceRequest"/> event so controller
        /// can deal with it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleNewInstanceRequest(object sender, EventArgs e)
        {
            OnNewInstanceRequest(e);
        }

        /// <summary>
        /// If there is another instance of the concept represented by this widget,
        /// then fires the <see cref="IView.NewInstanceRequest"/> event so controller
        /// can remove this. Otherwise simply clears the contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDeleteInstanceRequest(object sender, EventArgs e)
        {
            if (CanRemoveInstance)
            {
                OnRemoveInstanceRequest(e);
            }
            else
            {
                //This will trigger a PresenceChanged event, thereby implicitly invoking
                //the OnPresenceChanged() method below.
                presenceCheck.State = PresenceState.Null;
            }
        }

        /// <summary>
        /// This method is called whenever the can-add/remove-instance status
        /// of this widget changes. Subclasses can specify any further behaviour
        /// that depends on this status (for example enabling/disabling menu
        /// items for adding/removing instances)
        /// </summary>
        protected override void OnCanAddRemoveInstanceChange()
        {
            UpdateContextMenu();
        }

        /// <summary>
        /// Update the menu items depending on existing cardinality 
        /// For "add" menu, either "enable" or "disable"
        /// For "remove" menu, set text to either "delete" or "clear"
        /// </summary>
        private void UpdateContextMenu()
        {
            newContextMenuItem.Enabled = CanAddNewInstance && presenceCheck.State != PresenceState.Null;
            removeContextMenuItem.Enabled = CanRemoveInstance || presenceCheck.State != PresenceState.Null;
            removeContextMenuItem.Text = CanRemoveInstance ? MenuLabelForDelete : MenuLabelForClear;
        }

        /// <summary>
        /// Raises the <see cref="PresenceChanged"/> event. Also raises
        /// <see cref="WidgetBase.PropertyChanged"/> event on the property "Present".
        /// </summary>
        protected virtual void OnPresenceChanged(EventArgs e)
        {
            UpdateChildrenAccordingToPresence();
            UpdateContextMenu();

            if (PresenceChanged != null)
            {
                PresenceChanged(this, e);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Present"));
        }

        /// <summary>
        /// Make sure children are shown/hidden/reset according to presence
        /// </summary>
        private void UpdateChildrenAccordingToPresence()
        {
            foreach (IView child in Children)
            {
                if (Presence == PresenceState.Null)
                {
                    child.Reset();
                    child.Visible = false;
                }
                else
                {
                    child.Visible = true;
                }
            }
        }

        /// <summary>
        /// Subclasses can override this to return a customised panel
        /// </summary>
        /// <returns></returns>
        protected virtual Panel ConstructMainContentPanel()
        {
            return new FlowLayoutPanel
                       {
                           AutoSize = true,
                           Margin = new Padding(0),
                           FlowDirection = FlowDirection.TopDown,
                           WrapContents = false
                       };
        }

        /// <summary>
        /// Subclasses can override this to return a customised panel
        /// </summary>
        /// <returns></returns>
        protected virtual Panel ConstructSideContentPanel()
        {
            return new FlowLayoutPanel
                       {
                           AutoSize = true,
                           Margin = new Padding(0),
                           FlowDirection = FlowDirection.RightToLeft,
                           WrapContents = false
                       };
        }

        public override void Reset()
        {
            //This will implicitly reset children
            Presence = PresenceState.Null;
        }

        public override Size IdealSize
        {
            get
            {
                int sideContentWidth = presenceCheck.Width + presenceCheck.Margin.Horizontal;
                int sideContentHeight = presenceCheck.Height + presenceCheck.Margin.Vertical;
                int mainContentWidth = 0;
                int mainContentHeight = 0;

                foreach (IView child in Children)
                {
                    //if child belongs to side panel, then ideal width is the presenceCheck's
                    //width plus the cumulative side content children's width; ideal height is
                    //the maximum height between presenceCheck and all side content children
                    if (sideContentPanel.Controls.Contains(child as Control))
                    {
                        sideContentWidth += child.Size.Width + GastrOsConfig.LayoutConfig.DefaultMargin.Value.Horizontal;
                        sideContentHeight = Math.Max(sideContentHeight, child.Size.Height + GastrOsConfig.LayoutConfig.DefaultMargin.Value.Vertical);
                    }
                    else
                    {
                        mainContentWidth = Math.Max(mainContentWidth, child.Size.Width + GastrOsConfig.LayoutConfig.DefaultMargin.Value.Horizontal);
                        mainContentHeight += child.Size.Height + GastrOsConfig.LayoutConfig.DefaultMargin.Value.Vertical;
                    }
                }

                //a temporary solution to ensure core concept widgets without any main content
                //are still as wide as ones that have
                sideContentWidth = Math.Max(sideContentWidth,
                                            GastrOsConfig.LayoutConfig.InputWidth + GastrOsConfig.LayoutConfig.LabelWidth +
                                            GastrOsConfig.LayoutConfig.DefaultMargin.Value.Horizontal);

                return new Size(Math.Max(mainContentWidth, sideContentWidth), mainContentHeight + sideContentHeight);
            }
        }
    }
}