using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
 public interface IEmployerUsersService
    {
        Task<SuspendUsersResult> SuspendUsers(Core.Models.SuspendUsersRequest request, CancellationToken token);
    }
}