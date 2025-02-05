using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests;

[TestFixture]
public class EmployerSupportControllerTests : IDisposable
{
    private Mock<IMediator> _mediatorMock;
    private Fixture _fixture;
    private EmployerSupportController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _fixture = new Fixture();
        _controller = new EmployerSupportController(_mediatorMock.Object);
    }

    [Test]
    public async Task AccountDetails_ShouldReturnViewWithViewModel_WhenCalled()
    {
        // Arrange
        var accountHashedId = _fixture.Create<string>();
        var accountFieldsSelection = _fixture.Create<AccountFieldsSelection>();
        var query = new GetAccountDetailsQuery
        {
            AccountHashedId = accountHashedId,
            AccountFieldsSelection = accountFieldsSelection
        };
        var result = _fixture.Create<GetAccountDetailsQueryResult>();
        var viewModel = AccountDetailsViewModel.MapFrom(result);
        viewModel.SelectedTab = accountFieldsSelection;

        _mediatorMock.Setup(m => m.Send(It.Is<GetAccountDetailsQuery>(q => q.AccountHashedId == accountHashedId && q.AccountFieldsSelection == accountFieldsSelection), default))
                     .ReturnsAsync(result);

        // Act
        var response = await _controller.AccountDetails(accountHashedId, accountFieldsSelection) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(viewModel);
    }

    [Test]
    public async Task AccountDetails_ShouldCallMediatorWithCorrectQuery()
    {
        // Arrange
        var accountHashedId = _fixture.Create<string>();
        var accountFieldsSelection = _fixture.Create<AccountFieldsSelection>();
        var query = new GetAccountDetailsQuery
        {
            AccountHashedId = accountHashedId,
            AccountFieldsSelection = accountFieldsSelection
        };
        var result = _fixture.Create<GetAccountDetailsQueryResult>();

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAccountDetailsQuery>(), default))
                     .ReturnsAsync(result);

        // Act
        await _controller.AccountDetails(accountHashedId, accountFieldsSelection);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetAccountDetailsQuery>(q => q.AccountHashedId == accountHashedId && q.AccountFieldsSelection == accountFieldsSelection), default), Times.Once);
    }

    public void Dispose()
    {
        _controller.Dispose();
        GC.SuppressFinalize(this);
    }
}
