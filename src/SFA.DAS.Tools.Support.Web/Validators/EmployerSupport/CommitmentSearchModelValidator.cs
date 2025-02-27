using FluentValidation;
using SFA.DAS.Encoding;
using SFA.DAS.Learners.Validators;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

public class CommitmentSearchModelValidator : AbstractValidator<CommitmentSearchViewModel>
{
    public CommitmentSearchModelValidator(IUlnValidator ulnValidator, IEncodingService encodingService)
    {
        RuleFor(x => x.SearchTerm).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Must not be empty");
        RuleFor(x => x.SearchTerm).Cascade(CascadeMode.Stop).Must((m, x) =>
        {
            if (m.SearchType == ApprenticeshipSearchType.SearchByUln)
            {
                var result = ulnValidator.Validate(x);
                return result == UlnValidationResult.Success;
            }
            return true;
        }).WithMessage("This is not a valid ULN");

        RuleFor(x => x.SearchTerm).Cascade(CascadeMode.Stop).Must((m, x) =>
        {
            if (m.SearchType == ApprenticeshipSearchType.SearchByCohort)
            {
                return encodingService.TryDecode(m.SearchTerm, EncodingType.PublicAccountId, out var _);
            }
            return true;
        }).WithMessage("This is not a valid cohort reference");
    }
}