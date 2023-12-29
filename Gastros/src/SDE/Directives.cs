using System.Collections.Generic;
using System.Text.RegularExpressions;
using OceanEhr.OpenEhrV1.AM.Archetype.ConstraintModel;

namespace GastrOs.Sde
{
    /// <summary>
    /// Represents "directives" that modify how the generated GUI elements are
    /// organised and presented
    /// </summary>
    public class Directives
    {
        private Dictionary<CObject, Dictionary<string, Directive>> innerMap;

        public Directives()
        {
            innerMap = new Dictionary<CObject, Dictionary<string,Directive>>();
        }

        public ICollection<Directive> GetDirectivesFor(CObject node)
        {
            if (innerMap.ContainsKey(node) && innerMap[node] != null)
                return innerMap[node].Values;
            return new HashSet<Directive>();
        }

        public Directive GetDirectiveByName(CObject node, string name)
        {
            if (innerMap.ContainsKey(node) && innerMap[node].ContainsKey(name))
                return innerMap[node][name];
            return null;
        }

        private Dictionary<string, Directive> RetrieveDirectivesFor(CObject node)
        {
            Dictionary<string, Directive> directivesIndex;
            if (innerMap.ContainsKey(node) && innerMap[node] != null)
            {
                directivesIndex = innerMap[node];
            }
            else
            {
                directivesIndex = new Dictionary<string, Directive>();
                innerMap[node] = directivesIndex;
            }
            return directivesIndex;
        }

        public void AddDirectiveFor(CObject node, Directive directive)
        {
            RetrieveDirectivesFor(node)[directive.Name] = directive;
        }

        public void RemoveDirectiveFor(CObject node, Directive directive)
        {
            if (!innerMap.ContainsKey(node))
                return;
            Dictionary<string, Directive> directives = innerMap[node];
            if (directives != null)
            {
                directives.Remove(directive.Name);
                if (directives.Count == 0)
                    innerMap.Remove(node);
            }
        }
    }

    public class Directive
    {
        /// <summary>
        /// The general format of directives goes like:
        ///   name ( arg1, arg2, arg3, ... argn )
        /// where the arguments are optional
        /// </summary>
        public static readonly Regex Format = new Regex(@"(\w+)\s*(?:\(\s*(\w+)\s*(?:,\s*(\w+))*\s*\))?");

        //String constants denoting known directive names
        //Number of expected parameters is implicit - something to work on
        public const string CoreConcept = "isCoreConcept";
        public const string IsOrganiser = "isOrganiser";
        public const string HideChildren = "hideChildren";
        public const string HideOnGui = "hideOnGUI";
        public const string AllowChildrenSplit = "allowChildrenSplit";
        public const string ShowAs = "showAs";
        public const string Break = "break";
        public const string ShowWithParent = "showWithParent";

        private string name;
        private string[] parameters;

        public Directive(string name, params string[] parameters)
        {
            this.name = name;
            this.parameters = parameters;
        }

        public string Name
        {
            get { return name; }
        }

        public string[] Parameters
        {
            get { return parameters; }
        }

        public bool ParamAsBool(int index)
        {
            return bool.Parse(ParamAt(index));
        }

        public int ParamAsInt(int index)
        {
            return int.Parse(ParamAt(index));
        }

        public string ParamAt(int index)
        {
            if (index >= parameters.Length)
                return null;
            return parameters[index];
        }

        /// <summary>
        /// TODO add validation check
        /// </summary>
        /// <param name="coded"></param>
        /// <returns></returns>
        public static Directive[] Parse(string coded)
        {
            MatchCollection matches = Format.Matches(coded);
            Directive[] directives = new Directive[matches.Count];
            for (int i = 0; i < directives.Length; i ++)
            {
                List<string> arguments = new List<string>();
                Match m = matches[i];
                if (m.Groups.Count < 4)
                {
                    //this shouldn't happen unless regular expression has changed
                    continue;
                }
                string name = m.Groups[1].Value; //first group denotes the name of the directive
                //next group denotes first argument
                if (!string.IsNullOrEmpty(m.Groups[2].Value))
                    arguments.Add(m.Groups[2].Value);
                
                //then the next group captures denote rest of the arguments
                foreach (Capture capture in m.Groups[3].Captures)
                    arguments.Add(capture.Value);

                directives[i] = new Directive(name, arguments.ToArray());
            }
            return directives;
        }
    }
}