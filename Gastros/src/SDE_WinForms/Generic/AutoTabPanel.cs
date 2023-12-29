using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Views.WinForms.Support;
using OpenEhr.DesignByContract;
using GastrOs.Sde.Directives;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    /// <summary>
    /// A custom panel that automatically organises its children under tabs and
    /// columns
    /// 
    /// TODO simply? Especially in terms of layout and size management
    /// </summary>
    public class AutoTabPanel : Panel
    {
        private TabControl pagesContainer;
        private TabPage currentPage;
        private TableLayoutPanel currentColumnsGrid;
        private FlowLayoutPanel currentColumn;
        private int initialColumns;
        private string firstTabTitle = GuiDictionary.Lookup("page")+"-1";

        public AutoTabPanel() : this(1)
        {
        }

        public AutoTabPanel(int initialColumns)
        {
            this.initialColumns = initialColumns;
            InitComponents();
        }

        private void InitComponents()
        {
            SuspendLayout();

            currentColumnsGrid = InitialiseColumnsGrid();
            IncrementColumn();

            Controls.Add(currentColumnsGrid);

            ResumeLayout(false);
        }

        private TableLayoutPanel InitialiseColumnsGrid()
        {
            TableLayoutPanel gridPanel = new TableLayoutPanel
                                             {
                                                 Dock = DockStyle.Fill,
                                                 AutoSize = true,
                                                 ColumnCount = initialColumns,
                                                 RowCount = 1,
                                                 Margin = new Padding(0)
                                             };
            //Basically given n columns, 1 .. n-1 columns are autosized, and the nth column fills the rest
            for (int i = 0; i < initialColumns - 1; i++)
                gridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            gridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            gridPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            return gridPanel;
        }

        private FlowLayoutPanel ConstructColumn()
        {
            return new FlowLayoutPanel
                       {
                           Dock = DockStyle.Fill,
                           AutoSize = true,
                           FlowDirection = FlowDirection.TopDown,
                           WrapContents = false,
                           Margin = new Padding(0)
                       };
        }

        protected void IncrementTab()
        {
            if (pagesContainer == null)
            { //no tabs as of yet
                pagesContainer = new TabControl();
                pagesContainer.Margin = new Padding(0);
                pagesContainer.Dock = DockStyle.Fill;
                pagesContainer.AutoSize = true;
                pagesContainer.SizeMode = TabSizeMode.Fixed;
                pagesContainer.Layout += UpdateTabWidth;
                
                TabPage page1 = new TabPage(firstTabTitle) { AutoSize = true };

                Controls.Remove(currentColumnsGrid);
                page1.Controls.Add(currentColumnsGrid);
                //basically now that currentColumnsGrid is within tab, make sure that the stuff
                //inside tab is auto-resizable.
                currentColumnsGrid.Layout += UpdateTabLayout;
                
                pagesContainer.TabPages.Add(page1);
                Controls.Add(pagesContainer);
            }

            currentPage = new TabPage(GuiDictionary.Lookup("page")+"-" + (pagesContainer.TabCount + 1)) { AutoSize = true };
            currentColumnsGrid = InitialiseColumnsGrid();
            //make sure that the stuff //inside tab is auto-resizable.
            currentColumnsGrid.Layout += UpdateTabLayout;
            
            pagesContainer.TabPages.Add(currentPage);
            IncrementColumn();
            currentPage.Controls.Add(currentColumnsGrid);
        }

        protected void IncrementColumn()
        {
            currentColumn = ConstructColumn();
            currentColumnsGrid.Controls.Add(currentColumn, currentColumnsGrid.Controls.Count, 0);
            while (currentColumnsGrid.Controls.Count > currentColumnsGrid.ColumnCount)
            {
                //Basically given n columns, 1 .. n-1 columns are autosized, and the nth column fills the rest
                foreach (ColumnStyle colStyle in currentColumnsGrid.ColumnStyles)
                {
                    colStyle.SizeType = SizeType.AutoSize;
                }
                currentColumnsGrid.ColumnCount++;
                currentColumnsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }

#if (DEBUG)
            //currentColumn.MouseMove += currentColumn_MouseMove;
#endif
        }

#if (DEBUG)
        void currentColumn_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }
