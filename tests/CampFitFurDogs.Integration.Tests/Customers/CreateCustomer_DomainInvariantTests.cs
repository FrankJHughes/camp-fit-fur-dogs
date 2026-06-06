using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API Collection")]
public class CreateCustomer_DomainInvariantTests : IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;
    private readonly PostgresFixture _db;

    public CreateCustomer_DomainInvariantTests(ApiFactoryFixture factoryFixture, PostgresFixture postgresFixture)
    {
        _db = postgresFixture;

        _factory = factoryFixture.Factory;
        _factory.UseContainer(postgresFixture.Container);

        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

}
