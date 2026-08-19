# Host Layer Overview

The **CampFitFurDogs.Host** project is the composition root of the Camp Fit Fur Dogs platform.  
It contains the executable entry point (`Program.cs`) and is responsible for configuring the web host, applying hosting‑environment logic, activating platform middleware, registering services, and mapping all API endpoints.

The Host layer does **not** contain domain logic, application logic, or API logic.  
Its sole responsibility is orchestration.

---

## Responsibilities

The Host project:

- builds the `WebApplicationBuilder`
- applies hosting modules (environment‑specific configuration)
- registers platform services (API, Identity, Core)
- activates middleware pipelines
- maps all endpoints under `/api`
- runs the application

This separation keeps the API assembly pure and host‑agnostic.

---

## Why the Host Layer Exists

Historically, the API project contained:

- `Program.cs`
- hosting configuration
- environment adaptation
- middleware pipeline setup

This tightly coupled hosting concerns with API concerns.

Extracting the Host layer provides:

- **clean separation of concerns**
- **reusable API assembly**
- **environment‑specific hosting logic**
- **consistent startup across environments**
- **better testability and composition**

---

## High‑Level Startup Flow

The Host project performs the following steps:

1. **Create WebApplicationBuilder**
2. **Apply hosting modules**
3. **Register platform services**
4. **Register API + Identity endpoints**
5. **Configure middleware pipeline**
6. **Run the application**

Each step is delegated to platform extension methods to keep `Program.cs` minimal and declarative.

---

## Relationship to Other Layers

```
Host
 ├── Api
 │     ├── Endpoints
 │     ├── DTOs
 │     ├── Validators
 │     └── Exception Handlers
 ├── Application
 │     ├── Commands
 │     ├── Queries
 │     └── Services
 ├── Domain
 │     ├── Aggregates
 │     └── Value Objects
 └── Infrastructure
       ├── DbContexts
       ├── Readers/Writers
       └── Unit of Work
```

The Host layer sits at the top and composes all other layers.

---

## Summary

The Host project is the operational entry point of the platform.  
It configures hosting, activates platform middleware, registers services, maps endpoints, and runs the application.

It is the top of the dependency chain:

**Host → Api → Application → Domain → Infrastructure**
