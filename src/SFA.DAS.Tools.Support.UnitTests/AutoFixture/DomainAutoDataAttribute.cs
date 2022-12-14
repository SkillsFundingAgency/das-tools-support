using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace SFA.DAS.Tools.Support.UnitTests.AutoFixture
{
    public class DomainAutoDataAttribute : AutoDataAttribute
    {
        public DomainAutoDataAttribute()
            : base(() =>
            {
                var fixture = new Fixture();
                fixture.Customize(new AutoMoqCustomization());
                fixture.Customize(new SupportToolsCustomization())
                .Behaviors.Add(new OmitOnRecursionBehavior());
                return fixture;
            })
        { }

    }
}
