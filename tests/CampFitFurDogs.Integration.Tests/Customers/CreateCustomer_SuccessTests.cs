using System.Net;
using System.Net.Http.Json;
using CampFitFurDogs.Domain.Customers;
using CampFitFurDogs.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CampFitFurDogs.TestUtilities.Factories;
using CampFitFurDogs.TestUtilities.Fixtures;

namespace CampFitFurDogs.Integration.Tests.Customers;

[Collection("API With Postgres")]
public class CreateCustomer_SuccessTests : ApiWithPostgresTestBase
{
    public CreateCustomer_SuccessTests(
        CampFitFurDogsApiFactory factory,
        PostgresFixture fixture)
        : base(factory, fixture)
    {
    }

    private sealed record CreateCustomerResponse(Guid CustomerId);

    [Fact]
    public async Task CreateCustomer_SuccessfullyPersistsCustomer_AndReturns201()
    {
        var client = CreateClient();

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
        var response = await client.PostAsJsonAsync("/api/customers", request);

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>();
        body.Should().NotBeNull();
        body!.CustomerId.Should().NotBe(Guid.Empty);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should().Be($"/api/customers/{body.CustomerId}");

        // Assert — Database
        await using var scope = Factory.Services.CreateAsyncScope();
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
        customer.Phone!.Value.Should().Be("+19165551234");

        // Password hashed (not plaintext)
        customer.PasswordHash!.Value.Should().NotBe(request.Password);
        customer.PasswordHash.Verify(request.Password).Should().BeTrue();
    }
}
