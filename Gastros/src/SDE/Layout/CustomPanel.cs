using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace GastrOs.Sde.Views.WinForms.Generic
{
    /// <summary>
    /// Provides a reusable abstraction for custom layout panels that
    /// use an instance of <see cref="CustomLayoutBase"/> to layout
    /// their controls.
    /// </summary>
    public abstract class CustomPanel : Panel
    {
        public override Size GetPreferredSize(Size proposedSize)
        {
            //Note this only measures the ideal Size, without doing any layout
            Size idealSize = DynamicLayoutBase.LayoutOrMeasure(this, false);
            return idealSize;
        }

        public sealed override LayoutEngine LayoutEngine
        {
            get
            {
                return DynamicLayoutBase;
            }
        }

        protected abstract CustomLayoutBase DynamicLayoutBase
        {
            get;
        }
    }

    /// <summary>
    /// Provides a reusable abstraction for custom layout engines.
    /// </summary>
    public abstract class CustomLayoutBase : LayoutEngine
    {
        public override bool Layout(object containerObj, LayoutEventArgs layoutEventArgs)
        {
            if (!(containerObj is CustomPanel))
                return false;

            CustomPanel container = containerObj as CustomPanel;

            Size layoutSize = LayoutOrMeasure(container, true);

            container.SuspendLayout();

            container.AutoScrollMinSize = Size.Empty;
            if (container.AutoSize)
            {
                container.Size = layoutSize;
                //return true;
            }
            else if (layoutSize.Width > container.Width || layoutSize.Height > container.Height)
            {
                //Ideal Size is larger than the container's current Size
                container.AutoScrollMinSize = container.Size;
                //return true;
            }

            container.ResumeLayout(false);

            return false;
        }

        /// <summary>
        /// Either performs layout on the container or just measures the ideal
        /// Size of the container that just fits its contents, depending
        /// on the value of <see cref="layout"></see>
        /// </summary>
        /// <param name="container">the container to layout or measure</param>
        /// <param name="layout">performs layout if true; only measures otherwise</param>
        /// <returns>the ideal Size of the container</returns>
        public abstract Size LayoutOrMeasure(CustomPanel container, bool layout);
    }
}