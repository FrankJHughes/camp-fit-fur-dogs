using FluentAssertions;
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.DomainEvents;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DispatcherRegistrationGuardrailTests
    : GuardrailTestBase, IClassFixture<CampFitFurDogsApiFactory>
{
    public DispatcherRegistrationGuardrailTests(CampFitFurDogsApiFactory factory)
        : base(factory) { }

    [Fact]
    public void CommandDispatcher_ShouldBeRegistered() =>
        Get<ICommandDispatcher>().Should().NotBeNull();

    [Fact]
    public void CommandDispatcher_ShouldHaveSingleRegistration() =>
        GetAll<ICommandDispatcher>().Should().HaveCount(1);

    [Fact]
    public void DomainEventDispatcher_ShouldBeRegistered() =>
        Get<IDomainEventDispatcher>().Should().NotBeNull();

    [Fact]
    public void DomainEventDispatcher_ShouldHaveSingleRegistration() =>
        GetAll<IDomainEventDispatcher>().Should().HaveCount(1);
}