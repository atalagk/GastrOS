using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.Reports.Representation;
using Attribute=GastrOs.Wrapper.Reports.Representation.Attribute;

namespace GastrOs.Wrapper.Reports
{
    public class TurkishReportFormatter : IReportFormatter
    {
        //i.e. last character is one of [aıou] or second to last character is
        private static Regex suffixMatch = new Regex("[aıou].?$");

        public string FormatFindings(Findings findings)
        {
            return FormatFindingsOrInterventions(findings, false);
        }

        public string FormatInterventions(Findings findings)
        {
            return FormatFindingsOrInterventions(findings, true);
        }

        private string FormatFindingsOrInterventions(Findings findings, bool isIntervention)
        {
            //If no terms are present, and no "normal" indicated, return nothing
            if (findings.TermGroups.Count == 0 && findings.NormalTerm == null)
                return null;

            StringBuilder sb = new StringBuilder();
            //<Organ>
            sb.Append(findings.DisplayName);

            //If normal, then format as <Organ>: [(Site1, .. Site2)] normal.
            if (findings.NormalTerm != null)
            {
                sb.Append(": ");
                IList<Site> normalSites = findings.NormalTerm.Sites;
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
            foreach (IList<Term> termGroup in findings.TermGroups)
            {
                for (int ordinal = 1; ordinal <= termGroup.Count; ordinal++)
                {
                    //Prefix with ordinal (and separator if necessary) in case of multiple instances
                    sb.Append("\r\n    ");
                    if (termGroup.Count > 1) sb.Append('(').Append(ordinal).Append(") ");
                    sb.Append(FormatTerm(termGroup[ordinal - 1], isIntervention));
                }
            }

            return sb.ToString();
        }

        private string FormatTerm(Term term, bool isIntervention)
        {
            /* Findings:
             * Site1, Site2, Site(n){de|da} Term1 görüldü|bilinmiyor|yoktu. Attributen: Attributen Value
             * The grammar rules for above (if any site has been selected):
             *      1) "da " shall be appended if the last or second to last character is either of [aıou]
             *      2) for all other conditions "de " shall be appended
             * -----------
             * Interventions: same as Findings (including rules for Sites) but the existence qualifier
             * shall be limited to “yapıldı” only if the two state checkbox has been checked.
             **/

            StringBuilder sb = new StringBuilder();

            if (term.Sites.Count > 0)
            {
                string sitesText = term.Sites.ToPrettyString(", ");
                sb.Append(sitesText);
                //Apply the tricky grammar rule!
                if (suffixMatch.IsMatch(sitesText))
                    sb.Append("da ");
                else
                    sb.Append("de ");
            }

            sb.Append(term.DisplayName);
            if (term.SpellOutPresence)
            {
                string presenceText = isIntervention ? "yapıldı" : TermToTurkish(term);
                sb.Append(" ").Append(presenceText).Append(".");
            }
            else
            {
                sb.Append(".");
            }
            IList<Attribute> attributes = term.Attributes;
            if (attributes.Count > 0)
            {
                sb.Append(" ").Append(attributes.ToPrettyString(", ")).Append(".");
            }
            return sb.ToString();
        }

        private string TermToTurkish(Term term)
        {
            switch (term.Presence)
            {
                case PresenceState.Present:
                    return "görüldü";
                case PresenceState.Unknown:
                    return "bilinmiyor";
                case PresenceState.Absent:
                    return "yoktu";
            }
            return null;
        }

        public string FormatDiagnoses(Diagnoses dx)
        {
            return dx.ToString();
        }

        public string FormatExamInfo(ExamInfo examInfo)
        {
            return examInfo.ToString();
        }
    }
}