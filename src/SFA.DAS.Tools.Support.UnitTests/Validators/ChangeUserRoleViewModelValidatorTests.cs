using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class ChangeUserRoleViewModelValidatorTests
{
    [Test, MoqAutoData]
    public void When_All_Properties_Are_Populated_Should_Be_Valid(
        ChangeUserRoleViewModelValidator validator,
        ChangeUserRoleViewModel changeUserRoleViewModel
        )
    {
        // Act
        changeUserRoleViewModel.Role = Core.Models.EmployerSupport.Role.Owner;
        var result = validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test, MoqAutoData]
    public void When_Email_Is_Null_Should_Have_Validation_Error(
        ChangeUserRoleViewModelValidator validator,
        ChangeUserRoleViewModel changeUserRoleViewModel
        )
    {
        // Arrange
        changeUserRoleViewModel.Email = null;

        // Act
        var result = validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Email_Is_Empty_Should_Have_Validation_Error(
        ChangeUserRoleViewModelValidator validator,
        ChangeUserRoleViewModel changeUserRoleViewModel
        )
    {
        // Arrange
        changeUserRoleViewModel.Email = "";

        // Act
        var result = validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_HashedAccountId_Is_Null_Should_Have_Validation_Error(
        ChangeUserRoleViewModelValidator validator,
        ChangeUserRoleViewModel changeUserRoleViewModel
        )
    {
        // Arrange
        changeUserRoleViewModel.HashedAccountId = null;

        // Act
        var result = validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_HashedAccountId_Is_Empty_Should_Have_Validation_Error(
        ChangeUserRoleViewModelValidator validator,
        ChangeUserRoleViewModel changeUserRoleViewModel
        )
    {
        // Arrange       
        changeUserRoleViewModel.HashedAccountId = "";

        // Act
        var result = validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }
}
