using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountOrganisations;

public class GetAccountOrganisationsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetAccountOrganisationsQuery, GetAccountOrganisationsQueryResult>
{
    public async Task<GetAccountOrganisationsQueryResult> Handle(GetAccountOrganisationsQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);
        var accountDetailsResponse = await employerSupportApiClient.GetAccountOrganisations(accountId, cancellationToken);

        return (GetAccountOrganisationsQueryResult)accountDetailsResponse;
    }
}