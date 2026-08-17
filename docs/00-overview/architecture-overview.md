# Architecture Overview

The repository separates a product application from a reusable platform. That split is visible in the source tree and in the startup composition of the API.

## Repository shape

```text
src/
├── CampFitFurDogs/
│   ├── Api
│   ├── Application
│   ├── Domain
│   └── Infrastructure
├── Frank/
│   ├── Core/
│   ├── Identity/
│   └── Testing/
```

## Product layer

The `CampFitFurDogs` project owns the concrete business domain: dog profiles, ownership relationships, API endpoints, and product-specific behavior.

## Platform layer

`Frank` contains shared system primitives and reusable services, including:

- API hosting and endpoint composition
- application abstractions for commands and queries
- domain helpers and base contracts
- EF Core integration patterns
- identity, user, and session handling
- test utilities for realistic endpoint simulation

## Dependency direction

The dependency pattern is intentionally inward-facing:

- API composes modules and routes
- application coordinates use cases
- domain holds invariants and business rules
- infrastructure implements persistence and external adapters

## Practical effect

Product code stays focused on business behavior while the platform handles cross-cutting concerns and common infrastructure patterns.
