using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class StopApprenticeshipRow : ApprenticeshipRow
    {
        public DateTime? GetStopDate
        {
            get
            {
                if (DateTime.TryParse(EnteredDate, out DateTime parsedDate))
                {
                    return parsedDate;
                }

                return null;
            }
        }
    }
}
