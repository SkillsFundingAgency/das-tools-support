using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class Account
{
    public Account()
    {
        LegalEntities = [];
    }

    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public string PublicHashedAccountId { get; set; }
    public string DasAccountName { get; set; }
    public DateTime DateRegistered { get; set; }
    public string OwnerEmail { get; set; }
    public decimal Balance { get; set; }

    public IEnumerable<LegalEntity> LegalEntities { get; set; }
    public IEnumerable<PayeScheme> PayeSchemes { get; set; }
    public ICollection<TeamMember> TeamMembers { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
    public string ApprenticeshipEmployerType { get; set; }
}
