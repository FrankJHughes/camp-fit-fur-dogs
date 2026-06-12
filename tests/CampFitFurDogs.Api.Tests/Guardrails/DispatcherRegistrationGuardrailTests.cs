using CampFitFurDogs.Api.Tests.Fixtures;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;
using FluentAssertions;
using Frank.Abstractions;
using Frank.Abstractions.Events;
using Frank.Events;

namespace CampFitFurDogs.Api.Tests.Guardrails;

[Collection("API With Postgres")]
public class DispatcherRegistrationGuardrailTests
    : ApiWithPostgresTestBase
{
    public DispatcherRegistrationGuardrailTests(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
        : base(factory, fixture) { }

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
