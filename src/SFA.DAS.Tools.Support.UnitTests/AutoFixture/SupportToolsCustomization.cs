using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.App_Start;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Security.Claims;

namespace SFA.DAS.Tools.Support.UnitTests.AutoFixture
{
    public class SupportToolsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new CustomCompositeMetadataDetailsProvider());
            fixture.Register<IMapper>(CreateAutoMapperMap);
            fixture.Register<IOptions<ClaimsConfiguration>>(CreateClaimsConfiguration);
            fixture.Inject(new ViewDataDictionary(fixture.Create<DefaultModelMetadataProvider>(), fixture.Create<ModelStateDictionary>()));
            fixture.Customize<ControllerContext>(c
                => c.Without(d => d.ActionDescriptor)
                .With(e => e.HttpContext, CreateHttpContext));
            fixture.Customize<StopApprenticeshipRow>(c =>
                c.With(p => p.ApiSubmissionStatus, StopApprenticeshipRow.SubmissionStatus.NotSent)
                .With(p => p.ApiErrorMessage, string.Empty));
            fixture.Customize<StopApprenticeshipViewModel>(c => c.With(d => d.HasError, false));
            fixture.Customize<ResultBase>(c => c.With(d => d.ErrorMessage, string.Empty));
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
            config.Setup(s => s.Value).Returns(new ClaimsConfiguration
            {
                EmailClaim = "emailClaim",
                NameIdentifierClaim = "nameIdentifierClaim",
                NameClaim = "nameClaim"
            });
            return config.Object;
        }

        private static IMapper CreateAutoMapperMap()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = new Mapper(configuration);
            return mapper;
        }
    }
}
