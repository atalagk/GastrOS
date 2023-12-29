using System.Collections.Generic;
using System.Text;
using GastrOs.Sde.Support;

namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents a list of diagnoses for a single organ
    /// </summary>
    public class OrganDiagnoses : MstItem
    {
        private List<string> diagnoses;

        public OrganDiagnoses(string displayName) : base(displayName)
        {
        }

        public IList<string> Diagnoses
        {
            get
            {
                if (diagnoses == null)
                    diagnoses = new List<string>();
                return diagnoses;
            }
        }

        public string FreeText { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(DisplayName).Append(": ");
            sb.Append(Diagnoses.ToPrettyString(", "));
            if (!string.IsNullOrEmpty(FreeText))
            {
                sb.Append("\r\n    ").Append(FreeText);
            }

            return sb.ToString();
        }
    }
}