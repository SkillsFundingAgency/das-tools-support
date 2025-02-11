using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

public class ToolsSupportOuterApiClient(IOuterApiClient client) : IToolsSupportOuterApiClient
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountDetailsResponse>($"employeraccount/{accountId}/account-details?accountFieldSelection={accountFieldSelection}");
    }
}
