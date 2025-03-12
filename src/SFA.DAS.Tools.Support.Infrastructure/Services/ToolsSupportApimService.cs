using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class ToolsSupportApimService(IOuterApiClient client) : IToolsSupportApimService
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountDetailsResponse>($"employeraccount/{accountId}/account-details");
    }

    public async Task<GetAccountOrganisationsResponse> GetAccountOrganisations(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountOrganisationsResponse>($"employeraccount/{accountId}/organisations");
    }

    public async Task<GetTeamMembersResponse> GetTeamMembers(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetTeamMembersResponse>($"employeraccount/{accountId}/team-members");
    }

    public async Task<GetFinanceDataResponse> GetFinanceData(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetFinanceDataResponse>($"employeraccount/{accountId}/finance");
    }

    public async Task<GetPayeSchemeLevyDeclarationsResponse> GetPayeSchemeLevyDeclarations(long accountId, string payeRef, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetPayeSchemeLevyDeclarationsResponse>($"employeraccount/{accountId}/paye-levy-declarations?payeRef={payeRef}");
    }

    public async Task<GetChallengePermissionResponse> GetChallengePermission(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetChallengePermissionResponse>($"challenge/{accountId}");
    }

    public async Task<ChallengeEntryResponse> ChallengeEntry(ChallengeEntryRequest data, long accountId, CancellationToken cancellationToken = default)
    {
        var url = $"challenge/{accountId}";
        return await client.Post<ChallengeEntryRequest, ChallengeEntryResponse>(url, data);
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
