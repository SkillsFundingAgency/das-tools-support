using SFA.DAS.Tools.Support.Core.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IToolsSupportApimService
{
    Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default);
    Task<GetMatchingUsersResponse> GetMatchingUsers(string email, CancellationToken cancellationToken = default);
    Task<GetMatchingApprenticeshipsResponse> GetMatchingUlns(string uln, CancellationToken cancellationToken = default);
    Task<GetCohortResponse> GetCohort(long id, CancellationToken cancellationToken = default);
    Task<GetApprenticeshipDetailsResponse> GetApprenticeshipDetails(long id, CancellationToken cancellationToken = default);
    Task<GetMatchingEmployerAccountsResponse> GetMatchingAccounts(long? accountId, string payeRef, CancellationToken cancellationToken = default);
}