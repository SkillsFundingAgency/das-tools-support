using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using AutoFixture.NUnit3;

namespace SFA.DAS.Tools.Support.UnitTests;

public class CommitmentsDataControllerTests
{
    [Test, DomainAutoData]
    public async Task SearchApprenticeships_POST_EmptyModel_ReturnsError(CommitmentsDataController sut)
    {
        //Given
        var model = new SearchApprenticeshipsViewModel();

        //When
        var result = await sut.SearchApprenticeships(model);

        //Then
        result.Should().BeOfType<JsonResult>().Which
            .Value.Should().BeEquivalentTo(new
            {
                ErrorTitle = "Invalid Search",
                ErrorMessage = "At least one parameter must be populated"
            });
    }

    [Test, DomainAutoData]
    public async Task SearchApprenticeships_POST_ApiCallErrors_ReturnsError([Frozen] Mock<IEmployerCommitmentsService> api, SearchApprenticeshipsViewModel model, CommitmentsDataController sut)
    {
        //Given
        var apiResult = new SearchApprenticeshipsResult
        {
            ErrorMessage = "Api Error Message"
        };

        api.Setup(s => s.SearchApprenticeships(
                It.Is<SearchApprenticeshipsRequest>(s => s.CourseName == model.CourseName && s.EmployerName == model.EmployerName
                                                                                          && s.ProviderName == model.ProviderName && s.Ukprn == model.Ukprn && s.StartDate == model.StartDate && s.EndDate == model.EndDate && s.SearchTerm == model.ApprenticeNameOrUln
                                                                                          && s.ApprenticeshipStatus == model.SelectedStatus),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(apiResult));

        //When
        var result = await sut.SearchApprenticeships(model);

        //Then
        result.Should().BeOfType<JsonResult>().Which
            .Value.Should().BeEquivalentTo(new
            {
                ErrorTitle = "Call to Commitments Api Failed",
                ErrorMessage = apiResult.ErrorMessage
            });
    }

    [Test, DomainAutoData]
    public async Task SearchApprenticeships_POST_ApiCallSucceeds_ReturnsResult([Frozen] Mock<IEmployerCommitmentsService> api, SearchApprenticeshipsViewModel model, SearchApprenticeshipsResult apiResult, CommitmentsDataController sut)
    {
        //Given
        apiResult.ErrorMessage = "";
        api.Setup(s => s.SearchApprenticeships(
                It.Is<SearchApprenticeshipsRequest>(s => s.CourseName == model.CourseName && s.EmployerName == model.EmployerName
                                                                                          && s.ProviderName == model.ProviderName && s.StartDate == model.StartDate && s.EndDate == model.EndDate && s.SearchTerm == model.ApprenticeNameOrUln
                                                                                          && s.ApprenticeshipStatus == model.SelectedStatus),
                It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(apiResult));

        //When
        var result = await sut.SearchApprenticeships(model);

        //Then
        result.Should().BeOfType<JsonResult>().Which
            .Value.Should().BeEquivalentTo(apiResult.Apprenticeships.Select(apprenticeshipDto => new
            {
                apprenticeshipDto.Id,
                apprenticeshipDto.FirstName,
                apprenticeshipDto.LastName,
                apprenticeshipDto.EmployerName,
                apprenticeshipDto.ProviderName,
                apprenticeshipDto.CourseName,
                apprenticeshipDto.StartDate,
                apprenticeshipDto.EndDate,
                Status = apprenticeshipDto.ApprenticeshipStatus.ToString(),
                PaymentStatus = apprenticeshipDto.PaymentStatus.ToString()
            }));
    }
}