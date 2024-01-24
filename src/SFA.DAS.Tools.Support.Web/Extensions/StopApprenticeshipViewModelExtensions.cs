using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class StopApprenticeshipViewModelExtensions
{
    public static bool TryDeserialise(this StopApprenticeshipViewModel model, out List<StopApprenticeshipRow> result, ILogger logger = null)
    {
        result = null;

        try
        {
            result = JsonSerializer.Deserialize<List<StopApprenticeshipRow>>(model.ApprenticeshipsData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return true;
        }

        catch (Exception e)
        {
            logger?.LogError("Unable to deserialize apprenticeship data", e);
                
            return false;
        }                
    }
}