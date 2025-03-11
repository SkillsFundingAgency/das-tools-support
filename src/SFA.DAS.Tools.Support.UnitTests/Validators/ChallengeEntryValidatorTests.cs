using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models.Challenge;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class ChallengeEntryValidatorTests
{

    [Test, MoqAutoData]
    public void When_All_Properties_Are_Populated_Should_Be_Valid(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test, MoqAutoData]
    public void When_Challenge1_Is_Null_Should_Have_Validation_Error(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Arrange     
        challengeEntry.Challenge1 = null;
        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge1)
            .WithErrorMessage("'Challenge1' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Challenge1_Is_Empty_Should_Have_Validation_Error(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Arrange      
        challengeEntry.Challenge1 = "";

        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge1)
            .WithErrorMessage("'Challenge1' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Challenge2_Is_Null_Should_Have_Validation_Error(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Arrange      
        challengeEntry.Challenge2 = null;

        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge2)
            .WithErrorMessage("'Challenge2' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Challenge2_Is_Empty_Should_Have_Validation_Error(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Arrange   
        challengeEntry.Challenge2 = "";

        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge2)
            .WithErrorMessage("'Challenge2' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Balance_Is_Null_Should_Have_Validation_Error(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Arrange
        challengeEntry.Balance = null;

        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("'Balance' must not be empty.");
    }

    [Test, MoqAutoData]
    public void When_Balance_Is_Empty_Should_Have_Validation_Error(
        ChallengeEntryValidator validator,
        ChallengeEntry challengeEntry
        )
    {
        // Arrange
        challengeEntry.Balance = "";

        // Act
        var result = validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("'Balance' must not be empty.");
    }
}
