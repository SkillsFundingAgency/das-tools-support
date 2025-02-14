﻿namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class EmployerAccountSearchModel
{
    public string PublicHashedId { get; set; }
    public string PayeRef { get; set; }
    public List<MatchedAccount> Accounts { get; set; }
}

public class MatchedAccount
{
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public string PublicHashedAccountId { get; set; }
    public string DasAccountName { get; set; }
}