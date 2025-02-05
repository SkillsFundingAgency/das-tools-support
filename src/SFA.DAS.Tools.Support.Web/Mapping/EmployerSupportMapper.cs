using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Mapping;

public static class EmployerSupportMapper
{
    public static AccountDetailsViewModel MapToAccountDetailsViewModel(this GetAccountDetailsQueryResult response) => new()
    {
        Account = response.Account
    };
}
