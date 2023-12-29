using System.Collections.Generic;
using System.Text;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.Helpers;

namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents an MST term
    /// </summary>
    public class Term : MstItem
    {
        private PresenceState presence;
        private bool spellOutPresence;

        private IList<Attribute> attributes;
        private IList<Site> sites;

        public Term(string termName, PresenceState presence, bool spellOutPresence) : base(termName)
        {
            this.presence = presence;
            this.spellOutPresence = spellOutPresence;
            if (presence != PresenceState.Null)
            {
                attributes = new List<Attribute>();
                sites = new List<Site>();
            }
        }

        /// <summary>
        /// Gets the presence semantics value for this term
        /// </summary>
        public PresenceState Presence
        {
            get { return presence; }
        }

        /// <summary>
        /// Whether to include the suffix denoting presence (e.g. "present", "absent", "unknown")
        /// - false in case of "Normal" terms
        /// </summary>
        public bool SpellOutPresence
        {
            get { return spellOutPresence; }
        }

        /// <summary>
        /// Gets the list of attributes associated with this term (must be present,
        /// otherwise returns null)
        /// </summary>
        /// <returns></returns>
        public IList<Attribute> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Gets the list of sites associated with this term (must be present,
        /// otherwise returns null)
        /// </summary>
        /// <returns></returns>
        public IList<Site> Sites
        {
            get { return sites; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool isIntervention)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DisplayName);
            if (spellOutPresence)
            {
                string presenceText = isIntervention ? "performed" : Presence.ToString().ToLower();
                sb.Append(" ").Append(presenceText);
            }
            if (sites.Count > 0)
            {
                sb.Append(" at ").Append(sites.ToPrettyString(", ")).Append(".");
            }
            else
            {
                sb.Append('.');
            }
            IList<Attribute> attributes = Attributes;
            if (attributes.Count > 0)
            {
                sb.Append(" ").Append(attributes.ToPrettyString(", ")).Append(".");
            }
            return sb.ToString();
        }
    }
}