using FluentAssertions;
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.DomainEvents;
using CampFitFurDogs.SharedKernel;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DomainEventGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public DomainEventGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void DomainEventDispatcher_ShouldBeRegistered()
    {
        Get<IDomainEventDispatcher>().Should().NotBeNull();
    }

    [Fact]
    public void DomainEventDispatcher_ShouldHaveSingleEffectiveRegistration()
    {
        GetAll<IDomainEventDispatcher>().Should().HaveCount(1);
    }
}
