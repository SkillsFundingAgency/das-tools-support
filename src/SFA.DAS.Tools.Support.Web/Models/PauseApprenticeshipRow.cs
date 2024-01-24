using System;

namespace SFA.DAS.Tools.Support.Web.Models;

public class PauseApprenticeshipRow : ApprenticeshipRow
{
    public DateTime? GetPauseDate
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