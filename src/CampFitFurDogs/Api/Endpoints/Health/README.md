# Health — API Endpoints

The **Health** endpoint folder contains the minimal API surface used to report
the operational status of the Camp Fit Fur Dogs service.  
These endpoints are lightweight, anonymous, and designed for uptime monitoring,
load balancer probes, and external system health checks.

All routes are mapped **relative to the `/api` group**, which is created in
Program.cs:

```csharp
app.MapRegisteredApiEndpoints("/api")
    .WithTags("API")
    .WithDescription("Camp Fit Fur Dogs API");
```

---

## Files

### GetHealthEndpoint

**Route:** `GET /health`  
(Automatically becomes `GET /api/health`)

Returns a simple JSON payload indicating that the API is running.

Behavior:

- Always anonymous  
- Returns `200 OK` with `{ "Status": "Up" }`  
- Suitable for monitoring systems, readiness checks, and diagnostics

This endpoint does not perform any authentication, authorization, or
application‑layer queries. It is intentionally minimal.

---

### ServiceCollectionExtensions

Registers all health‑related endpoints using Frank.Core’s endpoint discovery
system.

Responsibilities:

- Scans the assembly containing `CampFitFurDogs.Api.AssemblyMarker`
- Filters to include only endpoint implementations under the
  `CampFitFurDogs.Api.Endpoints.Health` namespace
- Adds them via `AddFrankCoreApiEndpoints`

Usage:

```csharp
services.AddCampFitFurDogsApiEndpointsHealth();
```

---

## Design Principles

Health endpoints follow these principles:

- **Minimalism** — no dependencies, no authentication, no business logic  
- **Predictability** — stable output for monitoring systems  
- **Automatic discovery** — no manual registration required  
- **Isolation** — health checks do not rely on application state or external services

---

## Summary

This folder defines the health‑check surface for the Camp Fit Fur Dogs API:

- A single GET endpoint reporting service status  
- A registration extension enabling automatic discovery  

These endpoints ensure that external systems can reliably determine whether the
API is operational.
