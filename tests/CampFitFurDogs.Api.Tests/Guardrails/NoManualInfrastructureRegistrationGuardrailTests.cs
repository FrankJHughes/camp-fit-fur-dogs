using Frank.Testing.Contexts;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Factories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public class NoManualInfrastructureRegistrationGuardrailTests : IAsyncLifetime
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
    public void Should_Not_Have_Manual_Infrastructure_Registrations()
    {
        var factory = CreateFactory();

        var infraAssembly = typeof(CampFitFurDogs.Infrastructure.InfrastructureExtensions).Assembly;

        var infraTypes =
            DiRegistrationScanner.FindTypesWithInterfaces(
                infraAssembly,
                t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    (
                        t.Name.EndsWith("Repository") ||
                        t.Name.EndsWith("Provider") ||
                        t.Name.EndsWith("Service")
                    ));

        foreach (var (type, iface) in infraTypes)
        {
            iface.Should().NotBeNull();

            var registrations = GetAll(factory, iface!);

            registrations.Should().ContainSingle(
                $"{type.Name} must be registered exactly once via Scrutor, not manually");
        }
    }
}
