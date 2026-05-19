using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CampFitFurDogs.Api;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Integration.Tests.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

public class CreateCustomer_SuccessTests : IClassFixture<PostgresFixture>, IDisposable
{
    private readonly CampFitFurDogsApiFactory _factory;
    private readonly HttpClient _client;
    private readonly PostgresFixture _db;

    public CreateCustomer_SuccessTests(PostgresFixture db)
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

    [Fact]
    public async Task CreateCustomer_SuccessfullyPersistsCustomer_AndReturns201()
    {
        // Arrange
        var email = $"success-{Guid.NewGuid()}@Example.COM"; // test normalization
        var request = new
        {
            FirstName = "Frank",
            LastName = "Hughes",
            Email = email,

            // FIXED: must be a valid phone number, still tests trimming
            Phone = " 916-555-1234 ",

            Password = "SuperSecure123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers", request);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();
        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBe(Guid.Empty);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should().Be($"/api/customers/{body.CustomerId}");

        // Assert — Database
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customer = await db.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Id == CustomerId.From(body.CustomerId));

        customer.Should().NotBeNull();

        // Domain-level assertions
        customer!.FirstName.Value.Should().Be("Frank");
        customer.LastName.Value.Should().Be("Hughes");

        // Email normalized
        customer.Email.Value.Should().Be(email.ToLowerInvariant());

        // Phone normalized (10 digits → +1XXXXXXXXXX)
        customer.Phone.Value.Should().Be("+19165551234");

        // Password hashed (not plaintext)
        customer.PasswordHash.Value.Should().NotBe(request.Password);
        customer.PasswordHash.Verify(request.Password).Should().BeTrue();
    }

    private sealed record CreateCustomerResponse(Guid CustomerId);
}
