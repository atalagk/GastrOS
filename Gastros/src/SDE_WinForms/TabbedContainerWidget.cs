using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Directives;
using GastrOs.Sde.Views.WinForms.Generic;

namespace GastrOs.Sde.Views.WinForms
{
    public class TabbedContainerWidget : ContainerWidgetBase, IContainerView
    {
        private AutoTabPanel basePanel;
        private BoxPanel basePanelBox;

        public TabbedContainerWidget()
        {
            basePanel = new AutoTabPanel();
            basePanel.Margin = new Padding(0);
            basePanel.Dock = DockStyle.Fill;
            
            basePanelBox = new BoxPanel();
            basePanelBox.Contents = basePanel;
            basePanelBox.Margin = new Padding(0);
            basePanelBox.Dock = DockStyle.Fill;

            Controls.Add(basePanelBox);

#if (DEBUG)
            basePanel.MouseMove += new MouseEventHandler(basePanel_MouseMove);
#endif
        }

#if (DEBUG)
        void basePanel_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(Title + ": " + IdealSize);
        }
#endif

        public override bool RequiresExternalScrolling
        {
            get
            {
                return !basePanel.IsTabbed;
            }
        }

        public override string Title
        {
            get { return basePanelBox.Text; }
            set
            {
                if (string.Equals(Title, value))
                    return;
                basePanelBox.Text = value;
                OnTitleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets whether a frame is to be drawn around the panel
        /// </summary>
        public override bool Framed
        {
            get { return basePanelBox.Framed; }
            set
            {
                if (Framed == value)
                    return;
                basePanelBox.Framed = value;
            }
        }

        public override Size IdealSize
        {
            get
            {
                return basePanel.PreferredSize + basePanelBox.FrameAllowance;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size ideal = IdealSize;
            //"fill" this widget horizontally
            return new Size(Math.Max(ideal.Width, proposedSize.Width), ideal.Height);
        }

        protected override void AddChildView(IView child, int index)
        {
            BreakDirective breakDirective = child.Directives.GetDirectiveOfType<BreakDirective>();
            BreakStyle breakStyle = breakDirective != null ? breakDirective.BreakStyle : BreakStyle.None;
            basePanel.AddChild(child as Control, breakStyle);
        }

        protected override void RemoveChildView(IView child)
        {
            basePanel.RemoveChild(child as Control);
        }
    }
}
