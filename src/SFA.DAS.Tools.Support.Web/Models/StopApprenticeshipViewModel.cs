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
        private IEnumerable<ApprenticeshipDto> enumerable;

        public bool HasError { get; set; }
        public IEnumerable<ApprenticeshipDto> Apprenticeships { get; set; }
        public string ApprenticeshipsData { get; set; }

        public SearchParameters SearchParams { get; set; }

        public string GetApprenticesTableData()
        {
            var tableModel = Apprenticeships.Select(a => new
            {
                Id = a.Id,
                AccountId = a.EmployerAccountId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                ULN = a?.Uln,
                EmployerName = a.EmployerName,
                ProviderName = a.ProviderName,
                StartDate = a.StartDate.ToShortDateString(),
                EndDate = a.EndDate.ToShortDateString(),
                Status = a.ApprenticeshipStatus.ToString(),
                EnteredStopDate = ""
            });

            return JsonSerializer.Serialize(tableModel, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public class SearchParameters
        {
            public string CourseName { get; set; }
            public string EmployerName { get; set; }
            public string ProviderName { get; set; }
            public string ApprenticeName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string SelectedStatus { get; set; }

            public string GetFormattedStartDate
            {
                get
                {
                    if (StartDate == null || StartDate == DateTime.MinValue)
                    {
                        return null;
                    }

                    return StartDate.Value.ToString("yyyy-MM-dd");
                }
            }

            public string GetFormattedEndDate
            {
                get
                {
                    if (EndDate == null || EndDate == DateTime.MinValue)
                    {
                        return null;
                    }

                    return EndDate.Value.ToString("yyyy-MM-dd");
                }
            }

        }
    }
}
