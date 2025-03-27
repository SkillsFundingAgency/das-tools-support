using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetEmployerAccountsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetEmployerAccountsQuery, GetEmployerAccountsQueryResult>
{
    public async Task<GetEmployerAccountsQueryResult> Handle(GetEmployerAccountsQuery query, CancellationToken cancellationToken)
    {
        var response = new GetEmployerAccountsQueryResult { IsValidAccountCode = true };

        long? accountId = null;
        if (!string.IsNullOrWhiteSpace(query.PublicHashedAccountId))
        {
            if (encodingService.TryDecode(query.PublicHashedAccountId, EncodingType.PublicAccountId, out var decodedValue))
            {
                accountId = decodedValue;
            }
            else
            {
                response.IsValidAccountCode = false;
            }
        }

        if (string.IsNullOrWhiteSpace(query.PublicHashedAccountId) && string.IsNullOrWhiteSpace(query.PayeRef))
        {
            return response;
        }
        
        response.Accounts = (await employerSupportApiClient.GetMatchingAccounts(accountId, query.PayeRef, cancellationToken))
            .Accounts;
        return response;
    }
}
    