using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IEmployerSupportService
{
    Task<GetAccountDetailsResponse> GetAccountDetails(string accountHashedId, string accountFieldSelection, CancellationToken cancellationToken = default);
}