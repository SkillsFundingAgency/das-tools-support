namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class Declaration
{
    public string SubmissionDate { get; set; }
    public string PayrollDate { get; set; }
    public string LevySubmissionId { get; set; }
    public string LevyDeclarationDescription { get; set; }
    public string YearToDateAmount { get; set; }
    public LevyDeclarationSubmissionStatus SubmissionStatus { get; set; }
    public string PortalLink { get; set; }
}
public enum LevyDeclarationSubmissionStatus
{
    UnprocessedSubmission,
    LateSubmission,
    LatestSubmission
}