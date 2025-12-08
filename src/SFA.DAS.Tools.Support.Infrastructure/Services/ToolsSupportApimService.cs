using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class ToolsSupportApimService(IOuterApiClient client) : IToolsSupportApimService
{
    public async Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountDetailsResponse>($"employeraccounts/{accountId}/account-details");
    }

    public async Task<GetAccountOrganisationsResponse> GetAccountOrganisations(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetAccountOrganisationsResponse>($"employeraccounts/{accountId}/organisations");
    }

    public async Task<GetTeamMembersResponse> GetTeamMembers(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetTeamMembersResponse>($"employeraccounts/{accountId}/team-members");
    }

    public async Task<GetFinanceDataResponse> GetFinanceData(long accountId, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetFinanceDataResponse>($"employeraccounts/{accountId}/finance");
    }

    public async Task<GetPayeSchemeLevyDeclarationsResponse> GetPayeSchemeLevyDeclarations(long accountId, string payeRef, CancellationToken cancellationToken = default)
    {
        return await client.Get<GetPayeSchemeLevyDeclarationsResponse>($"employeraccounts/{accountId}/paye-levy-declarations?payeRef={payeRef}");
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
        var url = "employeraccounts/send-invitation";
        await client.Post<SendInvitationRequest, object>(url, data);
    }

    public async Task ResendInvitation(ResendInvitationRequest data, CancellationToken cancellationToken = default)
    {
        var url = "employeraccounts/resend-invitation";
        await client.Post<ResendInvitationRequest, object>(url, data);
    }

    public async Task ChangeUserRole(ChangeUserRoleRequest data, CancellationToken cancellationToken = default)
    {
        var url = "employeraccounts/change-role";
        await client.Post<ChangeUserRoleRequest, object>(url, data);
    }

    public Task<GetMatchingUsersResponse> GetMatchingUsers(string email, CancellationToken cancellationToken = default)
    {
        return client.Get<GetMatchingUsersResponse>($"users/query?email={WebUtility.UrlEncode(email)}");
    }

    public Task<GetMatchingApprenticeshipsResponse> GetMatchingUlns(string uln, CancellationToken cancellationToken = default)
    {
        return client.Get<GetMatchingApprenticeshipsResponse>($"apprenticeships/uln/{uln}");
    }

    public Task<GetCohortResponse> GetCohort(long id, CancellationToken cancellationToken = default)
    {
        return client.Get<GetCohortResponse>($"cohorts/{id}");
    }

    public Task<GetApprenticeshipDetailsResponse> GetApprenticeshipDetails(long id, CancellationToken cancellationToken = default)
    {
        return client.Get<GetApprenticeshipDetailsResponse>($"apprenticeships/{id}");
    }

    public Task<GetMatchingEmployerAccountsResponse> GetMatchingAccounts(long? accountId, string payeRef, string employerName = null, CancellationToken cancellationToken = default)
    {
        var url = $"employeraccounts?accountId={accountId}&payeSchemeRef={WebUtility.UrlEncode(payeRef)}";
        if (!string.IsNullOrWhiteSpace(employerName))
        {
            url += $"&employerName={WebUtility.UrlEncode(employerName)}";
        }
        return client.Get<GetMatchingEmployerAccountsResponse>(url);
    }

    public Task<ChangeUserStatusResponse> SuspendEmployerUser(string identifier, ChangeUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        return client.Post<ChangeUserStatusRequest, ChangeUserStatusResponse>($"users/{identifier}/suspend", request);
    }

    public Task<ChangeUserStatusResponse> ResumeEmployerUser(string identifier, ChangeUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        return client.Post<ChangeUserStatusRequest, ChangeUserStatusResponse>($"users/{identifier}/resume", request);
    }
}
