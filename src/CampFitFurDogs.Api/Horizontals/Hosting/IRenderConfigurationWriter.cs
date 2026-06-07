namespace CampFitFurDogs.Api.Horizontals.Hosting;

public interface IRenderConfigurationWriter
{
    void Apply(WebApplicationBuilder builder, string dbConnectionString, string frontendBaseUrl);
}
