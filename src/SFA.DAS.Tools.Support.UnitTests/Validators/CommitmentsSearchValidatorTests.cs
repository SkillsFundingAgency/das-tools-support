using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Learners.Validators;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class CommitmentsSearchValidatorTests
{
    [Test, MoqAutoData]
    public void When_No_SearchTerm_For_Uln_Search_Should_Have_Validation_Error(
        CommitmentSearchModelValidator validator,
        CommitmentSearchViewModel model
        )
    {
        // Arrange     
        model.SearchType = ApprenticeshipSearchType.SearchByUln;
        model.SearchTerm = string.Empty;

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("Uln must not be empty");
    }

    [Test, MoqAutoData]
    public void When_No_SearchTerm_For_Cohort_Search_Should_Have_Validation_Error(
        CommitmentSearchModelValidator validator,
        CommitmentSearchViewModel model
    )
    {
        // Arrange     
        model.SearchType = ApprenticeshipSearchType.SearchByCohort;
        model.SearchTerm = string.Empty;

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("Cohort must not be empty");
    }

    [Test, MoqAutoData]
    public void When_Uln_Search_But_It_Is_Invalid_Should_Have_Validation_Error(
        [Frozen] Mock<IUlnValidator> mockUlnValidator,
        [Greedy] CommitmentSearchModelValidator validator,
        CommitmentSearchViewModel model
    )
    {
        // Arrange     
        model.SearchType = ApprenticeshipSearchType.SearchByUln;

        mockUlnValidator.Setup(x => x.Validate(model.SearchTerm)).Returns(UlnValidationResult.IsInValidTenDigitUlnNumber);

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("This is not a valid ULN");
    }

    [Test, MoqAutoData]
    public void When_Uln_Search_And_It_Is_Valid_Should_Return_Valid(
        [Frozen] Mock<IUlnValidator> mockUlnValidator,
        [Greedy] CommitmentSearchModelValidator validator,
        CommitmentSearchViewModel model
    )
    {
        // Arrange     
        model.SearchType = ApprenticeshipSearchType.SearchByUln;

        mockUlnValidator.Setup(x => x.Validate(model.SearchTerm)).Returns(UlnValidationResult.Success);

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test, MoqAutoData]
    public void When_Cohort_Search_But_It_Is_Invalid_Should_Have_Validation_Error(
        [Frozen] Mock<IEncodingService> mockEncodingService,
        [Greedy] CommitmentSearchModelValidator validator,
        CommitmentSearchViewModel model,
        long value
    )
    {
        // Arrange     
        model.SearchType = ApprenticeshipSearchType.SearchByCohort;

        mockEncodingService.Setup(x => x.TryDecode(model.SearchTerm, EncodingType.CohortReference, out value)).Returns(false);

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchTerm)
            .WithErrorMessage("This is not a valid cohort reference");
    }

    [Test, MoqAutoData]
    public void When_Cohort_Search_But_And_It_Is_Valid_Should_Return_Valid(
        [Frozen] Mock<IEncodingService> mockEncodingService,
        [Greedy] CommitmentSearchModelValidator validator,
        CommitmentSearchViewModel model,
        long value
    )
    {
        // Arrange     
        model.SearchType = ApprenticeshipSearchType.SearchByCohort;

        mockEncodingService.Setup(x => x.TryDecode(model.SearchTerm, EncodingType.CohortReference, out value)).Returns(true);

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
