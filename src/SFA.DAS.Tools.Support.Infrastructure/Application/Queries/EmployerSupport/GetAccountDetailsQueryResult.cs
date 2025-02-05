using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryResult
{
    public Account Account { get; set; }

    public static GetAccountDetailsQueryResult MapFrom(GetAccountDetailsResponse source)
    {
        return new GetAccountDetailsQueryResult { Account = source.Account };
    }
}
