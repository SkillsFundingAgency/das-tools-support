using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetChallengePermissionResponse
{
    public Account Account { get; set; }

    public List<int> Characters { get; set; }

    public SearchResponseCodes StatusCode { get; set; }
}

public enum SearchResponseCodes
{
    NoSearchResultsFound = 0,
    Success = 1,
    SearchFailed = 2
}