using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class FinanceViewModel : AccountDetailsBaseViewModel
{
    public IEnumerable<PayeScheme> PayeSchemes { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
    public decimal Balance { get; set; }

}
