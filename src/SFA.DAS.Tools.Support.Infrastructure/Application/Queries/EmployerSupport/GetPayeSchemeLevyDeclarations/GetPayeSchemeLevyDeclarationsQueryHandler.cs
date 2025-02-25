using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;

public class GetPayeSchemeLevyDeclarationsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) 
    : IRequestHandler<GetPayeSchemeLevyDeclarationsQuery, GetPayeSchemeLevyDeclarationsResult>
{
    public async Task<GetPayeSchemeLevyDeclarationsResult> Handle(GetPayeSchemeLevyDeclarationsQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);

        var payeResponse = await employerSupportApiClient.GetPayeSchemeLevyDeclarations(accountId, query.HashedPayeRef, cancellationToken);

        return (GetPayeSchemeLevyDeclarationsResult)payeResponse;
    }
}
