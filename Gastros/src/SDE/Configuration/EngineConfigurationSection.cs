using System.Configuration;
using GastrOs.Sde.Configuration.Elements;

namespace GastrOs.Sde.Configuration
{
    /// <summary>
    /// Configuration section that deals with core SDE-generator properties.
    /// </summary>
    public class EngineConfigurationSection : CustomConfigSection
    {
        public const string DefaultContainerName = "winforms";
        public const string DefaultAnnotationId = "default";

        /// <summary>
        /// Name of the Unity container to use in generating GUI components
        /// </summary>
        [ConfigurationProperty("containerForWidgets", DefaultValue = DefaultContainerName)]
        public string UnityContainerName
        {
            get
            {
                return TryGetValue("containerForWidgets", DefaultContainerName);
            }
            set
            {
                this["containerForWidgets"] = value;
            }
        }

        [ConfigurationProperty("presenceSemantics")]
        public PresenceSemanticsElement PresenceSemantics
        {
            get
            {
                return this["presenceSemantics"] as PresenceSemanticsElement;
            }
            set
            {
                this["presenceSemantics"] = value;
            }
        }

        [ConfigurationProperty("annotationIdForDirectives", DefaultValue = DefaultAnnotationId)]
        public string AnnotationIdForDirectives
        {
            get
            {
                return TryGetValue("annotationIdForDirectives", DefaultAnnotationId);
            }
            set
            {
                this["annotationIdForDirectives"] = value;
            }
        }
    }
}
