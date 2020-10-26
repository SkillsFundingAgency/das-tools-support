using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace SFA.DAS.Tools.Support.UnitTests.AutoFixture
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
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
