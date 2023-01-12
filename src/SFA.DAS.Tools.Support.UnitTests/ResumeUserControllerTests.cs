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

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class ResumeUserControllerTests
    {
        private const string ResumeAction = "resume";

        [Test, DomainAutoData]
        public void ResumeUsers_POST_NoRowsSelected_RedirectsToSearch(ResumeUserController sut, UserSearchResultsViewModel model)
        {
            //Given
            model.SelectedIds = null;            

            //When
            var result = sut.ResumeUsers(model);

            //Then
            var action = result.Should().BeOfType<RedirectToActionResult>().Which;
            action.ActionName.Should().Be("Index");
            action.ControllerName.Should().Be("SearchUser");
            action.RouteValues.Values.Should().BeEquivalentTo(new object []
            {
                    model.HashedAccountId,
                    model.InternalAccountId,
                    ResumeAction
                });
        }

        [Test, DomainAutoData]
        public void CancelResumeUsers_POST_RedirectsToSearch(ResumeUserController sut, ResumeUsersViewModel model)
        {
            //Given

            //When
            var result = sut.CancelResumeUsers(model, ResumeAction);

            //Then
            var action = result.Should().BeOfType<RedirectToActionResult>().Which;
            action.ActionName.Should().Be("Index");
            action.ControllerName.Should().Be("SearchUser");
            action.RouteValues.Values.Should().BeEquivalentTo(new object []
            {
                    model.HashedAccountId,
                    model.InternalAccountId,
                    ResumeAction
                });
        }

        [Test, DomainAutoData]
        public async Task ResumeUsersConfirmation_POST_JsonDataError_ReturnsErrorViewModel(ResumeUserController sut, ResumeUsersViewModel model)
        {
            //Given
            model.UserData = "RandomData";

            //When
            var result = await sut.ResumeUsersConfirmation(model);

            //Then
            var resultModel = result.Should().BeOfType<ViewResult>().Which
                .Model.Should().BeOfType<ResumeUsersViewModel>().Which;
            resultModel.UserData.Should().BeSameAs(null);
            resultModel.HasError.Should().BeTrue();
            sut.ModelState.IsValid.Should().BeFalse();
            sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to read user information, please return to the search and try again");
        }

        [Test, DomainAutoData]
        public async Task ResumConfirmation_POST_IdentityError_ReturnsErrorViewModel(ResumeUserController sut, ResumeUsersViewModel model, List<AccountUserRow> apprenticeshipData)
        {
            //Given
            var jsonData = JsonSerializer.Serialize(apprenticeshipData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).ToString();
            model.UserData = jsonData;
            sut.ControllerContext.HttpContext = new DefaultHttpContext();

            //When
            var result = await sut.ResumeUsersConfirmation(model);

            //Then
            var resultModel = result.Should().BeOfType<ViewResult>().Which
                .Model.Should().BeOfType<ResumeUsersViewModel>().Which;
            resultModel.UserData.Should().BeSameAs(model.UserData);
            sut.ModelState.IsValid.Should().BeFalse();
            sut.ModelState.Values.First().Errors.First().ErrorMessage.Should().Be("Unable to retrieve userId or name from claim for request to Resume User");
        }

        [Test, DomainAutoData]
        public async Task ResumeUsersConfirmation_POST_DataEnteredCorrectly_SubmitsToApiAndFails([Frozen] Mock<IEmployerUsersService> api, ResumeUserController sut, ResumeUsersViewModel model, List<AccountUserRow> userData)
        {
            //Given
            var userAccountIds = userData.Select(u => u.UserRef);
            model.UserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).ToString();

            api.Setup(s => s.ResumeUser(
                It.Is<ResumeUserRequest>(r => userAccountIds.Contains(r.UserId)), It.IsAny<CancellationToken>()))
                .Returns((ResumeUserRequest request, CancellationToken token) =>
                {
                    return Task.FromResult(new ResumeUserResult
                    {
                        UserId = request.UserId,
                        ErrorMessage = $"Errored For {request.UserId}"
                    }); ;
                });

            //When
            var result = await sut.ResumeUsersConfirmation(model);

            //Then

            var resultModel = result.Should().BeOfType<ViewResult>().Which
                .Model.Should().BeOfType<ResumeUsersViewModel>().Which;
            resultModel.Users.All(s => s.ApiSubmissionStatus == SubmissionStatus.Errored && s.ApiErrorMessage.Equals($"Errored For {s.UserRef}"));
            resultModel.HasError.Should().BeFalse();
        }

        [Test, DomainAutoData]
        public async Task ResumeUsersConfirmation_POST_DataEnteredCorrectly_SubmitsToApiAndSucceeds([Frozen] Mock<IEmployerUsersService> api, ResumeUserController sut, ResumeUsersViewModel model, List<AccountUserRow> userData)
        {
            //Given
            var userAccountIds = userData.Select(u => u.UserRef);
            model.UserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).ToString();

            api.Setup(s => s.ResumeUser(
                It.Is<ResumeUserRequest>(r => userAccountIds.Contains(r.UserId)), It.IsAny<CancellationToken>()))
                .Returns((ResumeUserRequest request, CancellationToken token) =>
                {
                    return Task.FromResult(new ResumeUserResult
                    {
                        UserId = request.UserId
                    });
                });

            //When
            var result = await sut.ResumeUsersConfirmation(model);

            //Then

            var resultModel = result.Should().BeOfType<ViewResult>().Which
                .Model.Should().BeOfType<ResumeUsersViewModel>().Which;
            resultModel.Users.All(s => s.ApiSubmissionStatus == SubmissionStatus.Successful);
            resultModel.HasError.Should().BeFalse();
        }
    }
}