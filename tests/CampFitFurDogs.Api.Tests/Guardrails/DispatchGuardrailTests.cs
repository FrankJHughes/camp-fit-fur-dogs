using System.Linq;
using CampFitFurDogs.Application.Abstractions;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DispatchGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public DispatchGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void ShouldBeRegistered()
    {
        Get<ICommandDispatcher>().Should().NotBeNull();
    }

    [Fact]
    public void ShouldHaveSingleEffectiveRegistration()
    {
        GetAll<ICommandDispatcher>().Should().HaveCount(1);
    }
}
