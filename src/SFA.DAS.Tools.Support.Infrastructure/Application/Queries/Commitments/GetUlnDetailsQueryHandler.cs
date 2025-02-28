using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;

public class GetUlnDetailsQueryHandler(IToolsSupportApimService employerSupportApiClient) : IRequestHandler<GetUlnDetailsQuery, GetUlnDetailsQueryResult>
{
    public async Task<GetUlnDetailsQueryResult> Handle(GetUlnDetailsQuery query, CancellationToken cancellationToken)
    {
        var result = new GetUlnDetailsQueryResult();

        var response = await employerSupportApiClient.GetMatchingUlns(query.Uln, cancellationToken);
        result.Apprenticeships = response.ApprovedApprenticeships;

        return result;
    }
}