using System;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryResult
{
    public Account Account { get; set; }

    public static explicit operator GetAccountDetailsQueryResult(GetAccountDetailsResponse source)
    {
        if (source == null || source.Account == null)
        {
            return new GetAccountDetailsQueryResult { Account = new Account() };
        }
        return new GetAccountDetailsQueryResult { Account = source.Account };
    }

}
