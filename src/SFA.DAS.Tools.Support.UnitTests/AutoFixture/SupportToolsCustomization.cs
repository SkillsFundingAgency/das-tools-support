using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Tools.Support.Web.App_Start;
using SFA.DAS.Tools.Support.Web.Configuration;

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
            fixture.Customize<ControllerContext>(c => c.Without(d => d.ActionDescriptor));
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
                EmailClaim = fixture.Create<string>(),
                NameIdentifierClaim = fixture.Create<string>(),
                NameClaim = fixture.Create<string>()
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
