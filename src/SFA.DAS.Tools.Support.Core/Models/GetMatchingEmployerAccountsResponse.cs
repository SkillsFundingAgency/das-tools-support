using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetMatchingEmployerAccountsResponse
{
    public List<EmployerAccount> Accounts { get; set; } = new();
}

public class EmployerAccount
{
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public string PublicHashedAccountId { get; set; }
    public string DasAccountName { get; set; }
}