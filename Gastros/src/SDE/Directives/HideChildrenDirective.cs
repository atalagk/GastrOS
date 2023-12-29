namespace GastrOs.Sde.Directives
{
    public class HideChildrenDirective : IDirective
    {
        public string Name
        {
            get { return "hideChildren"; }
        }

        public void ParseParameters(params string[] parameters)
        {
        }
    }
}