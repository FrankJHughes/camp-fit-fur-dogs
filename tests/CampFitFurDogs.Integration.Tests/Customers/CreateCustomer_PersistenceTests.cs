using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API Collection")]
public class CreateCustomer_PersistenceTests
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;

    public CreateCustomer_PersistenceTests(ApiFactoryFixture factoryFixture, PostgresFixture postgresFixture)
    {
        _factory = factoryFixture.Factory;
        _factory.UseContainer(postgresFixture.Container);

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_PersistsCustomerInDatabase()
    {
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = $"persist-test-{Guid.NewGuid()}@example.com",
            Phone = "916-555-1234",
            Password = "SuperSecure123!"
        };

        //
        // 1. Send request
        //
        var response = await _client.PostAsJsonAsync("/api/customers", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        //
        // 2. Extract ID from Location header
        //
        var id = response.Headers.Location!.ToString().Split('/').Last();
        var customerId = CustomerId.From(Guid.Parse(id));

        //
        // 3. Load persisted entity
        //
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customer = await db.Set<Customer>().FindAsync(customerId);

        //
        // 4. Assertions
        //
        customer.Should().NotBeNull();

        customer!.Id.Value.Should().Be(customerId.Value);
        customer.Email.Value.Should().Be(request.Email.ToLowerInvariant());
        customer.FirstName.Value.Should().Be("Frank");
        customer.LastName.Value.Should().Be("Hughes");
        customer.Phone!.Value.Should().Be(PhoneNumber.From("916-555-1234").Value);

        // Password should be hashed (bcrypt)
        customer.PasswordHash!.Value.Should().NotBe(request.Password);
        customer.PasswordHash.Value.Should().MatchRegex(@"^\$2[aby]\$");
    }
}
