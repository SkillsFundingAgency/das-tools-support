using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;

public class GetPayeSchemeLevyDeclarationsQueryHandler(IToolsSupportApimService employerSupportApiClient) : IRequestHandler<GetPayeSchemeLevyDeclarationsQuery, GetPayeSchemeLevyDeclarationsResult>
{
    public async Task<GetPayeSchemeLevyDeclarationsResult> Handle(GetPayeSchemeLevyDeclarationsQuery query, CancellationToken cancellationToken)
    {
        var payeResponse = await employerSupportApiClient.GetPayeSchemeLevyDeclarations(query.HashedAccountId, query.HashedPayeRef, cancellationToken);

        return (GetPayeSchemeLevyDeclarationsResult)payeResponse;
    }
}
