namespace CampFitFurDogs.Api.Horizontals.Session.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SessionValidationMiddleware>();
    }
}
