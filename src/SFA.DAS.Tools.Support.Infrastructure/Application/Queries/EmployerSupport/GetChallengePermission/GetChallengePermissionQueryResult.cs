using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetChallengePermission;

public class GetChallengePermissionQueryResult
{
    public Account Account { get; set; }

    public List<int> Characters { get; set; } = [];

    public SearchResponseCodes StatusCode { get; set; }


    public static explicit operator GetChallengePermissionQueryResult(GetChallengePermissionResponse source)
    {
        if (source == null)
        {
            return new GetChallengePermissionQueryResult();
        }
        return new GetChallengePermissionQueryResult
        {
            Account = source.Account,
            Characters = source.Characters,
            StatusCode = source.StatusCode,
        };
    }
}
