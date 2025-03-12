namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
public class ChallengeEntryRequest
{
    public string Id { get; set; } = "";
    public string Challenge1 { get; set; } = "";
    public string Challenge2 { get; set; } = "";
    public string Balance { get; set; } = "";
    public int FirstCharacterPosition { get; set; }
    public int SecondCharacterPosition { get; set; }
}
