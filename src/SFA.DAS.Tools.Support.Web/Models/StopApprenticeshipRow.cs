using System;

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
