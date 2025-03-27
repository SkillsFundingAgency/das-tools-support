using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetChallengePermission;

public class GetChallengePermissionQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) 
    : IRequestHandler<GetChallengePermissionQuery, GetChallengePermissionQueryResult>
{
    public async Task<GetChallengePermissionQueryResult> Handle(GetChallengePermissionQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);
        var challengeResponse = await employerSupportApiClient.GetChallengePermission(accountId, cancellationToken);

        return (GetChallengePermissionQueryResult)challengeResponse;
    }
}