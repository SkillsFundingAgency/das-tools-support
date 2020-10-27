using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Tools.Support.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipViewModel
    {
        public bool HasError { get; set; }
        public IEnumerable<StopApprenticeshipRow> Apprenticeships { get; set; }
        public string ApprenticeshipsData { get; set; }

        public SearchParameters SearchParams { get; set; }

        public string GetApprenticesTableData() => JsonSerializer.Serialize(Apprenticeships, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        public class SearchParameters
        {
            public string CourseName { get; set; }
            public string EmployerName { get; set; }
            public string ProviderName { get; set; }
            public string ApprenticeName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string SelectedStatus { get; set; }
        }

        public bool ApprenticesStoppedCompleted => Apprenticeships != null && Apprenticeships.All(a => a.ApiSubmissionStatus == StopApprenticeshipRow.SubmissionStatus.Successful);
    }
}
