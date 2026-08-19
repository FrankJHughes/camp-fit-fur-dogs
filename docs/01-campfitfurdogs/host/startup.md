# Startup Pipeline

The **CampFitFurDogs.Host** project defines the complete startup pipeline for the Camp Fit Fur Dogs platform.  
It builds the web host, applies environment‑specific configuration, registers platform services, activates middleware, and maps all API endpoints.

This document explains each stage of the startup process.

---

## 1. Build the WebApplicationBuilder

Startup begins with:

```csharp
var builder = WebApplication.CreateBuilder(args);
```

The builder provides:

- configuration loading  
- logging setup  
- DI container initialization  
- environment detection  

The Host layer owns this lifecycle.

---

## 2. Apply Hosting Modules

Hosting modules live in the **API assembly**, but are **executed by the Host**:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

Hosting modules provide:

- Render PR Preview configuration overrides  
- GitHub artifact integration  
- environment‑specific settings  
- dynamic configuration merging  

This keeps environment logic out of the API layer.

---

## 3. Register Platform Services

The Host composes all platform layers:

```csharp
services
    .AddCampFitFurDogsApiPlatform(configuration)
    .AddFrankCoreApiPlatform(configuration)
    .AddFrankIdentityApiPlatform(configuration);
```

This registers:

- API services (DTOs, validators, exception handlers)
- Application services (CQRS)
- Infrastructure services (DbContexts, readers/writers)
- Identity services (OIDC, sessions)
- Core services (observability, routing, CORS)

---

## 4. Register Endpoints

Endpoints are discovered via assembly scanning:

```csharp
services
    .AddCampFitFurDogsApiEndpoints()
    .AddFrankIdentityApiEndpoints();
```

The Host later maps them under `/api`:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

This keeps endpoint registration declarative and slice‑aligned.

---

## 5. Activate Middleware Pipeline

The Host activates the global middleware pipeline:

```csharp
app
    .UseFrankCoreApiPlatform()
    .UseFrankIdentityApiPlatform();
```

This enables:

- global exception boundary  
- correlation IDs  
- request tracking  
- routing  
- CORS  
- authentication  
- authorization  
- Swagger (development only)

The API layer does **not** configure middleware; it only defines API‑specific components that the Host activates.

---

## 6. Run the Application

Finally:

```csharp
app.Run();
```

The Host project is the only layer that calls `Run()`.

---

## Summary

The Host startup pipeline:

1. builds the host  
2. applies hosting modules  
3. registers platform services  
4. registers endpoints  
5. activates middleware  
6. runs the application  

This keeps the API layer pure and ensures consistent startup across all environments.
