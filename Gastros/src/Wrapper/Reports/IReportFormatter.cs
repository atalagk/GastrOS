using GastrOs.Wrapper.Reports.Representation;

namespace GastrOs.Wrapper.Reports
{
    public interface IReportFormatter
    {
        string FormatFindings(Findings findings);
        string FormatInterventions(Findings findings);
        string FormatDiagnoses(Diagnoses dx);
        string FormatExamInfo(ExamInfo examInfo);
    }
}
