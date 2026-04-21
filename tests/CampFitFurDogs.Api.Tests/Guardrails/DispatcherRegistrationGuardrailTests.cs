using FluentAssertions;
using SharedKernel.Abstractions;
using SharedKernel.Events;

using CampFitFurDogs.Api.Tests.Fixtures;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DispatcherRegistrationGuardrailTests
    : ApiTestBase
{
    public DispatcherRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture){ }

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
