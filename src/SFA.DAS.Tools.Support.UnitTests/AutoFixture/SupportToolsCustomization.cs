using System.Collections.Generic;
using System.Security.Claims;
using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Mapping;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.UnitTests.AutoFixture;

public class SupportToolsCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => new CustomCompositeMetadataDetailsProvider());
        fixture.Register(CreateAutoMapperMap);
        fixture.Register(CreateClaimsConfiguration);
        fixture.Inject(new ViewDataDictionary(fixture.Create<DefaultModelMetadataProvider>(), fixture.Create<ModelStateDictionary>()));
        fixture.Customize<ControllerContext>(c
            => c.Without(d => d.ActionDescriptor)
                .With(e => e.HttpContext, CreateHttpContext));
        fixture.Customize<StopApprenticeshipRow>(c =>
            c.With(p => p.ApiSubmissionStatus, SubmissionStatus.NotSent)
                .With(p => p.ApiErrorMessage, string.Empty));
        fixture.Customize<PauseApprenticeshipRow>(c =>
            c.With(p => p.ApiSubmissionStatus, SubmissionStatus.NotSent)
                .With(p => p.ApiErrorMessage, string.Empty));
        fixture.Customize<ResumeApprenticeshipRow>(c =>
            c.With(p => p.ApiSubmissionStatus, SubmissionStatus.NotSent)
                .With(p => p.ApiErrorMessage, string.Empty));
        fixture.Customize<StopApprenticeshipViewModel>(c => c.With(d => d.HasError, false));
        fixture.Customize<PauseApprenticeshipViewModel>(c => c.With(d => d.HasError, false));
        fixture.Customize<ResumeApprenticeshipViewModel>(c => c.With(d => d.HasError, false));
        fixture.Customize<SuspendUsersViewModel>(c => c.With(d => d.HasError, false));
        fixture.Customize<ResumeUsersViewModel>(c => c.With(d => d.HasError, false));
        fixture.Customize<ResultBase>(c => c.With(d => d.ErrorMessage, string.Empty));

        fixture.Freeze<Mock<IEmployerUsersApiClient>>();
        fixture.Freeze<Mock<IToolsSupportApimService>>();

        fixture.Customize<EmployerUsers.Api.Types.UserViewModel>(c => c
            .With(d => d.IsSuspended, false)
            .With(d => d.IsLocked, false)
            .With(d => d.IsActive, true));

        fixture.Freeze<Mock<IAccountApiClient>>()
            .Setup(x => x.GetAccountUsers(12345))
            .ReturnsAsync(() => new List<TeamMemberViewModel>()
            {
                new TeamMemberViewModel
                {
                    Name = "Test",
                    Email = "t@est.com",
                    Role = "emperor",
                    Status = EAS.Account.Api.Types.InvitationStatus.Accepted,
                    UserRef = "123",
                    CanReceiveNotifications = true
                }
            });
    }

    private HttpContext CreateHttpContext()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim("emailClaim","NameOfUser@email.com"),
            new Claim("nameIdentifierClaim","123456"),
            new Claim("nameClaim","NameOfUser")
        }, "mock"));

        return new DefaultHttpContext() { User = user };
    }

    private class CustomCompositeMetadataDetailsProvider : ICompositeMetadataDetailsProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
            throw new System.NotImplementedException();
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            throw new System.NotImplementedException();
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            throw new System.NotImplementedException();
        }
    }


    private static IOptions<ClaimsConfiguration> CreateClaimsConfiguration()
    {
        var fixture = new Fixture();
        var config = new Mock<IOptions<ClaimsConfiguration>>();
        config.Setup(s => s.Value)
            .Returns(new ClaimsConfiguration("nameClaim", "emailClaim", "nameIdentifierClaim"));
        return config.Object;
    }

    private static IMapper CreateAutoMapperMap()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = new Mapper(configuration);
        return mapper;
    }
}