using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryHandler(IEmployerSupportApiClient employerSupportApiClient) : IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>
{
    public async Task<GetAccountDetailsQueryResult> Handle(GetAccountDetailsQuery query, CancellationToken cancellationToken)
    {
        var applicationsResponse = await employerSupportApiClient.GetAccountDetails(query.AccountHashedId, query.AccountFieldsSelection.ToString(), cancellationToken);

        return GetAccountDetailsQueryResult.MapFrom(applicationsResponse);
    }
}
