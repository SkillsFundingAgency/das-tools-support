using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class AccountDetailsViewModel
{
    public Account Account { get; set; }
    public AccountFieldsSelection SelectedTab { get; set; }

    public static AccountDetailsViewModel MapFrom(GetAccountDetailsQueryResult source)
    {
        if (source == null || source.Account == null)
        {
            return new AccountDetailsViewModel { Account = new Account() };
        }
        return new AccountDetailsViewModel { Account = source.Account };
    }
}