#endif

        public void SetTabTitle(int tabIndex, string title)
        {
            if (pagesContainer == null)
            {
                //Means no tabs have been created yet - so save this title
                firstTabTitle = title;
            }
            else if (tabIndex < pagesContainer.TabCount)
            {
                pagesContainer.TabPages[tabIndex].Text = title;
            }
        }

        public void AddChild(Control child)
        {
            AddChild(child, BreakStyle.None);
        }

        public void AddChild(Control child, BreakStyle breakStyle)
        {
            child.Width = currentColumn.Width - currentColumn.Padding.Horizontal - child.Margin.Horizontal;
            if (breakStyle == BreakStyle.Tab)
                IncrementTab();
            else if (breakStyle == BreakStyle.Next /*|| (!AutoSize && NeedNewColumn(child))*/)
                IncrementColumn();
            currentColumn.Controls.Add(child);
        }

        public void RemoveChild(Control child)
        {
            foreach (Control container in currentColumnsGrid.Controls)
            {
                if (container.Controls.Contains(child))
                {
                    container.Controls.Remove(child);
                }
            }
        }

        private void UpdateTabWidth(object sender, LayoutEventArgs e)
        {
            int width = Math.Max(pagesContainer.ClientSize.Width / pagesContainer.TabCount - 2, 0);
            pagesContainer.ItemSize = new Size(width, pagesContainer.ItemSize.Height);
        }

        private void UpdateTabLayout(object sender, LayoutEventArgs e)
        {
            ScrollableControl tab = sender as ScrollableControl;
            Check.Assert(tab != null);
            tab.AutoScrollMinSize = GetIdealSizeForGrid(tab);
        }

        /// <summary>
        /// Return the ideal Size for given columns grid.
        /// </summary>
        /// <param name="columnsGrid"></param>
        /// <returns></returns>
        private static Size GetIdealSizeForGrid(Control columnsGrid)
        {
            // max width of tab T = sum of max widths of columns in T
            // max height of tab T = max of the set of height sums for each column in T
            
            int idealWidth = 0;
            int idealHeight = 0;
            //traverse through each column (better be a FlowLayoutPanel!)
            foreach (FlowLayoutPanel column in columnsGrid.Controls)
            {
                int maxWidthForColumn = 0;
                int totalHeightForColumn = 0;
                foreach (Control child in column.Controls)
                {
                    maxWidthForColumn = Math.Max(maxWidthForColumn, child.Width + child.Margin.Horizontal);
                    totalHeightForColumn += child.Height + child.Margin.Vertical;
                }

                idealWidth += maxWidthForColumn;
                idealHeight = Math.Max(idealHeight, totalHeightForColumn);
            }
            return new Size(idealWidth, idealHeight);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            //preferred width = max of WT (set containing max widths of all tabs)
            //preferred height = max of WH (set containing max heights of all tabs)             

            //set up a generic way to traverse through tabs
            List<Control> tabs = new List<Control>();
            if (pagesContainer == null)
            { //i.e. only single tab
                tabs.Add(currentColumnsGrid);
            }
            else
            {
                //each tabpage should contain a single TableLayoutPanel representing
                //the columns
                foreach (TabPage tabPage in pagesContainer.TabPages)
                {
                    tabs.Add(tabPage.Controls[0]);
                }
            }

            int overallMaxWidth = 0;
            int overallMaxHeight = 0;

            //now actually traverse through the tabs and figure out max width & height
            foreach (Control tab in tabs)
            {
                Size idealSizeForTab = GetIdealSizeForGrid(tab);
                overallMaxWidth = Math.Max(overallMaxWidth, idealSizeForTab.Width);
                overallMaxHeight = Math.Max(overallMaxHeight, idealSizeForTab.Height);
            }

            Size preferredSize = new Size(overallMaxWidth, overallMaxHeight);
            //if more than one tab (hence existence of TabControl), then add the
            //vertical and horizontal offset required for tabs
            if (pagesContainer != null)
            {
                Size tabAllowance = new Size(8, pagesContainer.ItemSize.Height);
                preferredSize += tabAllowance;
            }

            return preferredSize;

//            return new Size(Math.Min(preferredSize.Width, LayoutConfig.MaximumContentSize.Width),
//                            Math.Min(preferredSize.Height, LayoutConfig.MaximumContentSize.Height));
        }

        public bool IsTabbed
        {
            get
            {
                return pagesContainer != null;
            }
        }

        public IList<Control> Children
        {
            get
            {
                List<Control> children = new List<Control>();
                if (pagesContainer == null)
                { //if single page, then all controls are within currentColumnsGrid
                    foreach (Control container in currentColumnsGrid.Controls)
                    {
                        foreach (Control child in container.Controls)
                        {
                            children.Add(child);
                        }
                    }
                }
                else
                {
                    foreach (Control page in pagesContainer.Controls)
                    {
                        //page had better have at least (and most) 1 child!
                        Check.Assert(page.Controls.Count == 1);
                        Control columnsGrid = page.Controls[0];
                        foreach (Control container in columnsGrid.Controls)
                        {
                            foreach (Control child in container.Controls)
                            {
                                children.Add(child);
                            }
                        }
                    }
                }
                return children;
            }
        }
    }
}