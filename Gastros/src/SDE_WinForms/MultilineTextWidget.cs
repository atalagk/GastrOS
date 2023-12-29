using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Configuration;
using GastrOs.Sde.Views.WinForms.Support;

namespace GastrOs.Sde.Views.WinForms
{
    /// <summary>
    /// Implements multi-line text view
    /// </summary>
    public class MultilineTextWidget : TextWidget
    {
        public MultilineTextWidget()
        {
            textField.Multiline = true;
            textField.ScrollBars = ScrollBars.Vertical;
            int idealHeight = textField.GetSizeToFitText(GastrOsConfig.LayoutConfig.InputWidth, 20,
                                                        "a\r\nb\r\nc\r\nd").Height;
            textField.Size = new Size(GastrOsConfig.LayoutConfig.InputWidth, idealHeight);
        }

        protected override int InputMinHeight
        {
            get
            {
                return 62; //CFG
            }
        }
    }
}
