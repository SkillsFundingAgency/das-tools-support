using FluentValidation;
using SFA.DAS.Tools.Support.Core.Models.Challenge;

namespace SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

public class ChallengeEntryValidator : AbstractValidator<ChallengeEntry>
{
    public ChallengeEntryValidator()
    {
        RuleFor(x => x.Challenge1).NotEmpty();
        RuleFor(x => x.Challenge2).NotEmpty();
        RuleFor(x => x.Balance).NotEmpty();
    }
}