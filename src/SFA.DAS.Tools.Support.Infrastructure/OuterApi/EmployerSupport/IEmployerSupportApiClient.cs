using SFA.DAS.Tools.Support.Core.Models;
using System.Threading.Tasks;
using System.Threading;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

public interface IEmployerSupportApiClient
{
    Task<GetAccountDetailsResponse> GetAccountDetails(string accountHashedId, string accountFieldSelection, CancellationToken cancellationToken = default);
}