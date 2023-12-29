namespace GastrOs.Sde.Directives
{
    public class OrganiserDirective : IDirective
    {
        public string Name
        {
            get { return "isOrganiser"; }
        }

        public void ParseParameters(params string[] parameters)
        {
        }
    }
}