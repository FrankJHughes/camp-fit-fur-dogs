using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.Infrastructure.Data;

namespace CampFitFurDogs.Api.Tests.Fixtures;

[Collection("api")]
public abstract class ApiTestBase : IAsyncLifetime
{
    protected readonly CampFitFurDogsApiFactory Factory;
    protected readonly PostgresFixture Fixture;
    protected HttpClient Client;

    protected ApiTestBase(CampFitFurDogsApiFactory factory, PostgresFixture fixture)
    {
        Fixture = fixture;
        Factory = factory;
        Factory.UseContainer(fixture.Container);
        Client = Factory.CreateClient();
    }

    public virtual async Task InitializeAsync()
    {

        using var scope = CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tableNames = db.Model.GetEntityTypes()
            .Select(e => e.GetTableName())
            .Where(t => t is not null)
            .Distinct()
            .Select(t => $"\"{t}\"");

        var tableList = string.Join(", ", tableNames);

        if (!string.IsNullOrEmpty(tableList))
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
            await db.Database.ExecuteSqlRawAsync(
                $"TRUNCATE TABLE {tableList} CASCADE;");
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

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
