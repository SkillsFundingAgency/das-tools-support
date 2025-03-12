using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountOrganisations;

public class GetAccountOrganisationsQueryResult
{
    public IEnumerable<LegalEntity> LegalEntities { get; set; }

    public static explicit operator GetAccountOrganisationsQueryResult(GetAccountOrganisationsResponse source)
    {
        if (source == null || source.LegalEntities == null)
        {
            return new GetAccountOrganisationsQueryResult { LegalEntities = [] };
        }
        return new GetAccountOrganisationsQueryResult { LegalEntities = source.LegalEntities };
    }
}
