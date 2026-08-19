# API Platform

The **CampFitFurDogs.Api Platform** defines all API‑specific service registration, exception handling, validation wiring, and endpoint discovery.  
It is intentionally **host‑agnostic**: the API assembly declares *what* must be registered, while the **CampFitFurDogs.Host** project decides *when* and *how* these registrations are activated during startup.

This separation keeps the API layer pure, declarative, and reusable across hosting environments.

---

## Purpose

The API platform provides:

- API‑specific DI registration  
- FluentValidation registration  
- API exception‑handling registration  
- Request validation observability wiring  
- Endpoint discovery via assembly scanning  
- Integration points for Frank.Core and Frank.Identity  

The platform does **not** configure hosting, middleware, or environment adaptation.  
Those responsibilities belong to **CampFitFurDogs.Host**.

---

## Platform Composition

The platform orchestrates registration of all major subsystems through a single extension method:

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

---

## Service Registration Order

### 1. Application Layer

Registers all CQRS components:

- command handlers  
- query handlers  
- FluentValidation validators  
- application services  

```csharp
public static IServiceCollection AddCampFitFurDogsApplication(
    this IServiceCollection services)
{
    services
        .AddApplicationDogs()
        .AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

    return services;
}
```

### 2. Infrastructure Layer

Registers persistence and external integrations:

- EF Core DbContexts  
- readers and writers  
- unit of work implementation  
- HTTP context accessor  

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

Registers all API‑level exception handlers:

- domain exception handlers  
- validation exception handlers  
- bad request handlers  
- unexpected exception handlers  

These handlers convert exceptions into RFC 7807 `ProblemDetails` responses.

---

## Request Validation Observability

The API platform wires the request‑validation observability pipeline, which emits:

- `api.validation.start`  
- `api.validation.end`  
- `api.validation.failed`  
- `api.validation.exception`  

These events are **API‑specific**, but emitted through **Frank.Core’s observability engine**.

The Host project activates this pipeline via:

```csharp
app.UseFrankCoreApiPlatform();
```

This ensures:

- validation observability is active in all environments  
- invalid requests never reach the Application layer  
- consistent diagnostics across slices  

---

## Endpoint Registration

Endpoints are registered separately from services and discovered via assembly scanning:

```csharp
services.AddCampFitFurDogsApiEndpoints();
```

The Host project later maps them under the `/api` prefix:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

Any class implementing `IEndpoint` in the CampFitFurDogs.Api assembly is automatically mapped.

This keeps endpoint registration declarative and slice‑aligned.

---

## Middleware Pipeline Integration (Host‑Activated)

The API platform does **not** configure middleware.  
Instead, the Host project composes the global pipeline using Frank.Core and Frank.Identity:

1. Global logging + exception boundary  
2. Observability (correlation IDs, request tracking)  
3. Routing  
4. CORS  
5. Authentication  
6. Authorization  
7. Swagger (development only)

This ensures consistent behavior across environments and vertical slices.

---

## Configuration

The API platform receives configuration from the Host project and passes it to infrastructure components:

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
- environment‑specific hosting behavior  
- logging configuration  
- CORS policy settings  

---

## Dependency Injection Lifetimes

Services are registered with lifetimes appropriate to their responsibilities:

- **Transient** — CQRS handlers  
- **Scoped** — DbContext, UnitOfWork, readers/writers  
- **Singleton** — configuration, logging  

This ensures predictable behavior and avoids resource contention.

---

## Service Dependencies

The service graph flows from Platform → Application → Infrastructure:

```
Host (Program.cs)
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

---

## Extending the Platform

To add new domain features (e.g., Plans, Schedules):

1. **Create vertical slice structure:**
   ```
   Domain/FeatureName/
   Application/FeatureName/
   Infrastructure/FeatureName/
   Api/Endpoints/FeatureName/
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

This keeps new features aligned with the vertical‑slice architecture.

---

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

---

## Source References

- `src/CampFitFurDogs/Api/Platform/ServiceCollectionExtensions.cs`  
- `src/CampFitFurDogs/Application/ServiceCollectionExtensions.cs`  
- `src/CampFitFurDogs/Infrastructure/ServiceCollectionExtensions.cs`  
- `src/CampFitFurDogs.Host/Program.cs` — hosting composition (new)
