using System;
using System.Text.RegularExpressions;

namespace GastrOs.Sde.Directives
{
    public enum ItemAlignment { Vertical, Horizontal }

    /// <summary>
    /// Used to specify an alternate visual representation to an element. Currently
    /// only applies to coded_text type values. Signature:
    /// 
    /// alternateStyle(align=[horizontal|vertical], border=[true|false], label=[true|false])
    /// </summary>
    public class AlternateStyleDirective : IDirective
    {
        private static readonly Regex alignExpr = new Regex(@"align\s*=\s*(horizontal|vertical)",
                                                            RegexOptions.IgnoreCase);
        private static readonly Regex borderExpr = new Regex(@"border\s*=\s*(true|false)",
                                                            RegexOptions.IgnoreCase);
        private static readonly Regex labelExpr = new Regex(@"label\s*=\s*(true|false)",
                                                            RegexOptions.IgnoreCase);

        private ItemAlignment alignment;
        private bool showBorder;
        private bool showLabel;

        public string Name
        {
            get { return "alternateStyle"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            foreach (string param in parameters)
            {
                Match alignmatch = alignExpr.Match(param);
                if (alignmatch.Success)
                {
                    //this should ideally work without exceptions, since the
                    //captured group should only comprise digits
                    alignment = string.Equals(alignmatch.Groups[1].Value, "horizontal")
                                    ? ItemAlignment.Horizontal
                                    : ItemAlignment.Vertical; //vertical is default
                    continue;
                }
                Match bordermatch = borderExpr.Match(param);
                if (bordermatch.Success)
                {
                    //this should ideally work without exceptions, since the
                    //captured group should only be true or false
                    showBorder = bool.Parse(bordermatch.Groups[1].Value);
                    continue;
                }
                Match labelmatch = labelExpr.Match(param);
                if (labelmatch.Success)
                {
                    //this should ideally work without exceptions, since the
                    //captured group should only be true or false
                    showLabel = bool.Parse(labelmatch.Groups[1].Value);
                    continue;
                }
                throw new FormatException("The GUI directive '" + Name +
                                          "' can't have '" + param + "' as its parameter. Please check that " +
                                          "it's formatted correctly");
            }
        }

        public ItemAlignment Alignment
        {
            get { return alignment; }
        }

        public bool ShowBorder
        {
            get { return showBorder; }
        }

        public bool ShowLabel
        {
            get { return showLabel; }
        }
    }
}
