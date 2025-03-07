using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models;
public class ChallengeEntryResponse
{
    public bool IsValid { get; set; }

    public string Id { get; set; } = "";

    public List<int> Characters { get; set; } = [];
}
