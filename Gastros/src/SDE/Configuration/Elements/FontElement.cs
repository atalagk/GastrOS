using System.Configuration;
using System.Drawing;

namespace GastrOs.Sde.Configuration.Elements
{
    /// <summary>
    /// Confiuration element that represents a font (family and size)
    /// </summary>
    public class FontElement : ConfigurationElement
    {
        [ConfigurationProperty("family", DefaultValue = "Arial", IsRequired = false)]
        public string Family
        {
            get
            {
                return this["family"] as string;
            }
            set
            {
                this["family"] = value;
            }
        }

        [ConfigurationProperty("size", DefaultValue = 9f, IsRequired = false)]
        public float Size
        {
            get
            {
                return (float)this["size"];
            }
            set
            {
                this["size"] = value;
            }
        }

        public Font Value
        {
            get
            {
                return new Font(Family, Size);
            }
        }
    }
}