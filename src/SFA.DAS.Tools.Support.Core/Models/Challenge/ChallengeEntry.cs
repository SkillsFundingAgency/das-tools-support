using System.Collections.Generic;

namespace SFA.DAS.Tools.Support.Core.Models.Challenge;

public class ChallengeEntry
{
    public string Id { get; set; }

    public string Challenge1 { get; set; }

    public string Challenge2 { get; set; }

    public string Balance { get; set; }

    public int FirstCharacterPosition { get; set; }

    public int SecondCharacterPosition { get; set; }
}
