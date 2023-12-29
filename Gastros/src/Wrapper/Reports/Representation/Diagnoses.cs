using System.Collections.Generic;
using GastrOs.Sde.Support;

namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents diagnosis information for a collection
    /// of organs associated with an endoscopy type.
    /// </summary>
    public class Diagnoses
    {
        private List<OrganDiagnoses> organs;

        public IList<OrganDiagnoses> Organs
        {
            get
            {
                if (organs == null)
                    organs = new List<OrganDiagnoses>();
                return organs;
            }
        }

        public override string ToString()
        {
            return organs.ToPrettyString("\r\n");
        }
    }
}