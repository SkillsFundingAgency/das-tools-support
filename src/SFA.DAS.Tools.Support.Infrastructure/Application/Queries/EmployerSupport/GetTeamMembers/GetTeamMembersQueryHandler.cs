using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetTeamMembers;

public class GetTeamMembersQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetTeamMembersQuery, GetTeamMembersQueryResult>
{
    public async Task<GetTeamMembersQueryResult> Handle(GetTeamMembersQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);
        var teamMembers = await employerSupportApiClient.GetTeamMembers(accountId, cancellationToken);

        return (GetTeamMembersQueryResult)teamMembers;
    }
}