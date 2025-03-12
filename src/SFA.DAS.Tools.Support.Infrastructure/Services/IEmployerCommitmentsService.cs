using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public interface IEmployerCommitmentsService
{
    Task<StopApprenticeshipResult> StopApprenticeship(StopApprenticeshipRequest request, CancellationToken token);
    Task<PauseApprenticeshipResult> PauseApprenticeship(PauseApprenticeshipRequest request, CancellationToken token);
    Task<ResumeApprenticeshipResult> ResumeApprenticeship(ResumeApprenticeshipRequest request, CancellationToken token);
    Task<SearchApprenticeshipsResult> SearchApprenticeships(SearchApprenticeshipsRequest request, CancellationToken token);
    Task<GetApprenticeshipResult> GetApprenticeship(long apprenticeshipId, CancellationToken token);
}