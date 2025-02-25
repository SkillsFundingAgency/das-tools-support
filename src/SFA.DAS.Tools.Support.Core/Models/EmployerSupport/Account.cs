using System;

namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class Account
{
    public string HashedAccountId { get; set; }
    public string PublicHashedAccountId { get; set; }
    public string DasAccountName { get; set; }
    public DateTime DateRegistered { get; set; }
    public string ApprenticeshipEmployerType { get; set; }    
}
