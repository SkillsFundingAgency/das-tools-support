using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetFinanceDetails;

public class GetFinanceDetailsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService, IPayeRefHashingService hashingService) : IRequestHandler<GetFinanceDetailsQuery, GetFinanceDetailsQueryResult>
{
    public async Task<GetFinanceDetailsQueryResult> Handle(GetFinanceDetailsQuery query, CancellationToken cancellationToken)
    {
        var accountId = encodingService.Decode(query.HashedAccountId, EncodingType.AccountId);
        var financeResponse = await employerSupportApiClient.GetFinanceData(accountId, cancellationToken);
        if (financeResponse == null)
        {
            return new GetFinanceDetailsQueryResult();
        }

        foreach (var scheme in financeResponse.PayeSchemes)
        {
            scheme.HashedPayeRef = hashingService.HashValue(scheme.Ref);
        }

        return (GetFinanceDetailsQueryResult)financeResponse;
    }
}