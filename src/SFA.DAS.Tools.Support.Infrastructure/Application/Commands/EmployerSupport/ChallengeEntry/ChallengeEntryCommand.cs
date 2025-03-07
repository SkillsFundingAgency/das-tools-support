using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChallengeEntry;
public class ChallengeEntryCommand : IRequest<ChallengeEntryCommandResult>
{
    public string Id { get; set; } = "";
    public string Challenge1 { get; set; } = "";
    public string Challenge2 { get; set; } = "";
    public string Balance { get; set; } = "";
    public int FirstCharacterPosition { get; set; }
    public int SecondCharacterPosition { get; set; }
}
