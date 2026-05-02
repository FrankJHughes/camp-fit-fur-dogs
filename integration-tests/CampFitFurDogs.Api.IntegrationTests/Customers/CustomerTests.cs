// using System.Net.Http.Json;
// using Xunit;

// namespace CampFitFurDogs.Api.IntegrationTests.Tests.Customers;

// public class CustomerTests : ApiTestBase
// {
//     [Fact]
//     public async Task Can_Create_And_List_Customers()
//     {
//         var createResponse = await Client.PostAsJsonAsync("/api/customers", new
//         {
//             firstName = "Test",
//             lastName = "User",
//             email = $"test-{Guid.NewGuid()}@example.com",
//             phone = "555-5555",
//             password = "P@ssw0rd!"
//         });

//         createResponse.EnsureSuccessStatusCode();

//         var listResponse = await Client.GetAsync("/api/customers");
//         listResponse.EnsureSuccessStatusCode();
//     }
// }
