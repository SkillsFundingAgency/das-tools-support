using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class OrganisationViewModel: AccountDetailsBaseViewModel
{
    public IEnumerable<LegalEntity> LegalEntities { get; set; }
}
