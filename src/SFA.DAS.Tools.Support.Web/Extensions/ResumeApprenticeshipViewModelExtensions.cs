
using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Extensions
{
    public static class ResumeApprenticeshipViewModelExtensions
    {
        public static bool TryDeserialise(this ResumeApprenticeshipViewModel model, out List<ResumeApprenticeshipRow> result, ILogger logger = null)
        {
            result = null;

            try
            {
                result = JsonSerializer.Deserialize<List<ResumeApprenticeshipRow>>(model.ApprenticeshipsData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                return true;
            }

            catch (Exception e)
            {
                logger?.LogError("Unable to deserialize apprenticeship data", e);
                
                return false;
            }                
        }
    }
}
