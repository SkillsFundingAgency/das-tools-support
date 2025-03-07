using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetFinanceDataResponse
{
    public IEnumerable<PayeScheme> PayeSchemes { get; set; } = [];
    public IEnumerable<Transaction> Transactions { get; set; } = [];
    public decimal Balance { get; set; }
}
