using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetTeamMembersResponse
{
    public ICollection<TeamMember> TeamMembers { get; set; }

}
