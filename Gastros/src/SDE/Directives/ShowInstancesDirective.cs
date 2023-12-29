using System;

namespace GastrOs.Sde.Directives
{
    public class ShowInstancesDirective : IDirective
    {
        private int instances;

        public string Name
        {
            get { return "showInstances"; }
        }

        public void ParseParameters(params string[] parameters)
        {
            if (parameters.Length < 1)
                throw new FormatException("The GUI directive '"+Name+
                    "' must have one parameter specifying the number of instances.");
            if (!int.TryParse(parameters[0], out instances))
                throw new FormatException("The GUI directive '" + Name +
                    "' can't have '"+parameters[0]+"' as its parameter; an integer is expected.");
        }

        public int Instances
        {
            get { return instances; }
        }
    }
}