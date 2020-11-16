using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Moq;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class EmployerCommitmentServiceTests
    {
        [Theory, AutoMoqData]
        public async Task StoppingAnApprenticeship_ReturnsError_WhenPassedInvalidEmployerAccountId(EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
        {
            //Given
            request.AccountId = 0;
            request.ApprenticeshipId = 1;

            //When
            var result = await sut.StopApprenticeship(request, new CancellationToken());

            //Then
            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("employerAccountId must be greater than 0 (Parameter 'employerAccountId')");
        }

        [Theory, AutoMoqData]
        public async Task StoppingAnApprenticeship_ReturnsError_WhenPassedInvalidApprenticeshipId(EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
        {
            //Given
            request.AccountId = 1;
            request.ApprenticeshipId = 0;

            //When
            var result = await sut.StopApprenticeship(request, new CancellationToken());

            //Then
            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("apprenticeshipId must be greater than 0 (Parameter 'apprenticeshipId')");
        }

        [Theory, AutoMoqData]
        public async Task StoppingAnApprenticeship_ReturnsError_WhenApiCallFails([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request, List<ErrorDetail> errors)
        {
            //Given
            apiClient.Setup(s => s.StopApprenticeship(request.ApprenticeshipId, It.IsAny<CommitmentsV2.Api.Types.Requests.StopApprenticeshipRequest>(), It.IsAny<CancellationToken>())).Throws(new CommitmentsApiModelException(errors));

            //When
            var result = await sut.StopApprenticeship(request, new CancellationToken());

            //Then
            result.HasError.Should().BeTrue();
            var errorMessage = GetErrorMessages(errors);
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Theory, AutoMoqData]
        public async Task StoppingAnApprenticeship_ReturnsSuccess_WhenApiCallSucceeds(EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
        {
            //Given

            //When
            var result = await sut.StopApprenticeship(request, new CancellationToken());

            //Then
            result.Should().BeEquivalentTo(new StopApprenticeshipResult { ApprenticeshipId = request.ApprenticeshipId });
        }

        [Theory, AutoMoqData]
        public async Task GetApprenticeship_ReturnsError_WhenPassedInvalidApprenticeshipId(EmployerCommitmentsService sut)
        {
            //Given
            var apprenticeshipId = 0;

            //When
            var result = await sut.GetApprenticeship(apprenticeshipId, new CancellationToken());

            //Then
            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("ApprenticeshipId must be greater than 0 (Parameter 'apprenticeshipId')");
        }

        [Theory, AutoMoqData]
        public async Task GetApprenticeship_ReturnsError_WhenApiCallFails([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, long apprenticeshipId, List<ErrorDetail> errors)
        {
            //Given
            apiClient.Setup(s => s.GetApprenticeship(apprenticeshipId, It.IsAny<CancellationToken>())).Throws(new CommitmentsApiModelException(errors));

            //When
            var result = await sut.GetApprenticeship(apprenticeshipId, new CancellationToken());

            //Then
            var errorMessage = GetErrorMessages(errors);
            result.ErrorMessage.Should().Be(errorMessage);
        }

        [Theory, AutoMoqData]
        public async Task GetApprenticeship_ReturnsApprenticeship_WhenApiCallSucceeds([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, long apprenticeshipId, GetApprenticeshipResponse response)
        {
            //Given
            apiClient.Setup(s => s.GetApprenticeship(apprenticeshipId, It.IsAny<CancellationToken>())).ReturnsAsync(response);

            //When
            var result = await sut.GetApprenticeship(apprenticeshipId, new CancellationToken());

            //Then
            result.Apprenticeship.Should().BeEquivalentTo(new
            {
                response.Id,
                response.EmployerAccountId,
                response.EmployerName,
                Ukprn = response.ProviderId,
                response.ProviderName,
                response.AccountLegalEntityId,
                response.FirstName,
                response.LastName,
                response.StartDate,
                response.EndDate,
                ApprenticeshipStatus = response.Status,
                response.Uln
            });
        }

        [Theory, AutoMoqData]
        public async Task SearchApprenticeships_ReturnsErrorMessage_WhenApiCallFails([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, SearchApprenticeshipsRequest request, List<ErrorDetail> errors)
        {
            //Given
            apiClient.Setup(s => s.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(s =>
                s.CourseName == request.CourseName &&
                s.EmployerName == request.EmployerName &&
                s.ProviderName == request.ProviderName &&
                s.ProviderId == request.Ukprn &&
                s.SearchTerm == request.SearchTerm &&
                s.StartDate == request.StartDate &&
                s.EndDate == request.EndDate)
            , It.IsAny<CancellationToken>())).Throws(new CommitmentsApiModelException(errors));

            //When
            var result = await sut.SearchApprenticeships(request, new CancellationToken());

            //Then
            var errorMessage = GetErrorMessages(errors);
            result.ErrorMessage.Should().Be(errorMessage);
        }


        [Theory, AutoMoqData]
        public async Task SearchApprenticeships_ReturnsSearchResults_WhenApiCallSucceeds([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, SearchApprenticeshipsRequest request, GetApprenticeshipsResponse response)
        {
            //Given
            apiClient.Setup(s => s.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(s =>
                s.CourseName == request.CourseName &&
                s.EmployerName == request.EmployerName &&
                s.ProviderName == request.ProviderName &&
                s.ProviderId == request.Ukprn &&
                s.SearchTerm == request.SearchTerm &&
                s.StartDate == request.StartDate &&
                s.EndDate == request.EndDate)
            , It.IsAny<CancellationToken>())).ReturnsAsync(response);

            //When
            var result = await sut.SearchApprenticeships(request, new CancellationToken());

            //Then
            result.HasError.Should().BeFalse();
            result.Apprenticeships.Count.Should().Be(response.Apprenticeships.Count());
            result.Apprenticeships.Should().BeEquivalentTo(response.Apprenticeships.
                Select(s => new
                {
                    s.ApprenticeshipStatus,
                    s.EndDate,
                    s.StartDate,
                    s.CourseName,
                    s.ProviderName,
                    s.Ukprn,
                    s.EmployerName,
                    s.Uln,
                    s.LastName,
                    s.FirstName,
                    s.Id,
                    s.AccountLegalEntityId,
                    s.ProviderRef,
                    s.EmployerRef,
                    s.DateOfBirth,
                    s.PauseDate,
                    s.CohortReference
             }));
        }

        private string GetErrorMessages(List<ErrorDetail> errors)
        {
            var concat = string.Empty;
            return errors.Aggregate(concat, (a, b) => a + " " + b.Message);
        }
    }
}