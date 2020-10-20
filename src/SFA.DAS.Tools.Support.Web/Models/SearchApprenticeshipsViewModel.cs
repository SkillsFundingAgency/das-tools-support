using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.CommitmentsV2.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class SearchApprenticeshipsViewModel
    {
        public SearchApprenticeshipsViewModel()
        {
            Statuses = new List<SelectListItem>()
            {
                new SelectListItem("Any", ""),
                new SelectListItem("Waiting to Start", "0"),
                new SelectListItem("Live", "1"),
                new SelectListItem("Paused", "2")
            };
            SelectedStatus = "0";
        }

        public string CourseName { get; set; }
        public string EmployerName { get; set; }
        public string ProviderName { get; set; }
        public string ApprenticeName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; }

        public string SelectedStatus { get; set; }

        public string SelectedIds { get; set; }

        public bool IsModelEmpty => string.IsNullOrWhiteSpace(CourseName) && string.IsNullOrWhiteSpace(EmployerName) && string.IsNullOrWhiteSpace(ProviderName)
                && string.IsNullOrWhiteSpace(ApprenticeName) && (StartDate == null || StartDate == DateTime.MinValue) && (EndDate == null || EndDate == DateTime.MinValue);

    }
}
