using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GastrOs.Sde.Directives
{
    public static class DirectiveHelper
    {
        public static readonly Regex Format = new Regex(@"(\w+)\s*(?:\(\s*([^,\(\)]+)\s*(?:,\s*([^,\(\)]+))*\s*\))?");

        private static readonly Dictionary<string, Type> directiveTypeMap;

        static DirectiveHelper()
        {
            directiveTypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            directiveTypeMap["isCoreConcept"] = typeof(CoreConceptDirective);
            directiveTypeMap["isOrganiser"] = typeof(OrganiserDirective);
            directiveTypeMap["hideChildren"] = typeof(HideChildrenDirective);
            directiveTypeMap["hideOnGUI"] = typeof(HideOnGuiDirective);
            directiveTypeMap["showInstances"] = typeof(ShowInstancesDirective);
            directiveTypeMap["showAs"] = typeof(ShowAsDirective);
            directiveTypeMap["break"] = typeof(BreakDirective);
            directiveTypeMap["showWithParent"] = typeof(ShowWithParentDirective);
            directiveTypeMap["showValueContext"] = typeof(ShowValueContextDirective);
            directiveTypeMap["showText"] = typeof(ShowTextDirective);
            directiveTypeMap["formAspects"] = typeof(FormAspectsDirective);
            directiveTypeMap["showDescription"] = typeof(ShowDescriptionDirective);
            directiveTypeMap["alternateStyle"] = typeof(AlternateStyleDirective);
            directiveTypeMap["grid"] = typeof(GridDirective);
        }

        /// <summary>
        /// The general format of directives goes like:
        ///   name ( arg1, arg2, arg3, ... argn )
        /// where the arguments are optional
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDirective[] Parse(string code)
        {
            MatchCollection matches = Format.Matches(code);
            IDirective[] directives = new IDirective[matches.Count];
            for (int i = 0; i < directives.Length; i++)
            {
                List<string> arguments = new List<string>();
                Match m = matches[i];
                if (m.Groups.Count < 4)
                {
                    //this shouldn't happen unless regular expression has changed
                    continue;
                }
                string name = m.Groups[1].Value; //first group denotes the name of the directive

                if (!directiveTypeMap.ContainsKey(name))
                {
                    throw new ArgumentException("Unknown GUI directive name '"+name+"'");
                }

                //next group denotes first argument
                if (!string.IsNullOrEmpty(m.Groups[2].Value))
                    arguments.Add(m.Groups[2].Value);

                //then the next group captures denote rest of the arguments
                foreach (Capture capture in m.Groups[3].Captures)
                    arguments.Add(capture.Value);

                Type baseType = typeof(IDirective);
                Type directiveType = directiveTypeMap[name];
                //try dynamically constructing the directive instance
                ConstructorInfo directiveConst = directiveType.GetConstructor(new Type[0]);
                if (!baseType.IsAssignableFrom(directiveType) || !directiveType.IsClass ||
                    directiveType.IsAbstract || directiveConst == null)
                {
                    throw new InvalidOperationException("The type " + directiveType.Name +
                                                        " associated with the '" + name +
                                                        "' directive must be a concrete"+
                                                        " subclass of " + baseType.Name +
                                                        " and define an empty public constructor");
                }
                IDirective directive = (IDirective)directiveConst.Invoke(null);
                directive.ParseParameters(arguments.ToArray());

                directives[i] = directive;
            }
            return directives;
        }
        
        /// <summary>
        /// Returns whether a directive of specified type exists within the specified
        /// collection of directives
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <param name="directives"></param>
        /// <returns></returns>
        public static bool HasDirectiveOfType<TD>(this IEnumerable<IDirective> directives) where TD : class, IDirective
        {
            return GetDirectiveOfType<TD>(directives) != null;
        }

        /// <summary>
        /// Returns the first instance (if any) of a directive of specified type
        /// from specified collection of directives
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <param name="directives"></param>
        /// <returns></returns>
        public static TD GetDirectiveOfType<TD>(this IEnumerable<IDirective> directives) where TD : class, IDirective
        {
            foreach (IDirective directive in directives)
            {
                if (directive is TD)
                    return directive as TD;
            }
            return null;
        }

        /// <summary>
        /// Removes all instances of directives of specified type
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <param name="directives"></param>
        public static void RemoveDirectivesOfType<TD>(this ICollection<IDirective> directives) where TD : class, IDirective
        {
            foreach (IDirective directive in new List<IDirective>(directives))
            {
                if (directive is TD)
                    directives.Remove(directive);
            }
        }
    }
}