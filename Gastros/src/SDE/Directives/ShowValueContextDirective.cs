using System;

namespace GastrOs.Sde.Directives
{
    public enum ShowValueContextMode { Normal, Append, Organise }

    public class ShowValueContextDirective : IDirective
    {
        private const string Pappend = "append";
        private const string Porganise = "organise";

        private ShowValueContextMode mode;

        public string Name
        {
            get { return "showValueContext"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '" + Name +
                                          "' must have one parameter specifying the mode (either '"+
                                          Pappend+"' or '"+Porganise+"').");

            if (string.Equals(parameters[0], Pappend, StringComparison.OrdinalIgnoreCase))
                mode = ShowValueContextMode.Append;
            else if (string.Equals(parameters[0], Porganise, StringComparison.OrdinalIgnoreCase))
                mode = ShowValueContextMode.Organise;
            else
                throw new FormatException("The GUI directive '" + Name +
                                          "' can't have '"+parameters[0]+"' as its first parameter. Either '" +
                                          Pappend+"' or '"+Porganise+"' is expected.");
        }

        public ShowValueContextMode Mode
        {
            get { return mode; }
        }
    }
}