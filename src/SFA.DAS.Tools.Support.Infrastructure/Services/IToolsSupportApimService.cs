using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IToolsSupportApimService
{
    Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default);
    Task<GetUserOverviewResponse> GetUserOverview(Guid userId, CancellationToken cancellationToken = default);
    Task SendInvitation(SendInvitationRequest data, CancellationToken cancellationToken = default);
    Task ResendInvitation(ResendInvitationRequest data, CancellationToken cancellationToken = default);
    Task ChangeUserRole(ChangeUserRoleRequest data, CancellationToken cancellationToken = default);
}