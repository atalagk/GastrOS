using System;

namespace GastrOs.Sde.Directives
{
    public enum ShowAsViewType { Form, Splash }
    public enum ShowAsViewBehaviour { Modal, Modeless, Smart}

    public class ShowAsDirective : IDirective
    {
        private const string Pform = "form";
        private const string Psplash = "splash";
        private const string Pmodal = "modal";
        private const string Pmodeless = "modeless";
        private const string Psmart = "smart";

        private ShowAsViewType viewType;
        private ShowAsViewBehaviour viewBehaviour;

        public string Name
        {
            get { return "showAs"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '" + Name +
                    "' must have at least one parameter specifying the view type (either "+
                    Pform+" or "+Psplash+").");
            
            if (string.Equals(parameters[0], Pform, StringComparison.OrdinalIgnoreCase))
                viewType = ShowAsViewType.Form;
            else if (string.Equals(parameters[0], Psplash, StringComparison.OrdinalIgnoreCase))
                viewType = ShowAsViewType.Splash;
            else
                throw new FormatException("The GUI directive '" + Name +
                    "' can't have '"+parameters[0]+"' as its first parameter. Either " +
                    Pform+" or "+Psplash+" is expected.");

            if (parameters.Length < 2)
                return;

            if (string.Equals(parameters[1], Pmodal, StringComparison.OrdinalIgnoreCase))
                viewBehaviour = ShowAsViewBehaviour.Modal;
            else if (string.Equals(parameters[1], Pmodeless, StringComparison.OrdinalIgnoreCase))
                viewBehaviour = ShowAsViewBehaviour.Modeless;
            else if (string.Equals(parameters[1], Psmart, StringComparison.OrdinalIgnoreCase))
                viewBehaviour = ShowAsViewBehaviour.Smart;
            else
                throw new FormatException("The GUI directive '" + Name +
                    "' can't have '" + parameters[1] + "' as its second parameter. Either " +
                    Pmodal + ", " + Pmodeless + " or " + Psmart + " is expected.");
        }

        public ShowAsViewType ViewType
        {
            get { return viewType; }
        }

        public ShowAsViewBehaviour ViewBehaviour
        {
            get { return viewBehaviour; }
        }
    }
}