using System.Linq;
using SharedKernel.Abstractions;
using FluentAssertions;

using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class CurrentUserServiceGuardrailTests
    : ApiTestBase
{
    public CurrentUserServiceGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

    [Fact]
    public void ShouldResolveTestCurrentUserService()
    {
        Get<ICurrentUserService>().Should().BeOfType<TestCurrentUser>();
    }

    [Fact]
    public void ShouldBeSameInstanceAsFactoryProperty()
    {
        Get<ICurrentUserService>().Should().BeSameAs(Factory.TestUser);
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
