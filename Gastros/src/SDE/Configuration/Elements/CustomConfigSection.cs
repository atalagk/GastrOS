using System.Configuration;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Configuration.Elements
{
    public abstract class CustomConfigSection : ConfigurationSection
    {
        protected T TryGetValue<T>(string propName, T defaultValue)
        {
            try
            {
                return (T) this[propName];
            } catch (ConfigurationErrorsException e)
            {
                Logger.Error("Failed to get value for property "+propName+". Defaulting to "+defaultValue, e);
                this[propName] = defaultValue;
                return (T) this[propName];
            }
        }
    }
}