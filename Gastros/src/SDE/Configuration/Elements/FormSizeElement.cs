using System.Configuration;
using System.Drawing;

namespace GastrOs.Sde.Configuration.Elements
{
    /// <summary>
    /// Configuration element for specifying default form size (width and height)
    /// </summary>
    public class FormSizeElement : CustomConfigElement
    {
        public const int DefaultWidth = 800;
        public const int DefaultHeight = 640;

        [ConfigurationProperty("width", DefaultValue = DefaultWidth)]
        public int Width
        {
            get
            {
                return TryGetValue("width", DefaultWidth);
            }
            set
            {
                this["width"] = value;
            }
        }

        [ConfigurationProperty("height", DefaultValue = DefaultHeight)]
        public int Height
        {
            get
            {
                return TryGetValue("height", DefaultHeight);
            }
            set
            {
                this["height"] = value;
            }
        }

        public Size Value
        {
            get
            {
                return new Size(Width, Height);
            }
        }
    }
}