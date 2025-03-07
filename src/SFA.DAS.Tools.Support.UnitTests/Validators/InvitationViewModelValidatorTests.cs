using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class InvitationViewModelValidatorTests
{
    private Fixture _fixture;
    private InvitationViewModelValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _validator = new InvitationViewModelValidator();
    }

    [Test]
    public void When_All_Properties_Are_Populated_Should_Be_Valid()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, "B")
            .With(x => x.FullName, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void When_Email_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, (string)null)
            .With(x => x.HashedAccountId, "B")
            .With(x => x.FullName, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test]
    public void When_Email_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, "")
            .With(x => x.HashedAccountId, "B")
            .With(x => x.FullName, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test]
    public void When_HashedAccountId_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, (string)null)
            .With(x => x.FullName, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }

    [Test]
    public void When_HashedAccountId_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, "")
            .With(x => x.FullName, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }

    [Test]
    public void When_FullName_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, "B")
            .With(x => x.FullName, (string)null)
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("'Full Name' must not be empty.");
    }

    [Test]
    public void When_FullName_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var invitation = _fixture.Build<InvitationViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, "B")
            .With(x => x.FullName, "")
            .Create();

        // Act
        var result = _validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("'Full Name' must not be empty.");
    }
}
