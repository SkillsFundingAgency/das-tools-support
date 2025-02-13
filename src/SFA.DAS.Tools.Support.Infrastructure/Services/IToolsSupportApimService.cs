using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IToolsSupportApimService
{
    Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default);
    Task<GetUserOverviewResponse> GetUserOverview(Guid userId, CancellationToken cancellationToken = default);
}