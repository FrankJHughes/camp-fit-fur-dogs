# Endpoints

The **Endpoints** subsystem provides discovery, registration, and mapping of
API endpoints implemented across vertical slices.  
It connects the Frank.Core application abstractions (`IEndpoint`) with the
ASP.NET Core routing pipeline.

Endpoints are intentionally simple: each slice defines its own endpoint class,
implements `IEndpoint`, and exposes a `Map` method.  
This folder contains the infrastructure that discovers those endpoints and
maps them exactly once.

All routes are mapped **relative to the `/api` group**, which is created in
Program.cs:

```csharp
app.MapRegisteredApiEndpoints("/api")
    .WithTags("API")
    .WithDescription("Camp Fit Fur Dogs API");
```

---

## Files

```
Endpoints/
├── EndpointRouteBuilderExtensions.cs
└── EndpointServiceCollectionExtensions.cs
```

---

## EndpointRouteBuilderExtensions

`EndpointRouteBuilderExtensions` provides the runtime mapping mechanism.

### Responsibilities

- Retrieves all registered `IEndpoint` implementations from DI.
- Deduplicates endpoints by concrete type name.
- Invokes each endpoint’s `Map(RouteGroupBuilder api)` method.
- Ensures every slice-defined endpoint is mapped exactly once.
- Applies core routing behaviors (filtering, validation) before mapping.

### Why this matters

Vertical slices register endpoints independently.  
This extension ensures they are all mapped without requiring manual wiring in
`Program.cs` or `Startup.cs`.

Usage example:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

---

## EndpointServiceCollectionExtensions

`EndpointServiceCollectionExtensions` provides discovery and registration of
endpoint implementations using the Frank.Core orchestrator.

### Responsibilities

- Scans assemblies for types that:
  - implement `IEndpoint`
  - AND are decorated with `[Registration]`
- Registers all implementations of `IEndpoint` found in the scanned assemblies.
- Allows customization of discovery rules via `DiscoveryOptions`.
- Automatically includes the `Frank.Core.Application` assembly in the scan.

### Why this matters

Vertical slices define endpoints in their own assemblies.  
This extension ensures those endpoints are discovered and registered without
requiring manual DI calls.

Usage example:

```csharp
services.AddFrankCoreApiEndpoints(
    assembliesToSearch: new[] { typeof(MySlice.AssemblyMarker).Assembly }
);
```

---

## How Endpoints Fit Into the Architecture

Endpoints are the outermost layer of each vertical slice.  
They:

- expose HTTP routes  
- translate requests into slice commands/queries  
- apply slice-specific authorization  
- emit observability events  
- remain free of business logic  

The Endpoints subsystem ensures:

- endpoints are discovered automatically  
- endpoints are registered automatically  
- endpoints are mapped automatically  

This keeps slice code clean and avoids boilerplate in the host application.

---

## Typical Flow

1. **Slice defines an endpoint**

```csharp
[Registration]
public sealed class CreateDogEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapPost("/dogs", HandleAsync);
    }
}
```

2. **Host registers endpoints**

```csharp
services.AddFrankCoreApiEndpoints(assembliesToSearch);
```

3. **Host maps endpoints**

```csharp
app.MapRegisteredApiEndpoints("/api");
```

Everything else is automatic.

---

## Design Principles

- **Vertical-slice friendly**  
  Endpoints live with their slice, not in a central routing file.

- **Convention-based discovery**  
  `[Registration]` + `IEndpoint` = discoverable.

- **Zero boilerplate**  
  No manual DI registration or route mapping.

- **Deterministic mapping**  
  Deduplication ensures each endpoint maps exactly once.

- **Group-relative routing**  
  Endpoints never hard‑code `/api`.

---

## Notes

- Endpoint discovery is opt‑in: if no assemblies are provided, nothing is registered.
- Mapping is runtime-only: endpoints must be registered before mapping.
- The orchestrator ensures consistent discovery rules across all subsystems.

