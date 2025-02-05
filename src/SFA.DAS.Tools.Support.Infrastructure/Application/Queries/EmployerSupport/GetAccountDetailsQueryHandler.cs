using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryHandler(IEmployerSupportService employerSupportService) : IRequestHandler<GetAccountDetailsQuery, GetAccountDetailsQueryResult>
{
    public async Task<GetAccountDetailsQueryResult> Handle(GetAccountDetailsQuery query, CancellationToken cancellationToken)
    {
        var applicationsResponse = await employerSupportService.GetAccountDetails(query.AccountHashedId, query.AccountFieldsSelection.ToString(), cancellationToken);

        return GetAccountDetailsQueryResult.MapFrom(applicationsResponse);
    }
}
