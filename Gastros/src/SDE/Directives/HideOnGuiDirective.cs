namespace GastrOs.Sde.Directives
{
    public class HideOnGuiDirective : IDirective
    {
        public string Name
        {
            get { return "hideOnGUI"; }
        }

        public void ParseParameters(params string[] parameters)
        {
        }
    }
}