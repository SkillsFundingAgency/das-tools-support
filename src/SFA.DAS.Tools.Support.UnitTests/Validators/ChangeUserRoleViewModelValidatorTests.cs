using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Core.Models.Challenge;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class ChangeUserRoleViewModelValidatorTests
{
    private Fixture _fixture;
    private ChangeUserRoleViewModelValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _validator = new ChangeUserRoleViewModelValidator();
    }

    [Test]
    public void When_All_Properties_Are_Populated_Should_Be_Valid()
    {
        // Arrange
        var changeUserRoleViewModel = _fixture.Build<ChangeUserRoleViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, "B")
            .With(x => x.Role, Role.Owner)
            .Create();

        // Act
        var result = _validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void When_Email_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var changeUserRoleViewModel = _fixture.Build<ChangeUserRoleViewModel>()
            .With(x => x.Email, (string)null)
            .With(x => x.HashedAccountId, "B")
            .With(x => x.Role, Role.Owner)
            .Create();

        // Act
        var result = _validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test]
    public void When_Email_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var changeUserRoleViewModel = _fixture.Build<ChangeUserRoleViewModel>()
            .With(x => x.Email, "")
            .With(x => x.HashedAccountId, "B")
            .With(x => x.Role, Role.Owner)
            .Create();

        // Act
        var result = _validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' must not be empty.");
    }

    [Test]
    public void When_HashedAccountId_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var changeUserRoleViewModel = _fixture.Build<ChangeUserRoleViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, (string)null)
            .With(x => x.Role, Role.Owner)
            .Create();

        // Act
        var result = _validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }

    [Test]
    public void When_HashedAccountId_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var changeUserRoleViewModel = _fixture.Build<ChangeUserRoleViewModel>()
            .With(x => x.Email, "A")
            .With(x => x.HashedAccountId, "")
            .With(x => x.Role, Role.Owner)
            .Create();

        // Act
        var result = _validator.TestValidate(changeUserRoleViewModel);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.HashedAccountId)
            .WithErrorMessage("'Hashed Account Id' must not be empty.");
    }  
}
