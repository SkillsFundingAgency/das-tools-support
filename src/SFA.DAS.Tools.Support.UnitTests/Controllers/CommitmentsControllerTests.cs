using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;
using SFA.DAS.Tools.Support.Infrastructure.Cache;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Mapping;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

public class CommitmentsControllerTests
{
    [Test, MoqAutoData]
    public async Task CommitmentsSearch_ShouldReturnDefaultViewModel_WhenCalled(
        Account account,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Greedy] CommitmentsController controller
       )
    {
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>())).ReturnsAsync(account);

        // Act
        var response = await controller.CommitmentSearch(account.HashedAccountId, "", null, null) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.Model.Should().BeOfType<CommitmentSearchViewModel>();
        var model = response.Model as CommitmentSearchViewModel;
        model.Account.Should().BeEquivalentTo(account);
        model.SelectedTab.Should().Be(AccountFieldsSelection.CommitmentSearch);
        model.SearchTerm.Should().BeNull();
        model.SearchType.Should().Be(ApprenticeshipSearchType.SearchByUln);
    }

    [Test, MoqAutoData]
    public async Task CommitmentsSearch_ShouldReturnViewModelWithSearchTypeAndTerm_WhenCalled(
        Account account,
        string searchTerm,
        ApprenticeshipSearchType searchType,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Greedy] CommitmentsController controller
    )
    {
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>())).ReturnsAsync(account);

        // Act
        var response = await controller.CommitmentSearch(account.HashedAccountId, searchTerm, searchType, null) as ViewResult;

        // Assert

        var model = response.Model as CommitmentSearchViewModel;
        model.SearchTerm.Should().Be(searchTerm);
        model.SearchType.Should().Be(searchType);
    }

    [TestCase(MatchFailure.NoneFound, "Not found")]
    [TestCase(MatchFailure.AccessDenied, "Account is unauthorised to access this Cohort")]
    public async Task CommitmentsSearch_ShouldReturnErrorMessageForFailureMatch_WhenCalled(MatchFailure failure, string expectedMessage)
    {
        var fixture = new Fixture();
        var account = fixture.Create<Account>();

        var cacheService = new Mock<ICacheService>();
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>())).ReturnsAsync(account);
        var controller = new CommitmentsController(Mock.Of<IMediator>(), Mock.Of<IEncodingService>(), cacheService.Object);

        // Act
        await controller.CommitmentSearch(account.HashedAccountId, "searchTerm", ApprenticeshipSearchType.SearchByCohort, failure);

        // Assert
        controller.ModelState.IsValid.Should().BeFalse();
        controller.ModelState.Count.Should().Be(1);
        controller.ModelState["SearchTerm"].Errors.First().ErrorMessage.Should().Be(expectedMessage);
    }

    [TestCase(ApprenticeshipSearchType.SearchByUln, "CommitmentUlnSearch")]
    [TestCase(ApprenticeshipSearchType.SearchByCohort, "ViewCohortDetails")]
    public async Task CommitmentsSearchPost_ShouldRedirect_To(ApprenticeshipSearchType type, string expectedAction)
    {
        var fixture = new Fixture();
        var controller = new CommitmentsController(Mock.Of<IMediator>(), Mock.Of<IEncodingService>(), Mock.Of<ICacheService>());

        var model = new CommitmentSearchViewModel
        {
            SearchType = type
        };

        // Act
        var response = await controller.CommitmentSearch("XXXXX", model) as RedirectToActionResult;

        // Assert
        response.ActionName.Should().Be(expectedAction);
   }

    [Test, MoqAutoData]
    public async Task CommitmentsSearchPost_ShouldReturnViewModelWithAccountDataAndViewModelField_WhenCalled(
        Account account,
        CommitmentSearchViewModel viewModel,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Greedy] CommitmentsController controller
    )
    {
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>())).ReturnsAsync(account);
        controller.ModelState.AddModelError("SearchTerm", "Test Error");

        // Act
        var response = await controller.CommitmentSearch(account.HashedAccountId, viewModel) as ViewResult;

        // Assert

        var model = response.Model as CommitmentSearchViewModel;
        model.SearchTerm.Should().Be(viewModel.SearchTerm);
        model.SearchType.Should().Be(viewModel.SearchType);
        model.Account.Should().BeEquivalentTo(account);
    }

    [Test, MoqAutoData]
    public async Task CommitmentsUlnSearch_ShouldRedirectToSearchPage_WhenNoRecordsFound(
        string uln,
        GetUlnDetailsQueryResult result,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Frozen] Mock<IMapper<ApprovedApprenticeshipUlnSummary, ApprenticeshipUlnSummary>> mockMapper,
        [Greedy] CommitmentsController controller
    )
    {
        result.Apprenticeships = new List<ApprovedApprenticeshipUlnSummary>();
        mockMediator.Setup(x => x.Send(It.Is<GetUlnDetailsQuery>(p => p.Uln == uln), It.IsAny<CancellationToken>())).ReturnsAsync(result);

        // Act
        var response = await controller.CommitmentUlnSearch("XXX", uln, mockMapper.Object) as RedirectToActionResult;

        // Assert
        response.ActionName.Should().Be("CommitmentSearch");
    }

    [Test, MoqAutoData]
    public async Task CommitmentsUlnSearch_ShouldDisplayList_WhenRecordsFound(
        string uln,
        Account account,
        GetUlnDetailsQueryResult result,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Frozen] Mock<IMapper<ApprovedApprenticeshipUlnSummary, ApprenticeshipUlnSummary>> mockMapper,
        [Greedy] CommitmentsController controller
    )
    {
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>()))
            .ReturnsAsync(account);
        mockMediator.Setup(x => x.Send(It.Is<GetUlnDetailsQuery>(p => p.Uln == uln), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.CommitmentUlnSearch("XXX", uln, mockMapper.Object) as ViewResult;

        // Assert
        var model = response.Model as CommitmentUlnSearchViewModel;
        model.Account.Should().BeEquivalentTo(account);
        model.SelectedTab.Should().Be(AccountFieldsSelection.CommitmentSearch);
        model.Uln.Should().Be(uln);
        model.HashedAccountId.Should().Be("XXX");
        model.Apprenticeships.Count.Should().Be(result.Apprenticeships.Count);
    }

    [Test, MoqAutoData]
    public async Task CommitmentsCohortSearch_ShouldRedirectToSearchPage_WhenNoRecordsFound(
        string cohortRef,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Frozen] Mock<IMapper<ApprovedApprenticeshipCohortSummary, ApprenticeshipCohortSummary>> mockMapper,
        [Greedy] CommitmentsController controller
    )
    {
        //result.Apprenticeships = new List<ApprovedApprenticeshipCohortSummary>();
        mockMediator.Setup(x =>
                x.Send(It.Is<GetCohortDetailsQuery>(p => p.CohortRef == cohortRef), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (GetCohortDetailsQueryResult) null);

        // Act
        var response = await controller.ViewCohortDetails("XXX", cohortRef, mockMapper.Object) as RedirectToActionResult;

        // Assert
        response.ActionName.Should().Be("CommitmentSearch");
        response.RouteValues["failure"].Should().Be(MatchFailure.NoneFound);
    }

    [Test, MoqAutoData]
    public async Task CommitmentsCohortSearch_ShouldRedirectToSearchPage_WhenCohortBelongToAnotherAccount(
        string cohortRef,
        GetCohortDetailsQueryResult result,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Frozen] Mock<IMapper<ApprovedApprenticeshipCohortSummary, ApprenticeshipCohortSummary>> mockMapper,
        [Greedy] CommitmentsController controller
    )
    {
        mockMediator.Setup(x =>
                x.Send(It.Is<GetCohortDetailsQuery>(p => p.CohortRef == cohortRef), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.ViewCohortDetails("XXX", cohortRef, mockMapper.Object) as RedirectToActionResult;

        // Assert
        response.ActionName.Should().Be("CommitmentSearch");
        response.RouteValues["failure"].Should().Be(MatchFailure.AccessDenied);
    }

    [Test, MoqAutoData]
    public async Task CommitmentsCohortSearch_ShouldDisplayCohort_WhenRecordFound(
        string cohortRef,
        Account account,
        GetCohortDetailsQueryResult result,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Frozen] Mock<IMapper<ApprovedApprenticeshipCohortSummary, ApprenticeshipCohortSummary>> mockMapper,
        [Greedy] CommitmentsController controller
    )
    {
        result.HashedAccountId = "XXX";
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>()))
            .ReturnsAsync(account);
        mockMediator.Setup(x => x.Send(It.Is<GetCohortDetailsQuery>(p => p.CohortRef == cohortRef), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.ViewCohortDetails("XXX", cohortRef, mockMapper.Object) as ViewResult;

        // Assert
        var model = response.Model as CohortDetailsViewModel;
        model.Account.Should().BeEquivalentTo(account);
        model.SelectedTab.Should().Be(AccountFieldsSelection.CommitmentSearch);
        model.CohortId.Should().Be(result.CohortId);
        model.CohortReference.Should().Be(cohortRef);
        model.HashedAccountId.Should().Be("XXX");
        model.EmployerAccountName.Should().Be(result.EmployerAccountName);
        model.ProviderName.Should().Be(result.ProviderName);
        model.CohortStatus.Should().Be(result.CohortStatus);
        model.Apprenticeships.Count.Should().Be(result.Apprenticeships.Count);
    }

    [Test, MoqAutoData]
    public async Task ViewApprenticeshipDetails_ShouldDetails(
        string hashedAccountId,
        string hashedApprenticeshipId,
        Account account,
        GetApprenticeshipDetailsQueryResult result,
        [Frozen] Mock<ICacheService> cacheService,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<IEncodingService> mockEncoder,
        [Greedy] CommitmentsController controller
    )
    {
        cacheService.Setup(x => x.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<Account>>>(), It.IsAny<int>()))
            .ReturnsAsync(account);
        mockMediator.Setup(x =>
                x.Send(
                    It.Is<GetApprenticeshipDetailsQuery>(p =>
                        p.HashedAccountId == hashedAccountId && p.HashedApprenticeshipId == hashedApprenticeshipId),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.ViewApprenticeshipDetails(hashedAccountId, hashedApprenticeshipId) as ViewResult;

        // Assert
        var model = response.Model as ApprenticeshipDetailsViewModel;
        model.Account.Should().BeEquivalentTo(account);
        model.SelectedTab.Should().Be(AccountFieldsSelection.CommitmentSearch);
        model.PaymentStatus.Should().Be(result.PaymentStatus);
        model.AgreementStatus.Should().Be(result.AgreementStatus);
        model.ConfirmationStatusDescription.Should().Be(result.ConfirmationStatusDescription);
        model.FirstName.Should().Be(result.FirstName);
        model.LastName.Should().Be(result.LastName);
        model.Email.Should().Be(result.Email);
        model.Uln.Should().Be(result.Uln);
        model.DateOfBirth.Should().Be(result.DateOfBirth);
        model.CohortReference.Should().Be(result.CohortReference);
        model.EmployerReference.Should().Be(result.EmployerReference);
        model.LegalEntity.Should().Be(result.LegalEntity);
        model.TrainingProvider.Should().Be(result.TrainingProvider);
        model.UKPRN.Should().Be(result.UKPRN);
        model.Trainingcourse.Should().Be(result.Trainingcourse);
        model.ApprenticeshipCode.Should().Be(result.ApprenticeshipCode);
        model.TrainingStartDate.Should().Be(result.TrainingStartDate);
        model.TrainingEndDate.Should().Be(result.TrainingEndDate);
        model.Version.Should().Be(result.Version);
        model.Option.Should().Be(result.Option);
        model.PauseDate.Should().Be(result.PauseDate);
        model.StopDate.Should().Be(result.StopDate);
        model.CompletionDate.Should().Be(result.CompletionDate);
        model.MadeRedundant.Should().Be(result.MadeRedundant);
        model.OverlappingTrainingDateRequestCreatedOn.Should().Be(result.OverlappingTrainingDateRequestCreatedOn);
        model.PendingChangesDescription.Should().Be(result.PendingChanges.Description);
        model.PendingChanges.Should().BeEquivalentTo(result.PendingChanges.Changes);
        model.ChangeOfProviderChain.Should().BeEquivalentTo(result.ChangeOfProviderChain);
    }

    [Test, MoqAutoData]
    public void ShouldMapFromFromApprovedApprenticeshipCohortSummaryToApprenticeshipCohortSummary(
        ApprovedApprenticeshipCohortSummary apprenticeship,
        [Frozen] Mock<IEncodingService> mockEncoder,
        ApprenticeshipCohortSummaryMapper sut
    )
    {
        mockEncoder.Setup(x => x.Encode(apprenticeship.Id, EncodingType.ApprenticeshipId)).Returns("XYZ");

        // Act
        var response = sut.Map(apprenticeship);

        // Assert
        response.Id.Should().Be(apprenticeship.Id);
        response.HashedId.Should().Be("XYZ");
        response.Uln.Should().Be(apprenticeship.Uln);
        response.DisplayName.Should().Be($"{apprenticeship.FirstName} {apprenticeship.LastName}");
        response.DateOfBirth.Should().Be(apprenticeship.DateOfBirth);
        response.TrainingDates.Should().Be($"{apprenticeship.StartDate.ToString("MM/yy")} to {apprenticeship.EndDate.ToString("MM/yy")}");
        response.StatusDescription.Should().Be(apprenticeship.Status);
    }

    [Test, MoqAutoData]
    public void ShouldMapFromFromApprovedApprenticeshipUlnSummaryToApprenticeshipUlnSummary(
        ApprovedApprenticeshipUlnSummary apprenticeship,
        [Frozen] Mock<IEncodingService> mockEncoder,
        ApprenticeshipUlnSummaryMapper sut
    )
    {
        mockEncoder.Setup(x => x.Encode(apprenticeship.Id, EncodingType.ApprenticeshipId)).Returns("XYZ");
        mockEncoder.Setup(x => x.Encode(apprenticeship.EmployerAccountId, EncodingType.AccountId)).Returns("ABC");

        // Act
        var response = sut.Map(apprenticeship);

        // Assert
        response.Id.Should().Be(apprenticeship.Id);
        response.HashedId.Should().Be("XYZ");
        response.HashedAccountId.Should().Be("ABC");
        response.ProviderId.Should().Be(apprenticeship.ProviderId);
        response.EmployerName.Should().Be(apprenticeship.EmployerName);
        response.DisplayName.Should().Be($"{apprenticeship.FirstName} {apprenticeship.LastName}");
        response.TrainingDates.Should().Be($"{apprenticeship.StartDate.ToString("MM/yy")} to {apprenticeship.EndDate.ToString("MM/yy")}");
        response.StatusDescription.Should().Be(apprenticeship.Status);
    }
}