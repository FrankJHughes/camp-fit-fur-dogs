# Routing

The **Routing** folder contains all infrastructure related to Minimal API routing
in Frank.Core.  
It provides unified endpoint filtering, automatic request validation, and
OpenAPI metadata support — all applied at the route‑group level.

This subsystem ensures that vertical slices remain clean while cross‑cutting
behaviors are applied consistently across the entire API surface.

Routing in Frank.Core is composed of three parts:

- **Core Routing** — unified endpoint filtering  
- **Validation** — FluentValidation‑based request validation  
- **OpenApi** — group‑level OpenAPI metadata  

---

## Folder Structure

```
Routing/
│
├── RouteGroupBuilderExtensions.cs        # Unified endpoint filtering
│
├── Validation/
│   ├── EndpointFilter.cs
│   ├── RouteGroupBuilderExtensions.cs
│   └── RouteHandlerBuilderExtensions.cs
│
└── OpenApi/
    └── RouteGroupBuilderExtensions.cs
```

---

# Core Routing

## RouteGroupBuilderExtensions (root)

The root `RouteGroupBuilderExtensions` file provides unified endpoint filtering
for Minimal API route groups.

### Responsibilities

- Adds a global endpoint filter factory to the route group  
- Ensures all endpoints in the group execute a shared filter pipeline  
- Provides a single hook for cross‑cutting behaviors such as:
  - Logging  
  - Metrics  
  - Correlation IDs  
  - Domain‑specific filters  
  - Validation (via the Validation subsystem)  

### Example

```csharp
app.MapRegisteredApiEndpoints("/api")
   .AddEndpointFiltering();
```

### Why this matters

Minimal API endpoints are intentionally lightweight.  
This extension allows you to configure shared behaviors once per group, keeping
endpoint classes focused solely on request handling.

---

# Validation Subsystem

The **Validation** folder provides FluentValidation‑based request validation for
Minimal API endpoints.

It integrates with the unified routing pipeline so that request DTOs are
validated automatically before endpoint handlers execute.

### Components

#### EndpointFilter\<TRequest>
- Extracts the request DTO  
- Runs the corresponding FluentValidation validator  
- Throws `ValidationException` on failure  
- Allows the handler to run only when validation succeeds  

#### RouteGroupBuilderExtensions (Validation)
- Adds automatic request validation to an entire route group  
- Detects request DTOs with registered validators  
- Attaches the correct `EndpointFilter<TRequest>` automatically  

#### RouteHandlerBuilderExtensions
- Adds validation to a single endpoint  
- Useful for fine‑grained control  

### Example

```csharp
app.MapRegisteredApiEndpoints("/api")
   .AddRequestValidation();
```

---

# OpenApi Subsystem

The **OpenApi** folder provides group‑level OpenAPI metadata for route groups.

### Responsibilities

- Applies OpenAPI tags  
- Applies human‑readable descriptions  
- Produces cleaner, more organized OpenAPI output  
- Replaces the deprecated `WithOpenApi()` extension  

### Example

```csharp
app.MapRegisteredApiEndpoints("/api")
   .AddOpenApiMetadata(
       tag: "API",
       description: "Camp Fit Fur Dogs API"
   );
```

---

# How Routing Fits Into the Architecture

Routing is the glue between:

- Endpoint discovery  
- Endpoint registration  
- Endpoint mapping  
- Endpoint filtering  
- Validation  
- OpenAPI documentation  

Vertical slices define endpoints.  
The Routing subsystem ensures they all share:

- a unified filter pipeline  
- automatic validation  
- consistent OpenAPI metadata  

without requiring slice authors to write boilerplate.

---

# Design Principles

- **Centralized filtering**  
  Cross‑cutting behaviors configured once per group.

- **Minimal API alignment**  
  Uses endpoint filter factories instead of MVC filters.

- **Extensible**  
  New filters or metadata can be added without modifying endpoint classes.

- **Vertical‑slice friendly**  
  Endpoints remain clean and focused on business logic.

- **Group‑relative routing**  
  All routing behavior is applied at the route‑group level.

---

# Summary

The Routing subsystem provides:

- Unified endpoint filtering  
- Automatic FluentValidation request validation  
- Group‑level OpenAPI metadata  
- Clean integration with Minimal APIs  
- A foundation for logging, metrics, correlation IDs, and more  

This folder ensures that all endpoints in the Camp Fit Fur Dogs API execute
consistently and predictably within a shared routing pipeline.
