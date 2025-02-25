using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetFinanceDetails;

public class GetFinanceDetailsQueryResult
{
    public IEnumerable<PayeScheme> PayeSchemes { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
    public decimal Balance { get; set; }

    public static explicit operator GetFinanceDetailsQueryResult(GetFinanceDataResponse source)
    {
        if (source == null)
        {
            return new GetFinanceDetailsQueryResult
            {
                PayeSchemes = [],
                Transactions = []
            };
        }
        return new GetFinanceDetailsQueryResult
        {
            PayeSchemes = source.PayeSchemes,
            Transactions = source.Transactions,
            Balance = source.Balance
        };
    }
}
