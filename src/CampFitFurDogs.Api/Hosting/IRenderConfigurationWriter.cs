namespace CampFitFurDogs.Api.Hosting;

public interface IRenderConfigurationWriter
{
    void Apply(WebApplicationBuilder builder, string dbConnectionString, string frontendBaseUrl);
}
