using FluentValidation;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

public class ChangeUserRoleViewModelValidator : AbstractValidator<ChangeUserRoleViewModel>
{
    public ChangeUserRoleViewModelValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.HashedAccountId).NotEmpty();
        RuleFor(x => x.Role).NotNull();
    }
}
