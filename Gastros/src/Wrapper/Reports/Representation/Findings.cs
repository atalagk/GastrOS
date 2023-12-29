using System.Collections.Generic;
using System.Linq;
using System.Text;
using GastrOs.Sde.Support;

namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents findings for a single organ.
    /// Stores findings as a many-to-many relationship between Terms
    /// and Sites, where Attributes are a property of the relationship.
    /// </summary>
    public class Findings : MstItem
    {
        //special term denoting "normal" - may be null, in which case no "normal" sites
        private Term normalTerm;
        private HashSet<IList<Term>> termGroups;

        public Findings(string organ) : base(organ)
        {
            termGroups = new HashSet<IList<Term>>();
        }

        /// <summary>
        /// Gets/sets the special term denoting "normal"-ness
        /// </summary>
        public Term NormalTerm
        {
            get { return normalTerm; }
            set { normalTerm = value; }
        }

        public void AddTermInstances(IEnumerable<Term> termInstances)
        {
            IList<Term> termGroup = new List<Term>();
            foreach (Term term in termInstances)
            {
                if (term.Presence != null)
                {
                    termGroup.Add(term);
                }
            }
            termGroups.Add(termGroup);
        }

        public HashSet<IList<Term>> TermGroups
        {
            get { return termGroups; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool isIntervention)
        {
            //If no terms are present, and no "normal" indicated, return nothing
            if (termGroups.Count() == 0 && normalTerm == null)
                return null;

            StringBuilder sb = new StringBuilder();
            //<Organ>
            sb.Append(DisplayName);

            //If normal, then format as <Organ>: [(Site1, .. Site2)] normal.
            if (normalTerm != null)
            {
                sb.Append(": ");
                IList<Site> normalSites = normalTerm.Sites;
                if (normalSites.Count > 0)
                {
                    sb.Append(normalSites.ToPrettyString(", ")).Append(" normal.");
                }
                else
                {
                    sb.Append("Normal.");
                }
            }

            //List all terms - grouped by instances of the same kind
            foreach (IList<Term> termGroup in termGroups)
            {
                for (int ordinal = 1; ordinal <= termGroup.Count; ordinal ++ )
                {
                    //Prefix with ordinal (and separator if necessary) in case of multiple instances
                    sb.Append("\r\n    ");
                    if (termGroup.Count > 1) sb.Append('(').Append(ordinal).Append(") ");
                    sb.Append(termGroup[ordinal - 1].ToString(isIntervention));
                }
            }

            return sb.ToString();
        }
    }
}