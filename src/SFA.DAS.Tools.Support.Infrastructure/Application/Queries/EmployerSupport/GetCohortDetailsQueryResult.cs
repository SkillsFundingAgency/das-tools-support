using SFA.DAS.Tools.Support.Core.Models;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetCohortDetailsQueryResult
{
    public long CohortId { get; set; }
    public string HashedAccountId { get; set; }
    public string EmployerAccountName { get; set; }
    public string ProviderName { get; set; }
    public long UkPrn { get; set; }
    public string CohortReference { get; set; }
    public string CohortStatus { get; set; }

    public List<ApprovedApprenticeshipCohortSummary> Apprenticeships { get; set; } = new ();
}