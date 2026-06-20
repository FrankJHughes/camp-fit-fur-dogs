using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Frank.Testing.Contexts;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoManualHandlerRegistrationGuardrailTests : IAsyncLifetime
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
            .WithCookieAuthOnly(false)
            .WithConfigOverride(cfg =>
                cfg.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Frontend:BaseUrl"] = "http://localhost:5173"
                    }
                )
            );

        return new ApiFactory(ctx);
    }

    private IEnumerable<object> GetAll(ApiFactory factory, Type iface)
    {
        using var scope = factory.Services.CreateScope();
        return scope.ServiceProvider.GetServices(iface).Cast<object>();
    }

    [Fact]
    public void Should_Not_Have_Manual_Handler_Registrations()
    {
        var factory = CreateFactory();

        var appAssembly = typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly;

        var handlers =
            DiRegistrationScanner.FindTypesWithInterfaces(
                appAssembly,
                t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        (
                            i.GetGenericTypeDefinition().Name.StartsWith("ICommandHandler") ||
                            i.GetGenericTypeDefinition().Name.StartsWith("IQueryHandler") ||
                            i.GetGenericTypeDefinition().Name.StartsWith("IDomainEventHandler")
                        )));

        foreach (var (type, iface) in handlers)
        {
            iface.Should().NotBeNull();

            var registrations = GetAll(factory, iface!);

            registrations.Should().ContainSingle(
                $"{type.Name} must be registered exactly once via Scrutor, not manually");
        }
    }
}
