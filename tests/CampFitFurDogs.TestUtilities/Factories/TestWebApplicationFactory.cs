using Microsoft.AspNetCore.Mvc.Testing;

namespace CampFitFurDogs.TestUtilities.Factories;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    // Intentionally empty.
    // All customization is done via WithWebHostBuilder in TestClientBuilder.
}
