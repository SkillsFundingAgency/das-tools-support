using SFA.DAS.Tools.Support.Core.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IToolsSupportApimService
{
    Task<GetAccountDetailsResponse> GetAccountDetails(long accountId, string accountFieldSelection, CancellationToken cancellationToken = default);
    Task<GetMatchUsersResponse> GetMatchingUsers(string email, CancellationToken cancellationToken = default);
    Task<GetMatchingEmployerAccountsResponse> GetMatchingAccounts(long? accountId, string payeRef, CancellationToken cancellationToken = default);
}