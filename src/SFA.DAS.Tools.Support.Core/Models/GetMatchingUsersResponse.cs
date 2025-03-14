using System;
using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models;

public class GetMatchingUsersResponse
{
    public List<UserProfile> Users { get; set; } = new();
}

public class UserProfile
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string? Email { get; set; }
}