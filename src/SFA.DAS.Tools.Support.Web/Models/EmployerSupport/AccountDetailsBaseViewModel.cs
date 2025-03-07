using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class AccountDetailsBaseViewModel
{
    public Account Account { get; set; }
    public AccountFieldsSelection SelectedTab { get; set; }
}
