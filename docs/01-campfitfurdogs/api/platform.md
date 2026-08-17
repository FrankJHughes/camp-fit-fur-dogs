# API Platform

The CampFitFurDogs API platform provides the composition root for the product’s vertical slice. It centralizes cross-cutting service registration, integrates platform modules from Frank.Core and Frank.Identity, and ensures that the API layer remains thin, declarative, and environment‑aware.

## Platform Composition

The platform orchestrates the registration of all major subsystems through a single fluent extension method:

```csharp
public static IServiceCollection AddCampFitFurDogsApiPlatform(
    this IServiceCollection services,
    IConfiguration configuration)
{
    return services
        .AddCampFitFurDogsApplication()                 // CQRS handlers, validators
        .AddCampFitFurDogsInfrastructure(configuration) // Persistence, databases
        .AddCampFitFurDogsApiExceptionHandlers();       // Exception handlers
}
```

This method forms the backbone of the product’s dependency graph.

## Service Registration Order

### 1. Application Layer

Registers all CQRS components:

- command handlers for dog operations  
- query handlers for dog retrieval  
- FluentValidation validators  
- application services  

```csharp
public static IServiceCollection AddCampFitFurDogsApplication(
    this IServiceCollection services)
{
    services
        .AddApplicationDogs()  // Dogs CQRS handlers/validators
        .AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

    return services;
}
```

### 2. Infrastructure Layer

Registers persistence and external integrations:

- EF Core database contexts (PostgreSQL)  
- dog persistence readers and writers  
- unit of work implementation  
- HTTP context accessor for current user resolution  

```csharp
public static IServiceCollection AddCampFitFurDogsInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    return services
        .AddHttpContextAccessor()
        .AddInfrastructureDbContexts(configuration)
        .AddInfrastructureDogs()
        .AddInfrastructureUnitOfWork();
}
```

### 3. Exception Handling Layer

Registers all API-level exception handlers:

- domain exception handlers  
- validation exception handlers  
- bad request handlers  
- unexpected exception handlers  

These handlers convert exceptions into RFC 7807 `ProblemDetails` responses.

## Endpoint Registration

Endpoints are registered separately from services and discovered via assembly scanning:

```csharp
// In Program.cs
services.AddCampFitFurDogsApiEndpoints();

// Later in the pipeline
app.MapRegisteredApiEndpoints("/api");
```

Any class implementing `IEndpoint` in the CampFitFurDogs.Api assembly is automatically mapped under the `/api` prefix.

## Middleware Pipeline Integration

The CampFitFurDogs platform composes into the global middleware pipeline through Frank.Core and Frank.Identity. The full pipeline order is:

1. **Global logging + exception boundary** (Frank.Core)  
2. **Observability** (Frank.Core) — correlation IDs, request tracking  
3. **Routing** (Frank.Core)  
4. **CORS** (Frank.Core) — origin validation  
5. **Authentication** (Frank.Identity)  
6. **Authorization** (Frank.Identity)  
7. **Swagger** (Frank.Core) — development only  

This ensures consistent behavior across environments and vertical slices.

## Configuration

The platform reads configuration from `appsettings.json` and passes it to infrastructure components:

```json
{
  "ConnectionStrings": {
    "AppDb": "Server=localhost;Database=campfitfurdogs;User Id=postgres;Password=..."
  },
  "Hosting": {
    "Environment": "Development"
  }
}
```

Configuration drives:

- database connection strings  
- environment-specific hosting behavior  
- logging configuration  
- CORS policy settings  

## Dependency Injection Lifetimes

Services are registered with lifetimes appropriate to their responsibilities:

- **Transient** — CQRS handlers (stateless, per request)  
- **Scoped** — DbContext, UnitOfWork (per HTTP request)  
- **Scoped** — persistence readers/writers  
- **Singleton** — configuration, logging  

This ensures predictable behavior and avoids resource contention.

## Service Dependencies

The service graph flows from Platform → Application → Infrastructure:

```
Program.cs
    └─ AddCampFitFurDogsApiPlatform()
         ├─ AddCampFitFurDogsApplication()
         │   ├─ AddApplicationDogs()
         │   └─ AddValidators()
         ├─ AddCampFitFurDogsInfrastructure()
         │   ├─ AddInfrastructureDbContexts()
         │   ├─ AddInfrastructureDogs()
         │   └─ AddInfrastructureUnitOfWork()
         └─ AddCampFitFurDogsApiExceptionHandlers()
```

This structure keeps the API layer declarative and ensures vertical slices remain cohesive.

## Extending the Platform

To add new domain features (e.g., Plans, Schedules):

1. **Create vertical slice structure:**
   ```
   Domain/FeatureName/          — Aggregates, value objects
   Application/FeatureName/     — CQRS handlers, validators
   Infrastructure/FeatureName/  — Persistence implementations
   Api/Endpoints/FeatureName/   — HTTP endpoints
   ```

2. **Add service registration:**
   ```csharp
   services.AddApplicationFeatureName();
   services.AddInfrastructureFeatureName();
   ```

3. **Wire into platform:**
   Update `AddCampFitFurDogsApplication()` and `AddCampFitFurDogsInfrastructure()`.

4. **Register endpoints:**
   Implement `IEndpoint` in `Api/Endpoints/FeatureName/`.

This keeps new features aligned with the vertical-slice architecture.

## Testing the Platform

Integration tests use `WebApplicationFactory` to bootstrap the full platform:

```csharp
public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace real database with test database
            // Replace external services with mocks
        });
    }
}
```

This verifies that services, middleware, and endpoints are wired correctly before deployment.

## Source References

- `src/CampFitFurDogs/Api/Platform/ServiceCollectionExtensions.cs` — platform composition  
- `src/CampFitFurDogs/Application/ServiceCollectionExtensions.cs` — application registration  
- `src/CampFitFurDogs/Infrastructure/ServiceCollectionExtensions.cs` — infrastructure registration  
- `src/CampFitFurDogs/Api/Program.cs` — hosting composition  
