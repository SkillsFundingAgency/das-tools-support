using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class AccountDetailsViewModel
{
    public Core.Models.EmployerSupport.Account Account { get; set; }
    public AccountFieldsSelection SelectedTab { get; set; }

    public static AccountDetailsViewModel MapFrom(GetAccountDetailsQueryResult source)
    {
        return new AccountDetailsViewModel { Account = source.Account };
    }
}
