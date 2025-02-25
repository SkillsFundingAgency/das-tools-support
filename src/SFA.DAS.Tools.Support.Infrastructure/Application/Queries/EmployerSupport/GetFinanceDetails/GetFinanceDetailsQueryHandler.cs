using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetFinanceDetails;

public class GetFinanceDetailsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetFinanceDetailsQuery, GetFinanceDetailsQueryResult>
{
    public async Task<GetFinanceDetailsQueryResult> Handle(GetFinanceDetailsQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);
        var accountDetailsResponse = await employerSupportApiClient.GetFinanceDetails(accountId, cancellationToken);

        return (GetFinanceDetailsQueryResult)accountDetailsResponse;
    }
}