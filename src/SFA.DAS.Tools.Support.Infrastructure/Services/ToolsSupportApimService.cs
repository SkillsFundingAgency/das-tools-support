using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class ToolsSupportApimService(IOuterApiClient client) : IToolsSupportApimService
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountDetailsResponse>($"employeraccount/{accountId}/account-details?accountFieldSelection={accountFieldSelection}");
    }

    public Task<GetMatchingUsersResponse> GetMatchingUsers(string email, CancellationToken cancellationToken = default)
    {
        return client.Get<GetMatchingUsersResponse>($"users/query?email={WebUtility.UrlEncode(email)}");
    }

    public Task<GetMatchingApprenticeshipsResponse> GetMatchingUlns(string uln, CancellationToken cancellationToken = default)
    {
        return client.Get<GetMatchingApprenticeshipsResponse>($"apprenticeships/uln/{uln}");
    }

    public Task<GetMatchingEmployerAccountsResponse> GetMatchingAccounts(long? accountId, string payeRef, CancellationToken cancellationToken = default)
    {
        return client.Get<GetMatchingEmployerAccountsResponse>($"accounts?accountId={accountId}&payeSchemeRef={WebUtility.UrlEncode(payeRef)}");
    }
}
