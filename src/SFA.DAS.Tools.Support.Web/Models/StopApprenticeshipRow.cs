using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipRow
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ULN { get; set; }
        public string EmployerName { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EnteredStopDate { get; set; }
        public DateTime? GetStopDate
        {
            get
            {
                if(DateTime.TryParse(EnteredStopDate, out DateTime parsedDate))
                {
                    return parsedDate;
                }

                return null;
            }
        }
    }
}
