using System;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class ResumeApprenticeshipRequest
    {
        public long ApprenticeshipId { get; set; }
        public string UserId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }

        public void Validate()
        {
            if (ApprenticeshipId <= 0)
            {
                throw new ValidationException("apprenticeshipId must be greater than 0");
            }
        }
    }
}