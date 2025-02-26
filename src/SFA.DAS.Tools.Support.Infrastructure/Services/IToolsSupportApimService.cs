using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IToolsSupportApimService
{
    Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, CancellationToken cancellationToken = default);
    Task<GetAccountOrganisationsResponse> GetAccountOrganisations(long accountId, CancellationToken cancellationToken = default);
    Task<GetTeamMembersResponse> GetTeamMembers(long accountId, CancellationToken cancellationToken = default);
    Task<GetFinanceDataResponse> GetFinanceData(long accountId, CancellationToken cancellationToken = default);
    Task<GetPayeSchemeLevyDeclarationsResponse> GetPayeSchemeLevyDeclarations(long accountId, string hashedPayeRef, CancellationToken cancellationToken = default);
    Task<GetChallengePermissionResponse> GetChallengePermission(long accountId, CancellationToken cancellationToken = default);
    Task<GetUserOverviewResponse> GetUserOverview(Guid userId, CancellationToken cancellationToken = default);
    Task<ChallengeEntryResponse> ChallengeEntry(ChallengeEntryRequest data, long accountId, CancellationToken cancellationToken = default);
    Task SendInvitation(SendInvitationRequest data, CancellationToken cancellationToken = default);
    Task ResendInvitation(ResendInvitationRequest data, CancellationToken cancellationToken = default);
    Task ChangeUserRole(ChangeUserRoleRequest data, CancellationToken cancellationToken = default);
}