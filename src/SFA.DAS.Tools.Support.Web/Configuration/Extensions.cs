using System;

namespace SFA.DAS.Tools.Support.Web.Configuration;

public static class Extensions
{
    public static string GetUIFormattedDate(this DateTime? date)
    {
        if (!date.HasValue || date.Value == DateTime.MinValue)
        {
            return null;
        }

        return date.Value.ToString("yyyy-MM-dd");
    }
}