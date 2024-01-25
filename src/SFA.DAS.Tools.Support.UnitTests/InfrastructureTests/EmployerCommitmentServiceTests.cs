using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests;

public class EmployerCommitmentServiceTests
{
    [Test, DomainAutoData]
    public async Task StoppingAnApprenticeship_ReturnsError_WhenPassedInvalidEmployerAccountId(EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
    {
        //Given
        request.AccountId = 0;
        request.ApprenticeshipId = 1;

        //When
        var result = await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("employerAccountId must be greater than 0");
    }

    [Test, DomainAutoData]
    public async Task StoppingAnApprenticeship_ReturnsError_WhenPassedInvalidApprenticeshipId(EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
    {
        //Given
        request.AccountId = 1;
        request.ApprenticeshipId = 0;

        //When
        var result = await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("apprenticeshipId must be greater than 0");
    }

    [Test, DomainAutoData]
    public async Task StoppingAnApprenticeship_ReturnsError_WhenApiCallFails([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request, List<ErrorDetail> errors)
    {
        //Given
        request.CurrentStopDate = null;
        apiClient.Setup(s => s.StopApprenticeship(request.ApprenticeshipId, It.IsAny<CommitmentsV2.Api.Types.Requests.StopApprenticeshipRequest>(), It.IsAny<CancellationToken>())).Throws(new CommitmentsApiModelException(errors));

        //When
        var result = await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        result.HasError.Should().BeTrue();
        var errorMessage = GetErrorMessages(errors);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Test, DomainAutoData]
    public async Task UpdatingStopDateOfAnApprenticeship_ReturnsError_WhenApiCallFails([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request, List<ErrorDetail> errors)
    {
        //Given
        apiClient.Setup(s => s.UpdateApprenticeshipStopDate(request.ApprenticeshipId, It.IsAny<CommitmentsV2.Api.Types.Requests.ApprenticeshipStopDateRequest>(), It.IsAny<CancellationToken>())).Throws(new CommitmentsApiModelException(errors));

        //When
        var result = await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        result.HasError.Should().BeTrue();
        var errorMessage = GetErrorMessages(errors);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Test, DomainAutoData]
    public async Task StoppingAnActiveApprenticeship_ShouldCallStopApprenticeship([Frozen]Mock<ICommitmentsApiClient> mockApiClient, EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
    {
        //Given
        request.CurrentStopDate = null;
        
        //When
        await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        mockApiClient.Verify(mock => mock.StopApprenticeship(request.ApprenticeshipId, It.IsAny<CommitmentsV2.Api.Types.Requests.StopApprenticeshipRequest>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, DomainAutoData]
    public async Task UpdatingStoppedApprenticeship_ShouldCallStopApprenticeship([Frozen] Mock<ICommitmentsApiClient> mockApiClient, EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
    {
        //Given

        //When
        await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        mockApiClient.Verify(mock => mock.UpdateApprenticeshipStopDate(request.ApprenticeshipId, It.IsAny<CommitmentsV2.Api.Types.Requests.ApprenticeshipStopDateRequest>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, DomainAutoData]
    public async Task StoppingAnApprenticeship_ReturnsSuccess_WhenApiCallSucceeds(EmployerCommitmentsService sut, Core.Models.StopApprenticeshipRequest request)
    {
        //Given

        //When
        var result = await sut.StopApprenticeship(request, new CancellationToken());

        //Then
        result.ApprenticeshipId.Should().Be(request.ApprenticeshipId);
    }

    [Test, DomainAutoData]
    public async Task GetApprenticeship_ReturnsError_WhenPassedInvalidApprenticeshipId(EmployerCommitmentsService sut)
    {
        //Given
        const int apprenticeshipId = 0;

        //When
        var result = await sut.GetApprenticeship(apprenticeshipId, new CancellationToken());

        //Then
        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("ApprenticeshipId must be greater than 0");
    }

    [Test, DomainAutoData]
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

    [Test, DomainAutoData]
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

    [Test, DomainAutoData]
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


    [Test, DomainAutoData]
    public async Task SearchApprenticeships_ReturnsSearchResults_WhenApiCallSucceeds([Frozen] Mock<ICommitmentsApiClient> apiClient, EmployerCommitmentsService sut, SearchApprenticeshipsRequest request, GetApprenticeshipsResponse response)
    {
        //Given
        apiClient.Setup(s => s.GetApprenticeships(It.Is<GetApprenticeshipsRequest>(apprenticeshipsRequest =>
                apprenticeshipsRequest.CourseName == request.CourseName &&
                apprenticeshipsRequest.EmployerName == request.EmployerName &&
                apprenticeshipsRequest.ProviderName == request.ProviderName &&
                apprenticeshipsRequest.ProviderId == request.Ukprn &&
                apprenticeshipsRequest.SearchTerm == request.SearchTerm &&
                apprenticeshipsRequest.StartDate == request.StartDate &&
                apprenticeshipsRequest.EndDate == request.EndDate)
            , It.IsAny<CancellationToken>())).ReturnsAsync(response);

        //When
        var result = await sut.SearchApprenticeships(request, new CancellationToken());

        //Then
        result.HasError.Should().BeFalse();
        result.Apprenticeships.Count.Should().Be(response.Apprenticeships.Count());
        result.Apprenticeships.Should().BeEquivalentTo(response.Apprenticeships.
            Select(detailsResponse => new
            {
                detailsResponse.ApprenticeshipStatus,
                detailsResponse.EndDate,
                detailsResponse.StartDate,
                detailsResponse.CourseName,
                detailsResponse.ProviderName,
                Ukprn = detailsResponse.ProviderId,
                detailsResponse.EmployerName,
                detailsResponse.Uln,
                detailsResponse.LastName,
                detailsResponse.FirstName,
                detailsResponse.Id,
                detailsResponse.AccountLegalEntityId,
                detailsResponse.ProviderRef,
                detailsResponse.EmployerRef,
                detailsResponse.DateOfBirth,
                detailsResponse.PauseDate,
                detailsResponse.CohortReference
            }));
    }

    private static string GetErrorMessages(IEnumerable<ErrorDetail> errors)
    {
        var concat = string.Empty;
        return errors.Aggregate(concat, (a, b) => a + " " + b.Message);
    }
}