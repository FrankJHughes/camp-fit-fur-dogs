using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[Collection("api")]
public abstract class ApiTestBase
{
    protected readonly CampFitFurDogsApiFactory Factory;
    protected readonly PostgresFixture Fixture;
    protected readonly HttpClient Client;

    protected ApiTestBase(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
    {
        Fixture = fixture;
        Factory = factory;
        Factory.UseContainer(fixture.Container);

        Client = Factory.CreateClient();
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
        return scope.ServiceProvider.GetServices<T>().ToList();
    }

    protected IEnumerable<object> GetAll(Type serviceType)
    {
        using var scope = CreateScope();
        return scope.ServiceProvider.GetServices(serviceType).Cast<object>().ToList();
    }
}
