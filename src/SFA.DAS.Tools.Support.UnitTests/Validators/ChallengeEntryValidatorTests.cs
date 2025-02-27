using AutoFixture;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Core.Models.Challenge;
using SFA.DAS.Tools.Support.Web.Validators.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Validators;
[TestFixture]
public class ChallengeEntryValidatorTests
{
    private Fixture _fixture;
    private ChallengeEntryValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _validator = new ChallengeEntryValidator();
    }

    [Test]
    public void When_All_Properties_Are_Populated_Should_Be_Valid()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, "A")
            .With(x => x.Challenge2, "B")
            .With(x => x.Balance, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void When_Challenge1_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, (string)null) 
            .With(x => x.Challenge2, "B")
            .With(x => x.Balance, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge1)
            .WithErrorMessage("'Challenge1' must not be empty.");
    }

    [Test]
    public void When_Challenge1_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, "")
            .With(x => x.Challenge2, "B")
            .With(x => x.Balance, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge1)
            .WithErrorMessage("'Challenge1' must not be empty.");
    }

    [Test]
    public void When_Challenge2_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, "A")
            .With(x => x.Challenge2, (string)null)
            .With(x => x.Balance, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge2)
            .WithErrorMessage("'Challenge2' must not be empty.");
    }

    [Test]
    public void When_Challenge2_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, "A")
            .With(x => x.Challenge2, "")
            .With(x => x.Balance, "1000")
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Challenge2)
            .WithErrorMessage("'Challenge2' must not be empty.");
    }

    [Test]
    public void When_Balance_Is_Null_Should_Have_Validation_Error()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, "A")
            .With(x => x.Challenge2, "B")
            .With(x => x.Balance, (string)null)
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("'Balance' must not be empty.");
    }

    [Test]
    public void When_Balance_Is_Empty_Should_Have_Validation_Error()
    {
        // Arrange
        var challengeEntry = _fixture.Build<ChallengeEntry>()
            .With(x => x.Challenge1, "A")
            .With(x => x.Challenge2, "B")
            .With(x => x.Balance, "")
            .Create();

        // Act
        var result = _validator.TestValidate(challengeEntry);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Balance)
            .WithErrorMessage("'Balance' must not be empty.");
    }
}
