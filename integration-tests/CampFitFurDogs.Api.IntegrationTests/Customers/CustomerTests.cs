// using System.Net.Http.Json;
// using Xunit;

// namespace CampFitFurDogs.Api.IntegrationTests.Tests.Users;

// public class UserTests : ApiTestBase
// {
//     [Fact]
//     public async Task Can_Create_And_List_Users()
//     {
//         var createResponse = await Client.PostAsJsonAsync("/api/users", new
//         {
//             firstName = "Test",
//             lastName = "User",
//             email = $"test-{Guid.NewGuid()}@example.com",
//             phone = "555-5555",
//             password = "P@ssw0rd!"
//         });

//         createResponse.EnsureSuccessStatusCode();

//         var listResponse = await Client.GetAsync("/api/users");
//         listResponse.EnsureSuccessStatusCode();
//     }
// }
