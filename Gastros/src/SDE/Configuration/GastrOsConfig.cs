using System;
using System.Configuration;
using GastrOs.Sde.Support;
using Microsoft.Practices.Unity.Configuration;

namespace GastrOs.Sde.Configuration
{
    /// <summary>
    /// Centrally handles all configuration-related matter for GastrOs SDE.
    /// This includes configuration for layout parameters and for the Unity container.
    /// </summary>
    public static class GastrOsConfig
    {
        /// <summary>
        /// Name of the expected XML root element
        /// </summary>
        public const string ConfigRootName = "GastrOsSDE";

        private static GastrOsConfigurationSectionGroup configurationRoot;

        static GastrOsConfig()
        {
            //TODO see if we can "pull out" configuration source so it can come from an arbitrary file
            Configure();
        }

        public static void Configure()
        {
            try
            {
                System.Configuration.Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configurationRoot = c.GetSectionGroup(ConfigRootName) as GastrOsConfigurationSectionGroup;
            } catch (Exception e)
            {
                Logger.Error("Failed to configure GastrOS SDE from configuration file. Resorting to default values.", e);
                //Resort to default values.
                configurationRoot = new GastrOsConfigurationSectionGroup();
            }
        }

        public static EngineConfigurationSection EngineConfig
        {
            get
            {
                return configurationRoot.EngineSection;
            }
        }

        public static LayoutConfigurationSection LayoutConfig
        {
            get
            {
                return configurationRoot.LayoutSection;
            }
        }

        public static UnityConfigurationSection UnityConfig
        {
            get
            {
                return configurationRoot.UnitySection;
            }
        }
    }
}
