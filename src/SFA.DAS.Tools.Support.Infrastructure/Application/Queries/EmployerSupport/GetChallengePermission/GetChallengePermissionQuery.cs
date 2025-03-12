using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetChallengePermission;

public class GetChallengePermissionQuery : IRequest<GetChallengePermissionQueryResult>
{
    public string HashedAccountId { get; set; } = "";
}
