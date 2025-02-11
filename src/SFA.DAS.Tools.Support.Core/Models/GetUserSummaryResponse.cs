using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetUserSummaryResponse
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public bool IsSuspended { get; set; }
    public List<AccountSummary> AccountSummaries { get; set; }
}

public class AccountSummary
{
    public string DasAccountName { get; set; }
    public string HashedAccountId { get; set; }
}
