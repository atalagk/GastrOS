using System;

namespace GastrOs.Sde.Directives
{
    public enum ShowTextMode { Multi, Smart }

    public class ShowTextDirective : IDirective
    {
        private const string Pmulti = "multi";
        private const string Psmart = "smart";

        private ShowTextMode mode;

        public string Name
        {
            get { return "showText"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '" + Name +
                                          "' must have one parameter specifying the show mode (either '"+
                                          Pmulti+"' or '"+Psmart+"').");

            if (string.Equals(parameters[0], Pmulti, StringComparison.OrdinalIgnoreCase))
                mode = ShowTextMode.Multi;
            else if (string.Equals(parameters[0], Psmart, StringComparison.OrdinalIgnoreCase))
                mode = ShowTextMode.Smart;
            else
                throw new FormatException("The GUI directive '" + Name +
                                          "' can't have '"+parameters[0]+"' as its first parameter. Either '" +
                                          Pmulti+"' or '"+Psmart+"' is expected.");
        }

        public ShowTextMode Mode
        {
            get { return mode; }
        }
    }
}