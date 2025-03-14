using System;
using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;

public class GetUsersByEmailQueryResult
{
    public List<UserProfile> Users { get; set; } = new ();
}