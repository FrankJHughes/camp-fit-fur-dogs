using CampFitFurDogs.Api.Hosting;
using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeRenderConfigurationWriter : IRenderConfigurationWriter
{
    public string? DbConnectionString { get; private set; }
    public string? FrontendBaseUrl { get; private set; }

    public void Apply(WebApplicationBuilder builder, string dbConnectionString, string frontendBaseUrl)
    {
        DbConnectionString = dbConnectionString;
        FrontendBaseUrl = frontendBaseUrl;

        builder.Configuration["ConnectionStrings:DefaultConnection"] = dbConnectionString;
        builder.Configuration["Frontend__BaseUrl"] = frontendBaseUrl;
    }
}
