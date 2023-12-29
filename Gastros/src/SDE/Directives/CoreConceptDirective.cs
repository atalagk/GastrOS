namespace GastrOs.Sde.Directives
{
    public class CoreConceptDirective : IDirective
    {
        public string Name
        {
            get { return "isCoreConcept"; }
        }

        public void ParseParameters(params string[] parameters)
        {
        }
    }
}
