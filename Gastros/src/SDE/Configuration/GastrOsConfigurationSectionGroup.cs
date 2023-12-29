using System.Configuration;
using Microsoft.Practices.Unity.Configuration;

namespace GastrOs.Sde.Configuration
{
    /// <summary>
    /// Top-level configuration class for GastrOs.
    /// </summary>
    public class GastrOsConfigurationSectionGroup : ConfigurationSectionGroup
    {
        public const string EngineSectionName = "Engine";
        public const string LayoutSectionName = "Layout";
        public const string UnitySectionName = "UnityContainer";

        private EngineConfigurationSection engineSectionCache;
        private LayoutConfigurationSection layoutSectionCache;
        private UnityConfigurationSection unitySectionCache;

        public EngineConfigurationSection EngineSection
        {
            get
            {
                EngineConfigurationSection section = Sections[EngineSectionName] as EngineConfigurationSection;
                if (section != null)
                    return section;
                if (engineSectionCache == null)
                    engineSectionCache = new EngineConfigurationSection();
                return engineSectionCache;
            }
        }

        public LayoutConfigurationSection LayoutSection
        {
            get
            {
                LayoutConfigurationSection section = Sections[LayoutSectionName] as LayoutConfigurationSection;
                if (section != null)
                    return section;
                if (layoutSectionCache == null)
                    layoutSectionCache = new LayoutConfigurationSection();
                return layoutSectionCache;
            }
        }

        public UnityConfigurationSection UnitySection
        {
            get
            {
                UnityConfigurationSection section = Sections[UnitySectionName] as UnityConfigurationSection;
                if (section != null)
                    return section;
                if (unitySectionCache == null)
                    unitySectionCache = new UnityConfigurationSection();
                return unitySectionCache;
            }
        }
    }
}
