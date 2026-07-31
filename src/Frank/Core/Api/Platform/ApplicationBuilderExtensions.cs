using Frank.Core.Api.Platform.Swagger;
using Frank.Core.Api.Middleware.Cors;
using Frank.Core.Api.Middleware.Exceptions;
using Frank.Core.Api.Middleware.Observations;
using Microsoft.AspNetCore.Builder;
using Frank.Core.Api.Platform.Logging;

namespace Frank.Core.Api.Platform;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseFrankCoreApiPlatform(this WebApplication app)
    {
        // Phase 1 — Global logging + exception boundary
        app.UseFrankCoreApiPlatformLogging();
        app.UseFrankCoreApiMiddlewareExceptions();

        // Phase 2 — Observability (request-level)
        app.UseFrankCoreApiMiddlewareObservations();

        // Phase 3 — Routing + CORS + origin logging
        app.UseRouting();
        app.UseCors();
        app.UseFrankCoreApiMiddlewareOriginLogging();

        // Phase 4 — Swagger (pre-auth)
        app.UseFrankCoreApiPlatformSwagger();

        return app;
    }
}
