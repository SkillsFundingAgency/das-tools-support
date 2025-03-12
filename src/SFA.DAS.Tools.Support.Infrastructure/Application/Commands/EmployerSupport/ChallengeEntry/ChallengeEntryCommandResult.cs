using System.Collections.Generic;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChallengeEntry;
public class ChallengeEntryCommandResult
{
    public bool IsValid { get; set; }

    public string Id { get; set; } = "";

    public List<int> Characters { get; set; } = [];

    public static explicit operator ChallengeEntryCommandResult(ChallengeEntryResponse source)
    {
        if (source == null) return new ChallengeEntryCommandResult();

        return new ChallengeEntryCommandResult
        {
            Id = source.Id,
            IsValid = source.IsValid,
            Characters = source.Characters,
        };
    }
}
