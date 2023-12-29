using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Views.WinForms.Generic;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Simple container view that vertically stacks sub-views.
    /// </summary>
    public class SimpleContainerWidget : ContainerWidgetBase, IContainerView
    {
        private BoxPanel basePanel;

        public SimpleContainerWidget()
        {
            basePanel = new BoxPanel();
            basePanel.Margin = new Padding(0);
            basePanel.Dock = DockStyle.Fill;

            Controls.Add(basePanel);

#if (DEBUG)
            basePanel.MouseMove += basePanel_MouseMove;
#endif
        }

#if (DEBUG)
        void basePanel_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(Title+": "+IdealSize);
        }
#endif

        public override string Title
        {
            get
            {
                return basePanel.Text;
            }
            set
            {
                if (Title == value)
                    return;
                basePanel.Text = value;
                //fire event as per interface contract
                OnTitleChanged(EventArgs.Empty);
            }
        }

        public override bool Framed
        {
            get
            {
                return basePanel.Framed;
            }
            set
            {
                basePanel.Framed = value;
            }
        }

        public override Size IdealSize
        {
            get
            {
                int width = 0;
                int height = 0;
                foreach (IView child in Children)
                {
                    width = Math.Max(child.Size.Width + GastrOsConfig.LayoutConfig.DefaultMargin.Value.Horizontal, width);
                    height += child.Size.Height + GastrOsConfig.LayoutConfig.DefaultMargin.Value.Vertical;
                }
                Size idealSize = new Size(width, height);
                //if framed, allow for the space taken up by border
                idealSize += basePanel.FrameAllowance;
                return idealSize;
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
            Control childControl = child as Control;
            childControl.Margin = GastrOsConfig.LayoutConfig.DefaultMargin.Value;
            basePanel.InsertChild(index, childControl);
        }

        protected override void RemoveChildView(IView child)
        {
            basePanel.RemoveChild(child as Control);
        }
    }
}