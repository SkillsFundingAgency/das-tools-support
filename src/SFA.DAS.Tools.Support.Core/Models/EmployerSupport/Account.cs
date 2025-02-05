using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class Account
{
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public string PublicHashedAccountId { get; set; }
    public string DasAccountName { get; set; }
    public DateTime DateRegistered { get; set; }
    public string OwnerEmail { get; set; }
    public IEnumerable<LegalEntity> LegalEntities { get; set; }

    //public IEnumerable<PayeSchemeModel> PayeSchemes { get; set; }
    //public ICollection<TeamMemberViewModel> EmployerAccountTeam { get; set; }
    //public IEnumerable<TransactionViewModel> Transactions { get; set; }
    public string ApprenticeshipEmployerType { get; set; }
}
