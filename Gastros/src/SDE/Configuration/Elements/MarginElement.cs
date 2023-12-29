using System.Configuration;
using System.Windows.Forms;

namespace GastrOs.Sde.Configuration.Elements
{
    /// <summary>
    /// Configuration element that represents a margin (left, right, top, bottom)
    /// </summary>
    public class MarginElement : CustomConfigElement
    {
        public const int DefaultLeft = 3;
        public const int DefaultRight = 3;
        public const int DefaultTop = 3;
        public const int DefaultBottom = 3;

        [ConfigurationProperty("left", DefaultValue = DefaultLeft)]
        public int Left
        {
            get
            {
                return TryGetValue("left", DefaultLeft);
            }
            set
            {
                this["left"] = value;
            }
        }

        [ConfigurationProperty("right", DefaultValue = DefaultRight)]
        public int Right
        {
            get
            {
                return TryGetValue("right", DefaultRight);
            }
            set
            {
                this["right"] = value;
            }
        }

        [ConfigurationProperty("top", DefaultValue = DefaultTop)]
        public int Top
        {
            get
            {
                return TryGetValue("top", DefaultTop);
            }
            set
            {
                this["top"] = value;
            }
        }

        [ConfigurationProperty("bottom", DefaultValue = DefaultBottom)]
        public int Bottom
        {
            get
            {
                return TryGetValue("bottom", DefaultBottom);
            }
            set
            {
                this["bottom"] = value;
            }
        }

        public Padding Value
        {
            get
            {
                return new Padding(Left, Top, Right, Bottom);
            }
        }
    }
}
