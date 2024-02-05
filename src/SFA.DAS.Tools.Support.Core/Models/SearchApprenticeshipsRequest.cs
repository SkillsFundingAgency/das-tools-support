using System;

namespace SFA.DAS.Tools.Support.Core.Models
{
    public class SearchApprenticeshipsRequest
    {
        public string CourseName { get; set; }
        public string EmployerName { get; set; }
        public string ProviderName { get; set; }
        public long? Ukprn { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ApprenticeshipStatus { get; set; }
    }
}