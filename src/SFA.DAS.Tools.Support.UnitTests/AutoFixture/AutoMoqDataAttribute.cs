using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using AutoMapper;
using SFA.DAS.Tools.Support.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Tools.Support.UnitTests.AutoFixture
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() =>
            {
                var fixture = new Fixture();
                fixture.Customize(new AutoMoqCustomization());
                fixture.Register<IMapper>(CreateAutoMapperMap);
                return fixture;
            })
        { }


        private static IMapper CreateAutoMapperMap()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            var mapper = new Mapper(configuration);
            return mapper;
        }
    }
}
