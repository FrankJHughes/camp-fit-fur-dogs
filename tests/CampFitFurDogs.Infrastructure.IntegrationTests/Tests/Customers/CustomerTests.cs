using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CampFitFurDogs.InfrastructureIntegrationTests.Fixtures;
using Xunit;

namespace CampFitFurDogs.InfrastructureIntegrationTests.Tests.Customers;

public class CustomerTests : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client;

    public CustomerTests(ApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Can_Create_And_List_Customers()
    {
        var createResponse = await _client.PostAsJsonAsync("/customers", new
        {
            firstName = "Test",
            lastName = "User",
            email = $"test-{Guid.NewGuid()}@example.com",
            phone = "555-5555",
            password = "P@ssw0rd!"
        });

        createResponse.EnsureSuccessStatusCode();

        var listResponse = await _client.GetAsync("/customers");
        listResponse.EnsureSuccessStatusCode();
    }
}
