using System;

namespace SFA.DAS.Tools.Support.Web.Models
{
    public class ResumeApprenticeshipRow : ApprenticeshipRow
    {
       public DateTime? GetResumedDate
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
