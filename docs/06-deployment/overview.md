# Deployment Overview
Deployment is handled through environment-aware startup, configuration, and runtime composition in the API project.
## Deployment model
The application is designed to run through standard ASP.NET Core hosting with environment-specific configuration wired through the startup pipeline:
- \src/CampFitFurDogs/Api/Program.cs\ ΓÇö application bootstrap and DI setup
- \src/CampFitFurDogs/Api/Helpers/Hosting.cs\ ΓÇö environment-aware hosting configuration
- \compose.yml\ ΓÇö local development with Docker
- \Dockerfile\ ΓÇö container image for any environment
- \ender.yaml\ ΓÇö Render.com deployment configuration
## Environment variables
Key environment variables for deployment:
- \ASPNETCORE_ENVIRONMENT\ ΓÇö Development, Testing, Production
- \ConnectionStrings__DefaultConnection\ ΓÇö database connection string
- \Identity__Oidc__ClientSecret\ ΓÇö OAuth2 client secret
- \Identity__Oidc__Authority\ ΓÇö identity provider URL
- \Frontend__BaseUrl\ ΓÇö frontend application URL
- \Email__SendGridApiKey\ ΓÇö email service API key
## Hosting modules
Environment-specific configuration is applied via hosting modules:
\\\csharp
public static IHostingModule[] ConstructHostingModules()
{
    return [ new RenderPrPreviewHostingModule() ];
}
\\\
Each module adapts settings based on where the app is running (Render PR preview, production, etc.).
## Database deployment
- Entity Framework Core handles schema management
- Migrations are tracked in \src/*/Infrastructure/Migrations\
- Apply migrations at startup: \dotnet ef database update\
- Or manually via script during deployment
## Container deployment
- \Dockerfile\ multi-stage build for optimized size
- Base image: \mcr.microsoft.com/dotnet/aspnet:8.0\
- Expose port 8080 for HTTP
- Set \ASPNETCORE_URLS=http://+:8080\
## Health checks
The API provides health endpoints for deployment readiness:
- \GET /health\ ΓÇö basic liveness check
- \GET /health/ready\ ΓÇö readiness check (database, dependencies)
## Observability in production
- Correlation IDs for request tracing across logs
- Structured logging for aggregation and alerting
- Exception tracking and error reporting
- Metrics and performance monitoring
