using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.Responses;

public class ChangeUserStatusResponse
{
    public string? Id { get; set; }
    public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
}

