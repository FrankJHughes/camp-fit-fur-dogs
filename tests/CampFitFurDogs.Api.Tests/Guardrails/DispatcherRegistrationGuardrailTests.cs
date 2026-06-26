using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Frank.Abstractions.Events;

using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using Frank.Abstractions.Command;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class DispatcherRegistrationGuardrailTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres = default!;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_postgres is not null)
            await _postgres.DisposeAsync();
    }

    private ApiFactory CreateFactory()
    {
        var ctx = new ApiContext()
            .WithDatabase(true, _postgres)
            .WithCookieAuthOnly(false);

        return new ApiFactory(ctx);
    }

    private T Get<T>(ApiFactory factory)
        where T : notnull
    {
        using var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    private IEnumerable<T> GetAll<T>(ApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetServices<T>();
    }

    // ------------------------------------------------------------
    // COMMAND DISPATCHER
    // ------------------------------------------------------------
    [Fact]
    public void CommandDispatcher_ShouldBeRegistered()
    {
        var factory = CreateFactory();
        Get<ICommandDispatcher>(factory).Should().NotBeNull();
    }

    [Fact]
    public void CommandDispatcher_ShouldHaveSingleRegistration()
    {
        var factory = CreateFactory();
        GetAll<ICommandDispatcher>(factory).Should().HaveCount(1);
    }

    // ------------------------------------------------------------
    // DOMAIN EVENT DISPATCHER
    // ------------------------------------------------------------
    [Fact]
    public void DomainEventDispatcher_ShouldBeRegistered()
    {
        var factory = CreateFactory();
        Get<IEventDispatcher>(factory).Should().NotBeNull();
    }

    [Fact]
    public void DomainEventDispatcher_ShouldHaveSingleRegistration()
    {
        var factory = CreateFactory();
        GetAll<IEventDispatcher>(factory).Should().HaveCount(1);
    }
}
