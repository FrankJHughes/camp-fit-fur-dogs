using System.Linq;
using CampFitFurDogs.Application.Abstractions;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class CurrentUserServiceGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public CurrentUserServiceGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void ShouldResolveTestCurrentUserService()
    {
        Get<ICurrentUserService>().Should().BeOfType<TestCurrentUserService>();
    }

    [Fact]
    public void ShouldBeSameInstanceAsFactoryProperty()
    {
        Get<ICurrentUserService>().Should().BeSameAs(Factory.TestUserService);
    }

    [Fact]
    public void ShouldHaveSingleEffectiveRegistration()
    {
        GetAll<ICurrentUserService>().Should().HaveCount(1);
    }

    [Fact]
    public void ShouldBehaveAsSingleton()
    {
        var s1 = Get<ICurrentUserService>();
        var s2 = Get<ICurrentUserService>();

        s1.Should().BeSameAs(s2);
    }
}
