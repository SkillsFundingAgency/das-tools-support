using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetTeamMembers;

public class GetTeamMembersQueryResult
{
    public ICollection<TeamMember> TeamMembers { get; set; }

    public static explicit operator GetTeamMembersQueryResult(GetTeamMembersResponse source)
    {
        if (source == null || source.TeamMembers == null)
        {
            return new GetTeamMembersQueryResult { TeamMembers = [] };
        }
        return new GetTeamMembersQueryResult { TeamMembers = source.TeamMembers };
    }
}
