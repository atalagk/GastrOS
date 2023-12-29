using System.Configuration;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Configuration.Elements
{
    public abstract class CustomConfigElement : ConfigurationElement
    {
        protected T TryGetValue<T>(string propName, T defaultValue)
        {
            try
            {
                return (T) this[propName];
            } catch (ConfigurationErrorsException e)
            {
                Logger.Error("Failed to get value for property "+propName, e);
                return defaultValue;
            }
        }
    }
}