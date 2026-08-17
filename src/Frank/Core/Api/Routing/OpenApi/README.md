# OpenApi

The **OpenApi** folder contains extensions that enhance API route groups with
OpenAPI‑friendly metadata.  
These extensions replace the deprecated `WithOpenApi()` mechanism and instead
use modern endpoint metadata attributes that are automatically consumed by
OpenAPI generators such as Swashbuckle and NSwag.

This subsystem ensures that every API group can be annotated with consistent,
human‑readable documentation without requiring individual endpoints to repeat
metadata.

---

## Files

```
OpenApi/
└── RouteGroupBuilderExtensions.cs
```

---

## RouteGroupBuilderExtensions

`RouteGroupBuilderExtensions` provides the `AddOpenApiMetadata` extension method,
which applies OpenAPI metadata to an entire `RouteGroupBuilder`.

### Responsibilities

- Applies an OpenAPI tag to all endpoints in the group  
- Applies a human‑readable description  
- Produces cleaner, more organized OpenAPI output  
- Serves as a modern replacement for the deprecated `WithOpenApi()` extension

### Example

```csharp
app.MapRegisteredApiEndpoints("/api")
   .AddOpenApiMetadata(
       tag: "API",
       description: "Camp Fit Fur Dogs API"
   );
```

### Why this matters

- Keeps OpenAPI metadata centralized at the group level  
- Avoids duplication across endpoints  
- Works seamlessly with Minimal API route groups  
- Ensures consistent documentation across the entire API surface

---

## Design Principles

- **Group‑level metadata**  
  Documentation is applied once per group, not per endpoint.

- **Modern OpenAPI support**  
  Uses attributes consumed by Swashbuckle, NSwag, and other generators.

- **Minimal API alignment**  
  Designed specifically for `RouteGroupBuilder`‑based routing.

- **Zero boilerplate**  
  Endpoint classes remain focused on routing and request handling.

---

## Summary

The OpenApi subsystem provides a lightweight, modern way to annotate API route
groups with documentation metadata.  
By centralizing tags and descriptions, it keeps endpoints clean while producing
clear, structured OpenAPI output.

This folder contains:

- A single extension method for OpenAPI metadata  
- A clean replacement for `WithOpenApi()`  
- A Minimal API‑friendly approach to documentation
