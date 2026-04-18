using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Guardrails;

public abstract class GuardrailTestBase
{
    protected readonly CampFitFurDogsApiFactory Factory;

    protected GuardrailTestBase(CampFitFurDogsApiFactory factory)
    {
        Factory = factory;
    }

    protected IServiceScope CreateScope()
        => Factory.Services
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

    protected T Get<T>() where T : notnull
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    protected IEnumerable<T> GetAll<T>() where T : notnull
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetServices<T>();
    }

    protected IEnumerable<object> GetAll(Type serviceType)
    {
        using var scope = Factory.Services.CreateScope();
        var provider = scope.ServiceProvider;

        return provider.GetServices(serviceType).Cast<object>();
    }

}
