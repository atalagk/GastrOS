using System;
using System.Configuration;
using GastrOs.Sde.Support;

namespace GastrOs.Wrapper.Helpers
{
    public class WrapperConfig
    {
        protected const string GastrosSection = "WrapperSettings";

        private static Configuration config;
        private static WrapperConfigurationSection gastrosSection;

        static WrapperConfig()
        {
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                gastrosSection = (WrapperConfigurationSection)config.GetSection(GastrosSection);
            } catch (Exception e)
            {
                Logger.Error("Failed to configure GastrOs wrapper.", e);
            }
            if (gastrosSection == null) gastrosSection = new WrapperConfigurationSection();
        }

        public static WrapperConfigurationSection Get
        {
            get
            {
                return gastrosSection;
            }
        }

        public static void Save()
        {
            config.Save();
        }
    }
}
