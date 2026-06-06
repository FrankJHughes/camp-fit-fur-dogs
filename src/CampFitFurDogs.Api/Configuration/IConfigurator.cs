namespace CampFitFurDogs.Api.Configuration;

public interface IConfigurator
{
    static abstract void ConfigureServices(IServiceCollection services, IConfiguration config);
    static abstract void Configure(WebApplication app);
}
