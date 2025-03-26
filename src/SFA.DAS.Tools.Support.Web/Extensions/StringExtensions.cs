namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class StringExtensions
{
    public static string GetPaymentStatusDisplayClass(this string paymentStatus)
    {
        switch (paymentStatus)
        {
            case "Stopped": return "govuk-tag--red";
            case "WaitingToStart": return "govuk-tag--yellow";
            case "Paused": return "govuk-tag--grey";
            case "Live": return "govuk-tag--blue";
            case "Completed": return "govuk-tag--green";
            default: return string.Empty;
        }
    }
}
