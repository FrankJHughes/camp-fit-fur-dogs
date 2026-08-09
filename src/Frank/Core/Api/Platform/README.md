# Frank.Core API — Platform Layer

The **Platform** layer defines the foundational hosting, configuration, and
cross‑cutting service composition for the Frank.Core API.  
It provides a unified entry point for registering platform services and assembling
the top‑level middleware pipeline that governs logging, exception handling,
observability, routing, CORS, origin logging, and development‑only Swagger
exposure.

The Platform layer does **not** implement business logic.  
Instead, it orchestrates the infrastructure that makes the API secure,
observable, predictable, and environment‑aware.

---

## Files

```
Platform/
├── ServiceCollectionExtensions.cs
└── ApplicationBuilderExtensions.cs
```

---

## ServiceCollectionExtensions

`ServiceCollectionExtensions` composes all platform‑level service registrations
into a single fluent extension method.

### Responsibilities

- Registers **[Platform CORS](ca://s?q=Tell_me_more_about_CORS_configuration)**  
- Registers **[Platform Logging](ca://s?q=Tell_me_more_about_Platform_Logging)**  
- Registers **[Platform Swagger](ca://s?q=Tell_me_more_about_Swagger_configuration)**  
- Registers **[Application Layer](ca://s?q=Explain_Application_layer_services)**  
- Registers **[Infrastructure Layer](ca://s?q=Explain_Infrastructure_layer_services)**  
- Registers **[API Middleware](ca://s?q=Tell_me_more_about_API_Middleware)**

### Why this matters

This unified registration method ensures:

- All platform subsystems are consistently configured  
- Service registration is centralized and predictable  
- The API is fully prepared before the middleware pipeline is assembled  
- Vertical slices remain focused on business logic, not platform concerns  

### Usage

```csharp
services.AddFrankCoreApiPlatform(Configuration);
```

This single call configures the entire platform layer.

---

## ApplicationBuilderExtensions

`ApplicationBuilderExtensions` defines the **phase‑driven platform pipeline**.

### Pipeline Phases

#### **Phase 1 — Global Logging + Exception Boundary**
```csharp
app.UseFrankCoreApiPlatformLogging();
app.UseFrankCoreApiMiddlewareExceptions();
```
- Enables HTTP logging in development  
- Establishes the global exception handler  

#### **Phase 2 — Observability**
```csharp
app.UseFrankCoreApiMiddlewareObservations();
```
- Creates and populates the request‑level observation context  
- Ensures correlation and trace continuity  

#### **Phase 3 — Routing + CORS + Origin Logging**
```csharp
app.UseRouting();
app.UseCors();
app.UseFrankCoreApiMiddlewareOriginLogging();
```
- Activates endpoint routing  
- Applies the platform CORS policy  
- Logs inbound origin information  

#### **Phase 4 — Swagger (Development Only)**
```csharp
app.UseFrankCoreApiPlatformSwagger();
```
- Exposes OpenAPI only in development  
- Keeps production secure and minimal  

### Why this matters

The platform pipeline ensures:

- Correct ordering of cross‑cutting concerns  
- Unified observability across slices and external services  
- Secure defaults (exception boundary, CORS, no Swagger in production)  
- Developer‑friendly tooling in development environments  
- Predictable request lifecycle  

### Usage

```csharp
app.UseFrankCoreApiPlatform();
```

This single call assembles the entire platform pipeline.

---

## How the Platform Layer Fits Into the Architecture

The Platform layer sits between **Hosting** and **Middleware**, orchestrating
cross‑cutting concerns that apply to all vertical slices.

It complements:

- **[Middleware](ca://s?q=Tell_me_more_about_API_Middleware)**  
- **[Application](ca://s?q=Explain_Application_layer_services)**  
- **[Infrastructure](ca://s?q=Explain_Infrastructure_layer_services)**  
- **[Vertical Slices](ca://s?q=Explain_vertical_slice_architecture)**  

Together, these layers form a secure, observable, maintainable API foundation.

---

## Design Principles

- **Centralized configuration**  
  One place to register all platform services.

- **Phase‑driven pipeline**  
  Middleware ordering is intentional and deterministic.

- **Environment‑aware behavior**  
  Development gets enhanced tooling; production stays minimal.

- **Separation of concerns**  
  Platform composes; subsystems implement.

- **Predictability**  
  Every request flows through the same structured pipeline.

---

## Notes

- The Platform layer is intentionally minimal and compositional.  
- Additional phases (authentication, metrics, rate limiting) can be added later.  
- Vertical slices should never register cross‑cutting middleware themselves.

---
