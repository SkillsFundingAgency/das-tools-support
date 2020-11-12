using System;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class ApprenticeshipSearchResultsViewModel
    {
        public string CourseName { get; set; }
        public string EmployerName { get; set; }
        public string ProviderName { get; set; }
        public long? Ukprn { get; set; }
        public string ApprenticeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public string SelectedIds { get; set; }
    }
}
