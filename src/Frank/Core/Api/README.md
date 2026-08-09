# Frank.Core API — API Layer

The **Api** layer is the topmost layer of the Frank.Core architecture.  
It hosts the HTTP surface of the system, composes platform‑level capabilities,
and integrates cross‑cutting middleware with vertical slices, application
services, and infrastructure.

The Api layer does **not** contain business logic.  
Instead, it defines how the application is *presented* to the outside world:
routing, middleware, platform configuration, and the unified hosting model.

---

## Purpose of the API Layer

The Api layer provides:

- A **single entry point** for the entire HTTP API.
- Composition of **[Platform](ca://s?q=Tell_me_more_about_the_Platform_layer)** subsystems (CORS, Logging, Swagger, Pipeline).
- Composition of **[Middleware](ca://s?q=Tell_me_more_about_API_Middleware)** subsystems (Observations, Exceptions, Security Headers, Origin Logging).
- Integration of **[Application](ca://s?q=Explain_Application_layer_services)** and **[Infrastructure](ca://s?q=Explain_Infrastructure_layer_services)** layers.
- A predictable, secure, observable hosting environment for all vertical slices.

It is the “shell” of the system — everything flows through it.

---

## Folder Structure

```
Api/
├── Platform/
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
├── Middleware/
│   ├── Observations/
│   ├── Exceptions/
│   ├── SecurityHeaders/
│   ├── Cors/
│   └── OriginLogging/
└── (Vertical slices live outside Api, but are hosted here)
```

---

## Key Responsibilities

### 1. Platform Service Composition

The Api layer registers all platform subsystems through:

```csharp
services.AddFrankCoreApiPlatform(Configuration);
```

This includes:

- **[Platform CORS](ca://s?q=Tell_me_more_about_CORS_configuration)**  
- **[Platform Logging](ca://s?q=Tell_me_more_about_Platform_Logging)**  
- **[Platform Swagger](ca://s?q=Tell_me_more_about_Swagger_configuration)**  
- **[Application Layer](ca://s?q=Explain_Application_layer_services)**  
- **[Infrastructure Layer](ca://s?q=Explain_Infrastructure_layer_services)**  
- **[API Middleware](ca://s?q=Tell_me_more_about_API_Middleware)**  

---

### 2. Platform Pipeline Assembly

The Api layer assembles the full middleware pipeline:

```csharp
app.UseFrankCoreApiPlatform();
```

This pipeline ensures:

- Global logging  
- Exception boundary  
- Observability context  
- Routing  
- CORS  
- Origin logging  
- Swagger (dev‑only)  

---

### 3. Hosting Vertical Slices

Vertical slices register their endpoints via:

```csharp
app.MapGroup("/dogs").MapDogEndpoints();
```

The Api layer ensures slices run inside a secure, observable, consistent
environment.

---

## How the API Layer Fits Into the Architecture

The Api layer sits at the top of the architecture:

```
[ API ]
   ↓
[ Platform ]
   ↓
[ Middleware ]
   ↓
[ Application ]
   ↓
[ Infrastructure ]
```

It is responsible for:

- Bootstrapping the system  
- Composing platform services  
- Assembling the middleware pipeline  
- Hosting vertical slices  
- Exposing the HTTP interface  

Everything that enters or leaves the system flows through the Api layer.

---

## Design Principles

- **Separation of concerns**  
  Api composes; Platform configures; Middleware implements; Application executes.

- **Environment‑aware behavior**  
  Development gets enhanced tooling (logging, Swagger); production stays minimal.

- **Predictable pipeline**  
  All requests follow the same structured flow.

- **Cross‑cutting consistency**  
  Observability, security, and error handling apply uniformly.

- **Single entry point**  
  One place defines how the API is hosted.

---

## Notes

- The Api layer should remain thin and compositional.  
- No business logic should live here.  
- All cross‑cutting concerns should be implemented in Middleware or Platform.  
- Vertical slices should remain independent and self‑contained.

---
