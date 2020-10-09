using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class SearchApprenticeshipsViewModel
    {
        public string CourseName { get; set; }
        public string EmployerName { get; set; }
        public string ProviderName { get; set; }
        public string SearchTerm { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        public bool IsModelEmpty => string.IsNullOrWhiteSpace(CourseName) && string.IsNullOrWhiteSpace(EmployerName) && string.IsNullOrWhiteSpace(ProviderName)
                && string.IsNullOrWhiteSpace(SearchTerm) && (StartDate == null || StartDate == DateTime.MinValue) && (EndDate == null || EndDate == DateTime.MinValue);

    }
}
