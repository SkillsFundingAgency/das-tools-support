using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;

public class GetAccountDetailsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>
{
    public async Task<GetAccountDetailsQueryResult> Handle(GetAccountDetailsQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);
        var accountDetailsResponse = await employerSupportApiClient.GetAccountDetails(accountId, cancellationToken);

        return (GetAccountDetailsQueryResult)accountDetailsResponse;
    }
}
