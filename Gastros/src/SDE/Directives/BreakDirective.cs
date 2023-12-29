using System;

namespace GastrOs.Sde.Directives
{
    public enum BreakStyle { None, Next, Tab, Parent }

    public class BreakDirective : IDirective
    {
        private const string Pnext = "next";
        private const string Ptab = "tab";
        private const string Pparent = "parent";

        private BreakStyle breakStyle;

        /// <summary>
        /// Empty constructor needed to allow auto-instantiation
        /// </summary>
        public BreakDirective() {}

        public BreakDirective(BreakStyle breakStyle)
        {
            this.breakStyle = breakStyle;
        }

        public string Name
        {
            get { return "break"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '" + Name +
                                          "' must have one parameter specifying the break style (either "+
                                          Pnext+", "+Pparent+" or "+Ptab+").");

            if (string.Equals(parameters[0], Pnext, StringComparison.OrdinalIgnoreCase))
                breakStyle = BreakStyle.Next;
            else if (string.Equals(parameters[0], Ptab, StringComparison.OrdinalIgnoreCase))
                breakStyle = BreakStyle.Tab;
            else if (string.Equals(parameters[0], Pparent, StringComparison.OrdinalIgnoreCase))
                breakStyle = BreakStyle.Parent;
            else
                throw new FormatException("The GUI directive '" + Name +
                                          "' can't have '" + parameters[0] + "' as its first parameter. Either " +
                                          Pnext + ", " + Pparent + " or " + Ptab + " is expected.");
        }

        public BreakStyle BreakStyle
        {
            get { return breakStyle; }
        }

        public override bool Equals(object obj)
        {
            if (obj is BreakDirective)
            {
                return breakStyle == ((BreakDirective) obj).breakStyle;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return breakStyle.GetHashCode();
        }
    }
}