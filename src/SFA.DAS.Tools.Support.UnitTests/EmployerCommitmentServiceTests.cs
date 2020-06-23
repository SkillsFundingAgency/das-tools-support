using FluentAssertions;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class EmployerCommitmentServiceTests
    {
        [Theory, AutoMoqData]
        public async Task StoppingAnApprenticeship_ThrowsArgumentException_WhenPassedInvalidEmployerAccountId(EmployerCommitmentsService sut, long apprenticeshipId, long employerAccountId, string userId, DateTime stopDate)
        {
            //Given
            employerAccountId = 0;
            apprenticeshipId = 1;

            //When
            var result = await sut.StopApprenticeship(employerAccountId, apprenticeshipId, userId, stopDate);

            //Then
            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("employerAccountId must be greater than 0 (Parameter 'employerAccountId')");   
        }

        [Theory, AutoMoqData]
        public async Task StoppingAnApprenticeship_ThrowsArgumentException_WhenPassedInvalidApprenticeshipId(EmployerCommitmentsService sut, long apprenticeshipId, long employerAccountId, string userId, DateTime stopDate)
        {
            //Given
            employerAccountId = 1;
            apprenticeshipId = 0;

            //When
            var result = await sut.StopApprenticeship(employerAccountId, apprenticeshipId, userId, stopDate);

            //Then
            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("apprenticeshipId must be greater than 0 (Parameter 'apprenticeshipId')");
        }
    }
}
