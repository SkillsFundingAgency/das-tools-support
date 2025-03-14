using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;

public class GetUserOverviewQueryHandler(IToolsSupportApimService employerSupportApiClient) : IRequestHandler<GetUserOverviewQuery, GetUserOverviewQueryResult>
{
    public async Task<GetUserOverviewQueryResult> Handle(GetUserOverviewQuery query, CancellationToken cancellationToken)
    {
        var userOverview = await employerSupportApiClient.GetUserOverview(query.UserId, cancellationToken);

        return (GetUserOverviewQueryResult)userOverview;
    }
}
