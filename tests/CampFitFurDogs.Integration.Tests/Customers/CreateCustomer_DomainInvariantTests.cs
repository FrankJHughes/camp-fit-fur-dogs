using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using CampFitFurDogs.Integration.Tests.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_DomainInvariantTests : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;
    private readonly PostgresFixture _db;

    public CreateCustomer_DomainInvariantTests(PostgresFixture db)
    {
        _db = db;

        _factory = new CampFitFurDogsApiFactory();
        _factory.UseContainer(db.Container);

        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

}
