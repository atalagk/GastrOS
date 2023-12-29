using System;
using System.Text.RegularExpressions;

namespace GastrOs.Sde.Directives
{
    public class FormAspectsDirective : IDirective
    {
        private static readonly Regex widthExpr = new Regex(@"width\s*=\s*(\d+)", RegexOptions.IgnoreCase);
        private static readonly Regex heightExpr = new Regex(@"height\s*=\s*(\d+)", RegexOptions.IgnoreCase);

        private int width, height;
        
        public string Name
        {
            get { return "formAspects"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '" + Name +
                                          "' must have at least one parameter specifying width or height.");
            
            foreach (string param in parameters)
            {
                Match widthmatch = widthExpr.Match(param);
                if (widthmatch.Success)
                {
                    //this should ideally work without exceptions, since the
                    //captured group should only comprise digits
                    width = int.Parse(widthmatch.Groups[1].Value);
                    continue;
                }
                Match heightmatch = heightExpr.Match(param);
                if (heightmatch.Success)
                {
                    //this should ideally work without exceptions, since the
                    //captured group should only comprise digits
                    height = int.Parse(heightmatch.Groups[1].Value);
                    continue;
                }
                throw new FormatException("The GUI directive '" + Name +
                                          "' can't have '" + param + "' as its parameter. Please check that "+
                                          "it's formatted correctly");
            }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }
    }
}