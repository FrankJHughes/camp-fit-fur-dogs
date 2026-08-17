# Platform Swagger / OpenAPI

The **Platform Swagger** subsystem provides environment‑aware OpenAPI support for
the Frank.Core API.  
It registers the OpenAPI document generator and conditionally exposes the
OpenAPI endpoint only in development environments.

This ensures that API documentation is available to developers while keeping
production deployments secure, minimal, and free of unnecessary surface area.

---

## Files

```
Swagger/
├── ServiceCollectionExtensions.cs
└── ApplicationBuilderExtensions.cs
```

---

## ServiceCollectionExtensions

`ServiceCollectionExtensions` registers the OpenAPI generation infrastructure.

### Responsibilities

- Adds the OpenAPI document generator via `AddOpenApi()`.
- Ensures the API can produce an OpenAPI specification.
- Keeps registration centralized and platform‑consistent.

### Why this matters

This subsystem ensures:

- OpenAPI generation is **always available**, regardless of environment.
- Exposure of the OpenAPI endpoint is **controlled at runtime**.
- Vertical slices do not need to register Swagger themselves.

### Usage

```csharp
services.AddFrankCoreApiPlatformSwagger();
```

This prepares the OpenAPI generator for use by the application pipeline.

---

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` conditionally exposes the OpenAPI endpoint.

### Responsibilities

- Detects the current hosting environment.
- Maps the OpenAPI document via `MapOpenApi()` **only in Development**.
- Prevents Swagger/OpenAPI exposure in production environments.

### Why this matters

- Developers get full API documentation during local development.
- Production environments remain secure and minimal.
- Documentation exposure is predictable and environment‑driven.

### Usage

```csharp
app.UseFrankCoreApiPlatformSwagger();
```

This exposes the OpenAPI endpoint only when appropriate.

---

## How Platform Swagger Fits Into the Architecture

Platform Swagger is part of the API’s hosting and developer‑experience layer.  
It complements:

- **[Platform Logging](ca://s?q=Tell_me_more_about_Platform_Logging)**  
- **[Platform CORS](ca://s?q=Tell_me_more_about_CORS_configuration)**  
- **[Security Headers](ca://s?q=Tell_me_more_about_Security_Headers_middleware)**  
- **[Observations](ca://s?q=Tell_me_more_about_Observations_middleware)**  

Together, these subsystems create a secure, observable, developer‑friendly API
platform.

---

## Design Principles

- **Environment‑aware**  
  Swagger is exposed only in development.

- **Developer‑focused**  
  OpenAPI is available where it helps most.

- **Secure by default**  
  No documentation endpoints in production.

- **Centralized configuration**  
  Registration and exposure are defined once.

---

## Notes

- `AddOpenApi()` registers the generator; `MapOpenApi()` exposes it.
- Additional Swagger UI or tooling can be added later if needed.
- This subsystem is safe for all environments.

---
