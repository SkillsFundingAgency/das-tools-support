using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using NUnit.Framework;

namespace SFA.DAS.Tools.Support.UnitTests;

public class SuspendUserControllerTests
{
    private const string SuspendAction = "suspend";

    [Test, DomainAutoData]
    public void SuspendUsers_POST_NoRowsSelected_RedirectsToSearch(SuspendUserController sut, UserSearchResultsViewModel model)
    {
        //Given
        model.SelectedIds = null;            

        //When
        var result = sut.SuspendUsers(model);

        //Then
        var action = result.Should().BeOfType<RedirectToActionResult>().Which;
        action.ActionName.Should().Be("Index");
        action.ControllerName.Should().Be("SearchUser");
        action.RouteValues.Values.Should().BeEquivalentTo(new object []
        {
            model.HashedAccountId,
            model.InternalAccountId,
            SuspendAction
        });
    }

    [Test, DomainAutoData]
    public void CancelSuspendUsers_POST_RedirectsToSearch(SuspendUserController sut, SuspendUsersViewModel model)
    {
        //Given

        //When
        var result = sut.CancelSuspendUsers(model, SuspendAction);

        //Then
        var action = result.Should().BeOfType<RedirectToActionResult>().Which;
        action.ActionName.Should().Be("Index");
        action.ControllerName.Should().Be("SearchUser");
        action.RouteValues.Values.Should().BeEquivalentTo(new object []
        {
            model.HashedAccountId,
            model.InternalAccountId,
            SuspendAction
        });
    }

    [Test, DomainAutoData]
    public async Task SuspendUsersConfirmation_POST_JsonDataError_ReturnsErrorViewModel(SuspendUserController sut, SuspendUsersViewModel model)
    {
        //Given
        model.UserData = "RandomData";

        //When
        var result = await sut.SuspendUsersConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<SuspendUsersViewModel>().Which;
        resultModel.UserData.Should().BeSameAs(null);
        resultModel.HasError.Should().BeTrue();
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to read user information, please return to the search and try again");
    }

    [Test, DomainAutoData]
    public async Task SuspendConfirmation_POST_IdentityError_ReturnsErrorViewModel(SuspendUserController sut, SuspendUsersViewModel model, List<AccountUserRow> apprenticeshipData)
    {
        //Given
        var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        model.UserData = jsonData;
        sut.ControllerContext.HttpContext = new DefaultHttpContext();

        //When
        var result = await sut.SuspendUsersConfirmation(model);

        //Then
        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<SuspendUsersViewModel>().Which;
        resultModel.UserData.Should().BeSameAs(model.UserData);
        sut.ModelState.IsValid.Should().BeFalse();
        sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to retrieve userId or name from claim for request to Suspend User");
    }

    [Test, DomainAutoData]
    public async Task SuspendUsersConfirmation_POST_DataEnteredCorrectly_SubmitsToApiAndFails([Frozen] Mock<IEmployerUsersService> api, SuspendUserController sut, SuspendUsersViewModel model, List<AccountUserRow> userData)
    {
        //Given
        var userAccountIds = userData.Select(u => u.UserRef);
        model.UserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        api.Setup(s => s.SuspendUser(
                It.Is<SuspendUserRequest>(r => userAccountIds.Contains(r.UserId)), It.IsAny<CancellationToken>()))
            .Returns((SuspendUserRequest request, CancellationToken token) => Task.FromResult(new SuspendUserResult
            {
                UserId = request.UserId,
                ErrorMessage = $"Errored For {request.UserId}"
            }));

        //When
        var result = await sut.SuspendUsersConfirmation(model);

        //Then

        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<SuspendUsersViewModel>().Which;
        resultModel.Users.All(s => s.ApiSubmissionStatus == SubmissionStatus.Errored && s.ApiErrorMessage.Equals($"Errored For {s.UserRef}"));
        resultModel.HasError.Should().BeFalse();
    }

    [Test, DomainAutoData]
    public async Task SuspendUsersConfirmation_POST_DataEnteredCorrectly_SubmitsToApiAndSucceeds([Frozen] Mock<IEmployerUsersService> api, SuspendUserController sut, SuspendUsersViewModel model, List<AccountUserRow> userData)
    {
        //Given
        var userAccountIds = userData.Select(u => u.UserRef);
        model.UserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        api.Setup(s => s.SuspendUser(
                It.Is<SuspendUserRequest>(r => userAccountIds.Contains(r.UserId)), It.IsAny<CancellationToken>()))
            .Returns((SuspendUserRequest request, CancellationToken token) => Task.FromResult(new SuspendUserResult
            {
                UserId = request.UserId
            }));

        //When
        var result = await sut.SuspendUsersConfirmation(model);

        //Then

        var resultModel = result.Should().BeOfType<ViewResult>().Which
            .Model.Should().BeOfType<SuspendUsersViewModel>().Which;
        resultModel.Users.All(s => s.ApiSubmissionStatus == SubmissionStatus.Successful);
        resultModel.HasError.Should().BeFalse();
    }
}