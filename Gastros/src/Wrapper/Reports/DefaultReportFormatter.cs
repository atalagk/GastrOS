using GastrOs.Wrapper.Reports.Representation;

namespace GastrOs.Wrapper.Reports
{
    public class DefaultReportFormatter : IReportFormatter
    {
        public string FormatFindings(Findings findings)
        {
            return findings.ToString();
        }

        public string FormatInterventions(Findings findings)
        {
            return findings.ToString(true);
        }

        public string FormatDiagnoses(Diagnoses dx)
        {
            return dx.ToString();
        }

        public string FormatExamInfo(ExamInfo examInfo)
        {
            return examInfo.ToString();
        }
    }
}
