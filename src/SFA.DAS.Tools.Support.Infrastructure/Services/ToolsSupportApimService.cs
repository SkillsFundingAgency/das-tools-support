using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class ToolsSupportApimService(IOuterApiClient client) : IToolsSupportApimService
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountDetailsResponse>($"employeraccount/{accountId}/account-details?accountFieldSelection={accountFieldSelection}");
    }
    
    public async Task<GetPayeSchemeLevyDeclarationsResponse> GetPayeSchemeLevyDeclarations(string hashedAccountId, string hashedPayeRef, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetPayeSchemeLevyDeclarationsResponse>($"employeraccount/{hashedAccountId}/paye-levy-declarations?hashedPayeRef={hashedPayeRef}");
    }

    public async Task<GetUserOverviewResponse> GetUserOverview(Guid userId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetUserOverviewResponse>($"users/query/user-overview?userId={userId}");
    }

    public async Task SendInvitation(SendInvitationRequest data, CancellationToken cancellationToken = default)
    {
        var url = "employeraccount/send-invitation";
        await client.Post<SendInvitationRequest, object>(url, data);
    }

    public async Task ResendInvitation(ResendInvitationRequest data, CancellationToken cancellationToken = default)
    {
        var url = "employeraccount/resend-invitation";
        await client.Post<ResendInvitationRequest, object>(url, data);
    }
    
    public async Task ChangeUserRole(ChangeUserRoleRequest data, CancellationToken cancellationToken = default)
    {
        var url = "employeraccount/change-role";
        await client.Post<ChangeUserRoleRequest, object>(url, data);
    }
}
