using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CampFitFurDogs.IntegrationTests.Fixtures;

public class ApiFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            };

            config.AddInMemoryCollection(settings!);
        });
    }
}
