using System.Configuration;
using GastrOs.Sde.Configuration.Elements;

namespace GastrOs.Sde.Configuration
{
    /// <summary>
    /// Represents the "LayoutConfig" section of the application configuration file. 
    /// </summary>
    public class LayoutConfigurationSection : CustomConfigSection
    {
        public const int DefaultQuantDecimalPlaces = 2;
        public const int DefaultLabelWidth = 140;
        public const int DefaultInputWidth = 180;
        public const int DefaultCoreConceptCheckWidth = 150;

        #region(scalar properties)

        /// <summary>
        /// Default width for attribute labels
        /// </summary>
        [ConfigurationProperty("labelWidth", DefaultValue = DefaultLabelWidth)]
        public int LabelWidth
        {
            get
            {
                return TryGetValue("labelWidth", DefaultLabelWidth);
            }
            set
            {
                this["labelWidth"] = value;
            }
        }

        /// <summary>
        /// Default width for attribute value input fields
        /// </summary>
        [ConfigurationProperty("inputWidth", DefaultValue = DefaultInputWidth)]
        public int InputWidth
        {
            get
            {
                return TryGetValue("inputWidth", DefaultInputWidth);
            }
            set
            {
                this["inputWidth"] = value;
            }
        }

        /// <summary>
        /// Maximum width for core concept checkboxes
        /// </summary>
        [ConfigurationProperty("coreConceptCheckWidth", DefaultValue = DefaultCoreConceptCheckWidth)]
        public int CoreConceptCheckWidth
        {
            get
            {
                return TryGetValue("coreConceptCheckWidth", DefaultCoreConceptCheckWidth);
            }
            set
            {
                this["coreConceptCheckWidth"] = value;
            }
        }

        /// <summary>
        /// No. decimal places for quantity fields
        /// </summary>
        [ConfigurationProperty("quantDecimalPlaces", DefaultValue = DefaultQuantDecimalPlaces)]
        public int QuantDecimalPlaces
        {
            get
            {
                return TryGetValue("quantDecimalPlaces", DefaultQuantDecimalPlaces);
            }
            set
            {
                this["quantDecimalPlaces"] = value;
            }
        }

        #endregion

        #region(composite properties)

        /// <summary>
        /// Default font for all text displayed in widgets
        /// </summary>
        [ConfigurationProperty("defaultFont")]
        public FontElement DefaultFont
        {
            get
            {
                return this["defaultFont"] as FontElement;
            }
            set
            {
                this["defaultFont"] = value;
            }
        }

        /// <summary>
        /// Default form size if not specified by directive
        /// </summary>
        [ConfigurationProperty("defaultFormSize")]
        public FormSizeElement DefaultFormSize
        {
            get
            {
                return this["defaultFormSize"] as FormSizeElement;
            }
            set
            {
                this["defaultFormSize"] = value;
            }
        }

        /// <summary>
        /// Default margin for all widgets
        /// </summary>
        [ConfigurationProperty("defaultMargin")]
        public MarginElement DefaultMargin
        {
            get
            {
                return this["defaultMargin"] as MarginElement;
            }
            set
            {
                this["defaultMargin"] = value;
            }
        }

        /// <summary>
        /// Default margin for all input fields (e.g. textboxes, comboboxes)
        /// inside element widgets
        /// </summary>
        [ConfigurationProperty("defaultFieldMargin")]
        public MarginElement DefaultFieldMargin
        {
            get
            {
                return this["defaultFieldMargin"] as MarginElement;
            }
            set
            {
                this["defaultFieldMargin"] = value;
            }
        }

        #endregion
    }
}