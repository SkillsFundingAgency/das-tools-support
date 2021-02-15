using AutoFixture;
using AutoFixture.Xunit2;
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
using Xunit;
using static SFA.DAS.Tools.Support.Web.Models.StopApprenticeshipRow;

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