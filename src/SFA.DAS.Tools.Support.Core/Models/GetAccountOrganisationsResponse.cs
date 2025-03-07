using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetAccountOrganisationsResponse
{
    public IEnumerable<LegalEntity> LegalEntities { get; set; }
}
