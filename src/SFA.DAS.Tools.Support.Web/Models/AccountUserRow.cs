using System;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class AccountUserRow
    { 
        public string AccountId { get; set; }
        public string UserRef { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string AccountStatus { get; set; }
        public DateTime? LastSuspendedDate { get; set; }
        public SubmissionStatus ApiSubmissionStatus { get; set; }
        public string ApiErrorMessage { get; set; }
    }
}