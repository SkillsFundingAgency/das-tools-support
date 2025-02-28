using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetCohortDetailsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetCohortDetailsQuery, GetCohortDetailsQueryResult>
{
    public async Task<GetCohortDetailsQueryResult> Handle(GetCohortDetailsQuery query, CancellationToken cancellationToken)
    {
        var id = encodingService.Decode(query.CohortRef.ToUpper(), EncodingType.CohortReference);

        var response = await employerSupportApiClient.GetCohort(id, cancellationToken);

        if (response == null)
            return null;

        var result = new GetCohortDetailsQueryResult
        {
            CohortId = response.CohortId,
            CohortReference = response.CohortReference,
            HashedAccountId = encodingService.Encode(response.EmployerAccountId, EncodingType.AccountId),
            EmployerAccountName = response.EmployerAccountName,
            ProviderName = response.ProviderName,
            UkPrn = response.UkPrn,
            CohortStatus = response.CohortStatus,
            Apprenticeships = response.ApprovedApprenticeships
        };

        return result;
    }
}