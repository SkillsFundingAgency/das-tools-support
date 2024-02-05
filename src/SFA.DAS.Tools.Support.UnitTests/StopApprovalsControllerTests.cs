using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SFA.DAS.Tools.Support.UnitTests;

public class StopApprovalsControllerTests
{
    private const string StopAction = "stop";

    [Test, DomainAutoData]
    public async Task StopApprenticeship_POST_NoRowsSelected_RedirectsToSearch(StopApprovalsController sut, ApprenticeshipSearchResultsViewModel model)
    {
        //Given
        model.SelectedIds = null;

        //When
        var result = await sut.StopApprenticeship(model);

        //Then
        var action = result.Should().BeOfType<RedirectToActionResult>().Which;
        action.ActionName.Should().Be(RouteNames.Approval_SearchApprenticeships);
        action.RouteValues.Values.Should().BeEquivalentTo(new object []
        {
            model.ApprenticeNameOrUln,
            model.CourseName,
            model.ProviderName,
            model.Ukprn,
            model.EmployerName,
            model.Status,
            model.EndDate.Value.ToString("yyyy-MM-dd"),
            model.StartDate.Value.ToString("yyyy-MM-dd"),
            StopAction
        });
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeship_POST_ApiErrorOccurs_ReturnsErrorViewModel([Frozen] Mock<IEmployerCommitmentsService> api, StopApprovalsController sut, ApprenticeshipSearchResultsViewModel model)
    {
        //Given
        var fixture = new Fixture();
        var apprenticeResults = fixture.CreateMany<GetApprenticeshipResult>(3).ToList();
        var ids = apprenticeResults.Select(s => s.Apprenticeship.Id);
        apprenticeResults.ForEach(a => a.ErrorMessage = string.Empty);
        apprenticeResults.Last().ErrorMessage = "Api ErrorMessage";
        var identities = ids.ToList();
        model.SelectedIds = identities.Aggregate("", (current, value) => current + "," + value);
        api.Setup(a => a.GetApprenticeship(It.Is<long>(a => identities.Contains(a)), It.IsAny<CancellationToken>())).Returns((long id, CancellationToken token) =>
            Task.FromResult(apprenticeResults.FirstOrDefault(s => s.Apprenticeship.Id == id)));

        //When
        var result = await sut.StopApprenticeship(model);

        //Then
        result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which
            .HasError.Should().BeTrue();
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeship_POST_ApiReturnsResults_ReturnsViewModel([Frozen] Mock<IEmployerCommitmentsService> api, StopApprovalsController sut, ApprenticeshipSearchResultsViewModel model)
    {
        //Given
        var fixture = new Fixture();
        var apprenticeResults = fixture.CreateMany<GetApprenticeshipResult>(3).ToList();
        var ids = apprenticeResults.Select(s => s.Apprenticeship.Id);
        apprenticeResults.ForEach(a => a.ErrorMessage = string.Empty);
        model.SelectedIds = ids.Aggregate("", (current, value) => current + "," + value);
        api.Setup(a => a.GetApprenticeship(It.Is<long>(a => ids.Contains(a)), It.IsAny<CancellationToken>()))
            .Returns((long id, CancellationToken token) => Task.FromResult(apprenticeResults.Where(s => s.Apprenticeship.Id == id).FirstOrDefault()));

        //When
        var result = await sut.StopApprenticeship(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;

        resultModel.SearchParams.Should().BeEquivalentTo(new
        {
            model.ApprenticeNameOrUln,
            model.CourseName,
            model.EmployerName,
            model.ProviderName,
            model.Ukprn,
            SelectedStatus = model.Status,
            model.StartDate,
            model.EndDate
        });

        resultModel.Apprenticeships.Should().BeEquivalentTo(new object[]
        {
            new {
                apprenticeResults[0].Apprenticeship.Id,
                AccountId = apprenticeResults[0].Apprenticeship.EmployerAccountId,
                apprenticeResults[0].Apprenticeship.FirstName,
                apprenticeResults[0].Apprenticeship.LastName,
                apprenticeResults[0].Apprenticeship.Uln,
                apprenticeResults[0].Apprenticeship.EmployerName,
                apprenticeResults[0].Apprenticeship.ProviderName,
                apprenticeResults[0].Apprenticeship.Ukprn,
                apprenticeResults[0].Apprenticeship.CourseName,
                Status = apprenticeResults[0].Apprenticeship.ApprenticeshipStatus.ToString(),
                PaymentStatus = apprenticeResults[0].Apprenticeship.PaymentStatus.ToString(),
                apprenticeResults[0].Apprenticeship.StartDate,
                apprenticeResults[0].Apprenticeship.EndDate,
                ApiSubmissionStatus = SubmissionStatus.NotSent
            },
            new {
                apprenticeResults[1].Apprenticeship.Id,
                AccountId = apprenticeResults[1].Apprenticeship.EmployerAccountId,
                apprenticeResults[1].Apprenticeship.FirstName,
                apprenticeResults[1].Apprenticeship.LastName,
                apprenticeResults[1].Apprenticeship.Uln,
                apprenticeResults[1].Apprenticeship.EmployerName,
                apprenticeResults[1].Apprenticeship.ProviderName,
                apprenticeResults[1].Apprenticeship.Ukprn,
                apprenticeResults[1].Apprenticeship.CourseName,
                Status = apprenticeResults[1].Apprenticeship.ApprenticeshipStatus.ToString(),
                PaymentStatus = apprenticeResults[1].Apprenticeship.PaymentStatus.ToString(),
                apprenticeResults[1].Apprenticeship.StartDate,
                apprenticeResults[1].Apprenticeship.EndDate,
                ApiSubmissionStatus = SubmissionStatus.NotSent
            },
            new {
                apprenticeResults[2].Apprenticeship.Id,
                AccountId = apprenticeResults[2].Apprenticeship.EmployerAccountId,
                apprenticeResults[2].Apprenticeship.FirstName,
                apprenticeResults[2].Apprenticeship.LastName,
                apprenticeResults[2].Apprenticeship.Uln,
                apprenticeResults[2].Apprenticeship.EmployerName,
                apprenticeResults[2].Apprenticeship.ProviderName,
                apprenticeResults[2].Apprenticeship.Ukprn,
                apprenticeResults[2].Apprenticeship.CourseName,
                Status = apprenticeResults[2].Apprenticeship.ApprenticeshipStatus.ToString(),
                PaymentStatus = apprenticeResults[2].Apprenticeship.PaymentStatus.ToString(),
                apprenticeResults[2].Apprenticeship.StartDate,
                apprenticeResults[2].Apprenticeship.EndDate,
                ApiSubmissionStatus = SubmissionStatus.NotSent
            }
        });
    }


    [Test, DomainAutoData]
    public void CancelStopApprenticeship_POST_RedirectsToSearch(StopApprovalsController sut, StopApprenticeshipViewModel model)
    {
        //Given

        //When
        var result = sut.CancelStopApprenticeship(model);

        //Then
        var action = result.Should().BeOfType<RedirectToActionResult>().Which;
        action.ActionName.Should().Be(RouteNames.Approval_SearchApprenticeships);
        action.RouteValues.Values.Should().BeEquivalentTo(new object []
        {
            model.SearchParams.ApprenticeNameOrUln,
            model.SearchParams.CourseName,
            model.SearchParams.ProviderName,
            model.SearchParams.Ukprn,
            model.SearchParams.EmployerName,
            model.SearchParams.SelectedStatus,
            model.SearchParams.EndDate.Value.ToString("yyyy-MM-dd"),
            model.SearchParams.StartDate.Value.ToString("yyyy-MM-dd"),
            StopAction
        });
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeshipConfirmation_POST_JsonDataError_ReturnsErrorViewModel(StopApprovalsController sut, StopApprenticeshipViewModel model)
    {
        //Given
        model.ApprenticeshipsData = "RandomData";

        //When
        var result = await sut.StopApprenticeshipConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;
        resultModel.ApprenticeshipsData.Should().BeSameAs(null);
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to Read apprenticeship information, please return to the search and try again");
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeshipConfirmation_POST_IdentityError_ReturnsErrorViewModel(StopApprovalsController sut, StopApprenticeshipViewModel model, List<StopApprenticeshipRow> apprenticeshipData)
    {
        //Given
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;
        sut.ControllerContext.HttpContext = new DefaultHttpContext();

        //When
        var result = await sut.StopApprenticeshipConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;
        resultModel.ApprenticeshipsData.Should().BeSameAs(model.ApprenticeshipsData);
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to retrieve userId or name from claim for request to stop apprenticeship");
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeshipConfirmation_POST_NotAllStopDatesEntered_ReturnsErrorViewModel(StopApprovalsController sut, StopApprenticeshipViewModel model, List<StopApprenticeshipRow> apprenticeshipData)
    {
        //Given
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;

        //When
        var result = await sut.StopApprenticeshipConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;
        resultModel.ApprenticeshipsData.Should().BeSameAs(model.ApprenticeshipsData);
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Not all Apprenticeship rows have been supplied with a stop date.");
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeshipConfirmation_POST_DataEnteredCorrectly_SubmitsStopToApiAndFails([Frozen] Mock<IEmployerCommitmentsService> api, StopApprovalsController sut, StopApprenticeshipViewModel model, List<StopApprenticeshipRow> apprenticeshipData)
    {
        //Given
        apprenticeshipData.ForEach(s => s.EnteredDate = DateTime.Today.ToString("yyyy-MM-dd"));
        var apprenticeshipIds = apprenticeshipData.Select(s => s.Id);
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;

        api.Setup(s => s.StopApprenticeship(
                It.Is<StopApprenticeshipRequest>(r => apprenticeshipIds.Contains(r.ApprenticeshipId)), It.IsAny<CancellationToken>()))
            .Returns((StopApprenticeshipRequest request, CancellationToken token) => Task.FromResult(new StopApprenticeshipResult
            {
                ApprenticeshipId = request.ApprenticeshipId,
                ErrorMessage = $"Errored For {request.ApprenticeshipId}"
            }));

        //When
        var result = await sut.StopApprenticeshipConfirmation(model);

        //Then

        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;
        resultModel.Apprenticeships.All(s => s.ApiSubmissionStatus == SubmissionStatus.Errored && s.ApiErrorMessage.Equals($"Errored For {s.Id}")).Should().BeTrue();
        resultModel.HasError.Should().BeFalse();
    }

    [Test, DomainAutoData]
    public async Task StopApprenticeshipConfirmation_POST_DataEnteredCorrectly_SubmitsStopToApiAndSucceeds([Frozen] Mock<IEmployerCommitmentsService> api, StopApprovalsController sut, StopApprenticeshipViewModel model, List<StopApprenticeshipRow> apprenticeshipData)
    {
        //Given
        apprenticeshipData.ForEach(s => s.EnteredDate = DateTime.Today.ToString("yyyy-MM-dd"));
        var apprenticeshipIds = apprenticeshipData.Select(s => s.Id);
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;

        api.Setup(s => s.StopApprenticeship(
                It.Is<StopApprenticeshipRequest>(r => apprenticeshipIds.Contains(r.ApprenticeshipId)), It.IsAny<CancellationToken>()))
            .Returns((StopApprenticeshipRequest request, CancellationToken token) => Task.FromResult(new StopApprenticeshipResult
            {
                ApprenticeshipId = request.ApprenticeshipId
            }));

        //When
        var result = await sut.StopApprenticeshipConfirmation(model);

        //Then

        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;
        resultModel.Apprenticeships.All(s => s.ApiSubmissionStatus == SubmissionStatus.Successful).Should().BeTrue();
        resultModel.HasError.Should().BeFalse();
    }

    [Test, DomainAutoData]
    public async Task UpdateStopApprenticeshipConfirmation_POST_DataEnteredCorrectly_StopResultDateAddedToViewModel(
        [Frozen] Mock<IEmployerCommitmentsService> api, 
        StopApprovalsController sut, 
        StopApprenticeshipViewModel model, 
        List<StopApprenticeshipRow> apprenticeshipData)
    {
        //Given
        var newStopDate = DateTime.Today;
        apprenticeshipData.ForEach(s =>
        {
            s.StatusDate = DateTime.Today.AddMonths(-2);
            s.Status = ApprenticeshipStatus.Stopped.ToString();
            s.EnteredDate = newStopDate.ToString("yyyy-MM-dd"); ;
        });

        var apprenticeshipIds = apprenticeshipData.Select(s => s.Id);
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;

        api.Setup(s => s.StopApprenticeship(
                It.Is<StopApprenticeshipRequest>(r => apprenticeshipIds.Contains(r.ApprenticeshipId)), It.IsAny<CancellationToken>()))
            .Returns((StopApprenticeshipRequest request, CancellationToken token) => Task.FromResult(new StopApprenticeshipResult
            {
                ApprenticeshipId = request.ApprenticeshipId,
                StopDate = request.RequestedStopDate
            }));

        //When
        var result = await sut.StopApprenticeshipConfirmation(model);

        //Then

        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<StopApprenticeshipViewModel>().Which;
        resultModel.Apprenticeships.All(s => s.ApiSubmissionStatus == SubmissionStatus.Successful).Should().BeTrue();
        resultModel.HasError.Should().BeFalse();
        resultModel.Apprenticeships.First().StatusDate.Should().Be(newStopDate);
    }
}