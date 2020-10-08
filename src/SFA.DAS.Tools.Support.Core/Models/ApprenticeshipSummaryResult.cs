using System;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class ApprenticeshipSummaryResult : ResultBase
    {
        public string ProviderName { get; set; }
        public string EmployerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Uln { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? PauseDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string EndpointAssessorName { get; set; }
    }
}
