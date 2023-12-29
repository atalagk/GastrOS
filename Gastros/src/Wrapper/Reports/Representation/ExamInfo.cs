using System.Text;

namespace GastrOs.Wrapper.Reports.Representation
{
    /// <summary>
    /// Represents examination info for an endoscopy type
    /// </summary>
    public class ExamInfo
    {
        public Attribute ExtentSite { get; set; }
        public Attribute PreparationQuality { get; set; }
        public Attribute PreparationSite { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (ExtentSite != null)
                builder.Append(ExtentSite);
            if (PreparationQuality != null)
                builder.Append("\r\n").Append(PreparationQuality);
            if (PreparationSite != null)
                builder.Append("\r\n").Append(PreparationSite);
            return builder.ToString();
        }
    }
}