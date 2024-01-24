using System;
using System.ComponentModel;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public enum InvitationStatus : byte
    {
        [Description("Invitation awaiting response")]
        Pending = 1,
        [Description("Active")]
        Accepted = 2,
        [Description("Invitation expired")]
        Expired = 3,
        [Description("Invitation cancelled")]
        Deleted = 4
    }
    
    /// <summary>
    /// Used as a DTO for the Commitments Api,
    /// Depending on the Api Call and the Mapper, not all of the below properties may be populated
    /// Check each api response object to confirm
    /// </summary>
    public class AccountUserDto
    {
        public string UserRef { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public bool CanReceiveNotifications { get; set; }

        public string AccountStatus { get; set; }

        public InvitationStatus Status { get; set; }
        public DateTime? LastSuspendedDate { get; set; }
    }
}