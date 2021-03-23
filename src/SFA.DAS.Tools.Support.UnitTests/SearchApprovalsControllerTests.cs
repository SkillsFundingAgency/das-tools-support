using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models;
using Xunit;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class SearchApprovalsControllerTests
    {
        private const string StopAction = "stop";

        [Theory, AutoMoqData]
        public void SearchApprenticeships_GET_WithNoParameters_ReturnsView(SearchApprovalsController sut)
        {
            //When
            var result = sut.SearchApprenticeships(null, null, null, null, null, null, null, null, StopAction);

            //Then
            result.Should().BeOfType<ViewResult>().
                Which.Model.Should().BeOfType<SearchApprenticeshipsViewModel>().
                Which.Should().BeEquivalentTo(new SearchApprenticeshipsViewModel());
        }
    }
}