using System;

namespace GastrOs.Sde.Directives
{
    public enum ShowWithParentMode { Simple, Merged }

    public class ShowWithParentDirective : IDirective
    {
        private const string Psimple = "simple";
        private const string Pmerged = "merged";

        private ShowWithParentMode mode;

        public string Name
        {
            get { return "showWithParent"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '" + Name +
                                          "' must have one parameter specifying the show mode (either "+
                                          Psimple+" or "+Pmerged+").");

            if (string.Equals(parameters[0], Psimple, StringComparison.OrdinalIgnoreCase))
                mode = ShowWithParentMode.Simple;
            else if (string.Equals(parameters[0], Pmerged, StringComparison.OrdinalIgnoreCase))
                mode = ShowWithParentMode.Merged;
            else
                throw new FormatException("The GUI directive '" + Name +
                                          "' can't have '"+parameters[0]+"' as its first parameter. Either " +
                                          Psimple+" or "+Pmerged+" is expected.");
        }

        public ShowWithParentMode Mode
        {
            get { return mode; }
        }
    }
}