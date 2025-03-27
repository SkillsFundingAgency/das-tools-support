using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;

public class GetUserOverviewQueryResult
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public bool IsSuspended { get; set; }

    public List<AccountSummary> AccountSummaries { get; set; }

    public static explicit operator GetUserOverviewQueryResult(GetUserOverviewResponse source)
    {
        if (source == null)
        {
            return new GetUserOverviewQueryResult();
        }

        return new GetUserOverviewQueryResult
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email,
            IsActive = source.IsActive,
            IsLocked = source.IsLocked,
            IsSuspended = source.IsSuspended,
            AccountSummaries = source.AccountSummaries
        };
    }
}
