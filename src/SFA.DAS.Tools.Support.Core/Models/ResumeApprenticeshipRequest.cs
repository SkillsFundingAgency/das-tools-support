using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class ResumeApprenticeshipRequest
    {
        public long ApprenticeshipId { get; set; }
        // public DateTime PauseDate { get; set; }
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }

        public void Validate()
        {
            if (ApprenticeshipId <= 0)
            {
                throw new ArgumentException("apprenticeshipId must be greater than 0", "apprenticeshipId");
            }
        }
    }
}
