using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerUsersService
    {
        Task<SuspendUserResult> SuspendUser(SuspendUserRequest request, CancellationToken token);
        Task<ResumeUserResult> ResumeUser(ResumeUserRequest request, CancellationToken token);
    }
}