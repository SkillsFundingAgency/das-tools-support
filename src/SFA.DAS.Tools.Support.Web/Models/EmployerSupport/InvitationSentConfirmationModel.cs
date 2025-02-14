namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class InvitationSentConfirmationModel
{
    public bool Success { get; set; }
    public string MemberEmail { get; set; }
    public string HashedAccountId { get; set; }
}
