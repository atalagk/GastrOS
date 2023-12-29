using System.Configuration;
using GastrOs.Sde.Configuration.Elements;

namespace GastrOs.Wrapper.Helpers
{
    public class WrapperConfigurationSection : CustomConfigSection
    {
        public const string DefaultStartupMode = "normal";
        public const string DefaultKnowledgePath = "Knowledge";
        public const string DefaultLanguage = "en";

        [ConfigurationProperty("startupMode", DefaultValue = DefaultStartupMode)]
        public string StartupMode
        {
            get
            {
                return TryGetValue("startupMode", DefaultStartupMode);
            }
            set
            {
                this["startupMode"] = value;
            }
        }

        [ConfigurationProperty("knowledgePath", DefaultValue = DefaultKnowledgePath)]
        public string KnowledgePath
        {
            get
            {
                return TryGetValue("knowledgePath", DefaultKnowledgePath);
            }
            set
            {
                this["knowledgePath"] = value;
            }
        }

        [ConfigurationProperty("language", DefaultValue = DefaultLanguage)]
        public string Language
        {
            get
            {
                return TryGetValue("language", DefaultLanguage);
            }
            set
            {
                this["language"] = value;
            }
        }
    }
}
