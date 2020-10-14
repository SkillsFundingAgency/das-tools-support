using SFA.DAS.CommitmentsV2.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipViewModel
    {
        [Required(ErrorMessage = "Please enter a valid Stop Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EnteredStopDate { get; set; }

        public long ApprenticeshipId { get; set; }
        public long EmployerAccountId { get; set; }
        public bool SubmittedSuccessfully { get; set; }
        public bool ApprenticeshipNotFound { get; set; }
        public DateTime StopDate { get; set; }
        public string ApprenticeshipName => string.Concat(FirstName, " ", LastName);
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public string EndpointAssessorName { get; set; }
        public string ULN { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? PauseDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
