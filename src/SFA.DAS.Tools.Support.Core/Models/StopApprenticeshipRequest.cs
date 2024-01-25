using System;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class StopApprenticeshipRequest
    {
        public long ApprenticeshipId { get; set; }
        public long AccountId { get; set; }
        public DateTime? CurrentStopDate { get; set; }
        public DateTime RequestedStopDate { get; set; }
        public bool MadeRedundant { get; set; }
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }

        public void Validate()
        {
            if (AccountId <= 0)
            {
                throw new Exception("employerAccountId must be greater than 0");
            }

            if (ApprenticeshipId <= 0)
            {
                throw new Exception("apprenticeshipId must be greater than 0");
            }
        }
    }
}