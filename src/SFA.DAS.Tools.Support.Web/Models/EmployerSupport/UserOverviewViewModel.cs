using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class UserOverviewViewModel
{
    public string Id { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public bool IsSuspended { get; set; }
    public List<AccountSummary> AccountSummaries { get; set; } = [];

    public UserStatus Status => DetermineStatus();

    private UserStatus DetermineStatus()
    {
        if (IsSuspended)
            return UserStatus.Suspended;
        if (IsActive && IsLocked)
            return UserStatus.Locked;
        if (IsActive && IsLocked == false)
            return UserStatus.Active;

        return UserStatus.Unverified;
    }

    public static UserOverviewViewModel MapFrom(GetUserOverviewQueryResult source)
    {
        if (source == null)
        {
            return new UserOverviewViewModel();
        }

        return new UserOverviewViewModel
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
