using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetEmployerAccountsQueryResult
{
    public bool IsValidAccountCode { get; set; }
    public List<EmployerAccount> Accounts { get; set; }
}
