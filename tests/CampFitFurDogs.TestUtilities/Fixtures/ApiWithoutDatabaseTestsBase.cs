using CampFitFurDogs.TestUtilities.Builders;
using CampFitFurDogs.TestUtilities.Factories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.TestUtilities.Fixtures;

public abstract class ApiWithoutDatabaseTestBase
{
    protected CampFitFurDogsApiFactory Factory { get; }

    protected ApiWithoutDatabaseTestBase(CampFitFurDogsApiFactory factory)
    {
        //
        // Never mutate the shared factory instance.
        // Always clone it so each test class gets a fully isolated environment.
        //
        Factory = factory;
    }

    // ------------------------------------------------------------
    // CLIENT CREATION (CookieAuthOnly)
    // ------------------------------------------------------------
    protected HttpClient CreateClient() =>
        new TestClientBuilder(
            Factory
                .Clone()
                .WithDefaultApiConfig()
                .WithCookieAuthOnly())
        .BuildClient();

    protected virtual HttpClient CreateClientWithOverrides(
        Action<IConfigurationBuilder>? overrides = null,
        WebApplicationFactoryClientOptions? options = null)
    {
        var factory = Factory.Clone();

        if (overrides is not null)
            factory.WithConfigOverrides(overrides);

        factory.WithoutDatabase();

        return factory.CreateClient(options ?? new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // ------------------------------------------------------------
    // SERVICE ACCESS HELPERS
    // ------------------------------------------------------------
    protected IServiceScope CreateScope()
        => Factory.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

    protected T Get<T>() where T : notnull
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    protected async Task WithScope(Func<IServiceProvider, Task> action)
    {
        using var scope = CreateScope();
        await action(scope.ServiceProvider);
    }

    protected IEnumerable<T> GetAll<T>() where T : notnull
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetServices<T>().ToList();
    }

    protected IEnumerable<object> GetAll(Type serviceType)
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetServices(serviceType).Cast<object>().ToList();
    }
}
