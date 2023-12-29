using System.Configuration;
using GastrOs.Sde.Support;

namespace GastrOs.Sde.Configuration.Elements
{
    /// <summary>
    /// Configuration element for configuring the meaning of "presence" in core concepts
    /// </summary>
    public class PresenceSemanticsElement : CustomConfigElement
    {
        //public const string DefaultPresenceCode = "at0105";
        public const string DefaultDescForNull = "Presence=null";
        public const string DefaultDescForPresent = "Presence=present";
        public const string DefaultDescForUnknown = "Presence=unknown";
        public const string DefaultDescForAbsent = "Presence=absent";

        /*[ConfigurationProperty("presenceCode", DefaultValue = DefaultPresenceCode)]
        public string PresenceCode
        {
            get
            {
                return TryGetValue("presenceCode", DefaultPresenceCode);
            }
            set
            {
                this["presenceCode"] = value;
            }
        }*/

        [ConfigurationProperty("descForNull", DefaultValue = DefaultDescForNull)]
        public string DescForNull
        {
            get
            {
                return TryGetValue("descForNull", DefaultDescForNull);
            }
            set
            {
                this["descForNull"] = value;
            }
        }

        [ConfigurationProperty("descForPresent", DefaultValue = DefaultDescForPresent)]
        public string DescForPresent
        {
            get
            {
                return TryGetValue("descForPresent", DefaultDescForPresent);
            }
            set
            {
                this["descForPresent"] = value;
            }
        }

        [ConfigurationProperty("descForUnknown", DefaultValue = DefaultDescForUnknown)]
        public string DescForUnknown
        {
            get
            {
                return TryGetValue("descForUnknown", DefaultDescForUnknown);
            }
            set
            {
                this["descForUnknown"] = value;
            }
        }

        [ConfigurationProperty("descForAbsent", DefaultValue = DefaultDescForAbsent)]
        public string DescForAbsent
        {
            get
            {
                return TryGetValue("descForAbsent", DefaultDescForAbsent);
            }
            set
            {
                this["descForAbsent"] = value;
            }
        }

        /// <summary>
        /// Returns the ontology description (in the term bindings) that the given
        /// presence status corresponds to.
        /// </summary>
        /// <param name="presence"></param>
        /// <returns></returns>
        public string OntologyDescriptionFor(PresenceState presence)
        {
            switch (presence)
            {
                case PresenceState.Present:
                    return DescForPresent;
                case PresenceState.Unknown:
                    return DescForUnknown;
                case PresenceState.Absent:
                    return DescForAbsent;
                default:
                    return DescForNull;
            }
        }

        /// <summary>
        /// The logical converse of <see cref="OntologyDescriptionFor"/>
        /// </summary>
        /// <param name="ontologyDescription"></param>
        /// <returns></returns>
        public PresenceState LookupPresenceState(string ontologyDescription)
        {
            if (string.Equals(ontologyDescription, DescForPresent))
                return PresenceState.Present;
            if (string.Equals(ontologyDescription, DescForUnknown))
                return PresenceState.Unknown;
            if (string.Equals(ontologyDescription, DescForAbsent))
                return PresenceState.Absent;
            return PresenceState.Null;
        }
    }
}