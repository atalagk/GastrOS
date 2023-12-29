namespace GastrOs.Sde.Directives
{
    public class ShowDescriptionDirective : IDirective
    {
        public string Name
        {
            get { return "showDescription"; }
        }

        public void ParseParameters(params string[] parameters)
        {
        }
    }
}