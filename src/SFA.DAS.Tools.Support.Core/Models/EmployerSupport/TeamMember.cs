﻿namespace SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

public class TeamMember
{
    public string UserRef { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool CanReceiveNotifications { get; set; }
    public InvitationStatus Status { get; set; }
}
