using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

public class PauseApprovalsControllerTests
{
    private const string PauseAction = "pause";

    [Test, DomainAutoData]
    public async Task PauseApprenticeship_POST_ApiErrorOccurs_ReturnsErrorViewModel([Frozen] Mock<IEmployerCommitmentsService> api, PauseApprovalsController sut, ApprenticeshipSearchResultsViewModel model)
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
        var result = await sut.PauseApprenticeship(model);

        //Then
        result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<PauseApprenticeshipViewModel>().Which
            .HasError.Should().BeTrue();
    }

    [Test, DomainAutoData]
    public async Task PauseApprenticeship_POST_ApiReturnsResults_ReturnsViewModel([Frozen] Mock<IEmployerCommitmentsService> api, PauseApprovalsController sut, ApprenticeshipSearchResultsViewModel model)
    {
        //Given
        var fixture = new Fixture();
        var apprenticeResults = fixture.CreateMany<GetApprenticeshipResult>(3).ToList();
        var ids = apprenticeResults.Select(s => s.Apprenticeship.Id);
        apprenticeResults.ForEach(a => a.ErrorMessage = string.Empty);
        var identities = ids.ToList();
        model.SelectedIds = identities.Aggregate("", (current, value) => current + "," + value);
        api.Setup(a => a.GetApprenticeship(It.Is<long>(a => identities.Contains(a)), It.IsAny<CancellationToken>()))
            .Returns((long id, CancellationToken token) => Task.FromResult(apprenticeResults.FirstOrDefault(s => s.Apprenticeship.Id == id)));

        //When
        var result = await sut.PauseApprenticeship(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<PauseApprenticeshipViewModel>().Which;

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
            new
            {
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
            new
            {
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
            new
            {
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
    public void CancelPauseApprenticeship_POST_RedirectsToSearch(PauseApprovalsController sut, PauseApprenticeshipViewModel model)
    {
        //Given

        //When
        var result = sut.CancelPauseApprenticeship(model, PauseAction);

        //Then
        var action = result.Should().BeOfType<RedirectToActionResult>().Which;
        action.ActionName.Should().Be(RouteNames.Approval_SearchApprenticeships);
        action.RouteValues.Values.Should().BeEquivalentTo(new object[]
        {
            model.SearchParams.ApprenticeNameOrUln,
            model.SearchParams.CourseName,
            model.SearchParams.ProviderName,
            model.SearchParams.Ukprn,
            model.SearchParams.EmployerName,
            model.SearchParams.SelectedStatus,
            model.SearchParams.EndDate.Value.ToString("yyyy-MM-dd"),
            model.SearchParams.StartDate.Value.ToString("yyyy-MM-dd"),
            PauseAction
        });
    }

    [Test, DomainAutoData]
    public async Task PauseApprenticeshipConfirmation_POST_JsonDataError_ReturnsErrorViewModel(PauseApprovalsController sut, PauseApprenticeshipViewModel model)
    {
        //Given
        model.ApprenticeshipsData = "RandomData";

        //When
        var result = await sut.PauseApprenticeshipConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<PauseApprenticeshipViewModel>().Which;
        resultModel.ApprenticeshipsData.Should().BeSameAs(null);
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to Read apprenticeship information, please return to the search and try again");
    }

    [Test, DomainAutoData]
    public async Task PauseApprenticeshipConfirmation_POST_IdentityError_ReturnsErrorViewModel(PauseApprovalsController sut, PauseApprenticeshipViewModel model, List<PauseApprenticeshipRow> apprenticeshipData)
    {
        //Given
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;
        sut.ControllerContext.HttpContext = new DefaultHttpContext();

        //When
        var result = await sut.PauseApprenticeshipConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<PauseApprenticeshipViewModel>().Which;
        resultModel.ApprenticeshipsData.Should().BeSameAs(model.ApprenticeshipsData);
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to retrieve userId or name from claim for request to Pause Apprenticeship");
    }

    [Test, DomainAutoData]
    public async Task PauseApprenticeshipConfirmation_POST_DataEnteredCorrectly_SubmitsPauseToApiAndFails([Frozen] Mock<IEmployerCommitmentsService> api, PauseApprovalsController sut, PauseApprenticeshipViewModel model, List<PauseApprenticeshipRow> apprenticeshipData)
    {
        //Given
        var apprenticeshipIds = apprenticeshipData.Select(s => s.Id);
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;

        api.Setup(s => s.PauseApprenticeship(
                It.Is<PauseApprenticeshipRequest>(r => apprenticeshipIds.Contains(r.ApprenticeshipId)), It.IsAny<CancellationToken>()))
            .Returns((PauseApprenticeshipRequest request, CancellationToken token) => Task.FromResult(new PauseApprenticeshipResult
            {
                ApprenticeshipId = request.ApprenticeshipId,
                ErrorMessage = $"Errored For {request.ApprenticeshipId}"
            }));

        //When
        var result = await sut.PauseApprenticeshipConfirmation(model);

        //Then

        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<PauseApprenticeshipViewModel>().Which;
        resultModel.Apprenticeships.All(s => s.ApiSubmissionStatus == SubmissionStatus.Errored && s.ApiErrorMessage.Equals($"Errored For {s.Id}")).Should().BeTrue();
        resultModel.HasError.Should().BeFalse();
    }

    [Test, DomainAutoData]
    public async Task PauseApprenticeshipConfirmation_POST_DataEnteredCorrectly_SubmitsPauseToApiAndSucceeds([Frozen] Mock<IEmployerCommitmentsService> api, PauseApprovalsController sut, PauseApprenticeshipViewModel model, List<PauseApprenticeshipRow> apprenticeshipData)
    {
        //Given
        var apprenticeshipIds = apprenticeshipData.Select(s => s.Id);
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.ApprenticeshipsData = jsonData;

        api.Setup(s => s.PauseApprenticeship(
                It.Is<PauseApprenticeshipRequest>(r => apprenticeshipIds.Contains(r.ApprenticeshipId)), It.IsAny<CancellationToken>()))
            .Returns((PauseApprenticeshipRequest request, CancellationToken token) => Task.FromResult(new PauseApprenticeshipResult
            {
                ApprenticeshipId = request.ApprenticeshipId
            }));

        //When
        var result = await sut.PauseApprenticeshipConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<PauseApprenticeshipViewModel>().Which;
        
        resultModel.Apprenticeships.All(s => s.ApiSubmissionStatus == SubmissionStatus.Successful).Should().BeTrue();
        resultModel.HasError.Should().BeFalse();
    }
}