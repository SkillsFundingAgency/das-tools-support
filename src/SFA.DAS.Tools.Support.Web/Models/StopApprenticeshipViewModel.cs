using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Tools.Support.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipViewModel : ApprenticeshipViewModel
    {
        public IEnumerable<StopApprenticeshipRow> Apprenticeships { get; set; }

        public bool ApprenticesStoppedCompleted => Apprenticeships != null && Apprenticeships.All(a => a.ApiSubmissionStatus == SubmissionStatus.Successful);

        public string GetApprenticesTableData() => JsonSerializer.Serialize(Apprenticeships, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
