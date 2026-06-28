#nullable enable
namespace CampFitFurDogs.Api.Horizontal.Cors.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseOriginLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OriginLoggingMiddleware>();
    }
}
