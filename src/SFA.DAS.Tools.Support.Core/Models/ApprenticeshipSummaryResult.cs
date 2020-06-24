using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public enum PaymentStatus : short
    {
        PendingApproval = 0,
        Active = 1,
        Paused = 2,
        Withdrawn = 3,
        Completed = 4,
        Deleted = 5
    }

    public enum TrainingType
    {
        Standard = 0,
        Framework = 1
    }

    public class ApprenticeshipSummaryResult : ResultBase
    {
        public string EmployerRef { get; set; }
        public string ProviderRef { get; set; }
        public string ProviderName { get; set; }
        public string ApprenticeshipName { get; }
        public string EndpointAssessorName { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Reference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ULN { get; set; }
        public TrainingType TrainingType { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PauseDate { get; set; }
        public string NINumber { get; set; }
        public long? ContinuationOfId { get; set; }
    }
}
