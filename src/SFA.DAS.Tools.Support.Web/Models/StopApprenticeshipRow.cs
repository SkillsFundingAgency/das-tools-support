using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipRow
    {
        public enum SubmissionStatus
        {
            NotSent = 0,
            Successful = 1,
            Errored = 2
        }

        public long Id { get; set; }
        public long AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ULN { get; set; }
        public string EmployerName { get; set; }
        public string ProviderName { get; set; }
        public long Ukprn { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EnteredStopDate { get; set; }
        public SubmissionStatus ApiSubmissionStatus { get; set; }
        public string ApiErrorMessage { get; set; }
        public DateTime? GetStopDate
        {
            get
            {
                if (DateTime.TryParse(EnteredStopDate, out DateTime parsedDate))
                {
                    return parsedDate;
                }

                return null;
            }
        }
    }
}
