using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipViewModel
    {
        [Required(ErrorMessage ="Please enter a valid ApprenticeshipId")]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ApprenticeshipId { get; set; }

        [Required(ErrorMessage = "Please enter a valid EmployerAccountId")]
        [Range(1,long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long EmployerAccountId { get; set; }

        [Required(ErrorMessage = "Please enter a valid Stop Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StopDate { get; set; }

        public bool SubmittedSuccessfully { get; set; }
    }
}
