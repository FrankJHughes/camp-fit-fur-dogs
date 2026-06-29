#nullable enable
namespace CampFitFurDogs.Api.Horizontals.Cors.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseOriginLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OriginLoggingMiddleware>();
    }
}
