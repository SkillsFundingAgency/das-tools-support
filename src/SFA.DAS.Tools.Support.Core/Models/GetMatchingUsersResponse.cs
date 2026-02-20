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
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? GovUkIdentifier { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsActive { get; set; }
    public bool IsLocked { get; set; }
}