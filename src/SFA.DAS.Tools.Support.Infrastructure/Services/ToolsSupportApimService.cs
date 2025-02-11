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
}
