using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using System;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipConfirmationViewModel
    {
        public long ApprenticeshipId { get; set; }

        public long EmployerAccountId { get; set; }

        public DateTime EnteredStopDate { get; set; }

        public bool SubmittedSuccessfully { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ProviderName { get; set; }
        public string ApprenticeshipName => string.Concat(FirstName, " ", LastName);
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EndpointAssessorName { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ULN { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PauseDate { get; set; }
    }
}
