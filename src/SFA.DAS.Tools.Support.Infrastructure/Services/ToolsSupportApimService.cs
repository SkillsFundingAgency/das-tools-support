using System;
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
    
    public async Task<GetUserSummaryResponse> GetUserSummary(Guid userId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetUserSummaryResponse>($"employeraccount/{userId}/user-summary");

        //return new GetUserSummaryResponse
        //{
        //    Id = "hashedId here",
        //    FirstName = "Fred",
        //    LastName = "Smith",
        //    Email = "ghghgh@ghghg.com",
        //    IsActive = true,
        //    IsLocked = false,
        //    IsSuspended = false,
        //    AccountSummaries =
        //    [
        //        new AccountSummary
        //        {
        //            DasAccountName = "Company NAME",
        //            HashedAccountId = "HASHEDACCOUNTID"
        //        }
        //    ]
        //};
    }
}
