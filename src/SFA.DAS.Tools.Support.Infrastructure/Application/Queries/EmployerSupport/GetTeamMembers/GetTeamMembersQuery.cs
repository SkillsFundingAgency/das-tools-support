using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetTeamMembers;

public class GetTeamMembersQuery : IRequest<GetTeamMembersQueryResult>
{
    public string HashedAccountId { get; set; } = "";
}