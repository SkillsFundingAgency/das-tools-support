using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class InvitationViewModelValidatorTests
{
    [Test, MoqAutoData]
    public void When_All_Properties_Are_Populated_Should_Be_Valid(
       InvitationViewModelValidator validator,
       InvitationViewModel invitation
       )
    {
        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test, MoqAutoData]
    public void When_Email_Is_Null_Should_Have_Validation_Error(
        InvitationViewModelValidator validator,
        InvitationViewModel invitation)
    {
        // Arrange
        invitation.Email = null;
        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Email_Is_Empty_Should_Have_Validation_Error(
                InvitationViewModelValidator validator,
        InvitationViewModel invitation)
    {
        // Arrange
        invitation.Email = "";

        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_HashedAccountId_Is_Null_Should_Have_Validation_Error(
                InvitationViewModelValidator validator,
        InvitationViewModel invitation)
    {
        // Arrange
        invitation.HashedAccountId = null;

        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_HashedAccountId_Is_Empty_Should_Have_Validation_Error(
                InvitationViewModelValidator validator,
        InvitationViewModel invitation)
    {
        // Arrange
        invitation.HashedAccountId = "";


        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_FullName_Is_Null_Should_Have_Validation_Error(
                InvitationViewModelValidator validator,
        InvitationViewModel invitation)
    {
        // Arrange
        invitation.FullName = null;

        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("'Full Name' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_FullName_Is_Empty_Should_Have_Validation_Error(
                InvitationViewModelValidator validator,
        InvitationViewModel invitation)
    {
        // Arrange
        invitation.FullName = "";

        // Act
        var result = validator.TestValidate(invitation);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("'Full Name' must not be empty.");
    }
}
