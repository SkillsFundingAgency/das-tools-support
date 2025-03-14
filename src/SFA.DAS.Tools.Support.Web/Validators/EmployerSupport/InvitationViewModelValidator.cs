using FluentValidation;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

public class InvitationViewModelValidator : AbstractValidator<InvitationViewModel>
{
    public InvitationViewModelValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.HashedAccountId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty();
    }
}
