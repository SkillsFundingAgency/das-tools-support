using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryHandler(IEmployerSupportApiClient employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>
{
    public async Task<GetAccountDetailsQueryResult> Handle(GetAccountDetailsQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.AccountHashedId, EncodingType.AccountId);
        var applicationsResponse = await employerSupportApiClient.GetAccountDetails(accountId, query.AccountFieldsSelection.ToString(), cancellationToken);

        return GetAccountDetailsQueryResult.MapFrom(applicationsResponse);
    }
}
