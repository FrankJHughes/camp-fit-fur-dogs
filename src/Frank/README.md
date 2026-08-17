# Frank — Platform Root

The **Frank** repository is a modular, multi‑project .NET platform built around
explicit architectural boundaries, vertical slices, and deterministic testing.
This root folder contains all core platform libraries, including API hosting,
application abstractions, domain primitives, identity subsystem, infrastructure
adapters, and the testing harness.

Every subsystem is fully isolated, documented, and independently testable.

---

## High‑Level Structure

```
Frank/
│
├── Core/
│   ├── Api/
│   ├── Application/
│   ├── Domain/
│   ├── EntityFrameworkCore/
│   ├── Infrastructure/
│   └── README.md
│
├── Identity/
│   ├── Api/
│   ├── Application/
│   ├── Domain/
│   ├── EntityFrameworkCore/
│   ├── Infrastructure/
│   └── README.md
│
└── Testing/
    ├── Contexts/
    ├── Endpoints/
    ├── Factories/
    ├── AssemblyMarker.cs
    └── README.md
```

Each subsystem contains its own internal structure and README files.

---

# Core

The **Core** directory contains the foundational building blocks of the Frank
platform. It is divided into several sub‑projects, each with a precise
responsibility.

---

## Core.Api

Provides the hosting surface for API endpoints:

- Endpoint routing extensions  
- Hosting modules  
- Middleware (CORS, exceptions, observations, security headers)  
- Platform‑level features (logging, swagger, CORS, etc.)  
- API assembly marker  
- All components are environment‑agnostic and infrastructure‑free

Subfolders include:

- **Endpoints** — endpoint mapping extensions  
- **HostingModules** — hosting engine + configuration source/provider  
- **Middleware** — CORS, exceptions, observations, security headers  
- **Platform** — logging, swagger, CORS, platform‑level DI  
- Each folder contains its own README.md

---

## Core.Application

Contains application‑level abstractions and orchestration logic:

- CQRS (commands, queries, dispatchers)  
- Domain events  
- Endpoint abstractions  
- Environment variable abstraction  
- Exception handling contracts  
- Hosting module contracts  
- Immutable context builder framework  
- Observation abstractions  
- Registration + scanning system  
- Application assembly marker  
- DI registration helpers

Subfolders include:

- **Abstractions** — all core contracts  
- **Cqrs** — command/query dispatchers  
- **DomainEvents** — event dispatching  
- **ImmutableContexts** — builder framework  
- **Registration** — discovery + scanning system  
- **Users**, **Sessions**, **Callback**, **Oidc** — identity‑related abstractions  
- Each folder contains its own README.md

---

## Core.Domain

Pure domain model:

- Aggregates  
- Entities  
- Value objects  
- Domain events  
- Domain exceptions  
- Domain assembly marker

Subfolders include:

- **Exceptions** — domain‑specific exception types  
- Each folder contains its own README.md

This layer is **pure C#**, with no dependencies on ASP.NET Core or infrastructure.

---

## Core.EntityFrameworkCore

Database integration layer:

- EF Core configurations  
- EF‑based Unit of Work  
- EF assembly marker  
- Infrastructure‑specific persistence logic

Subfolders include:

- **Configurations** — aggregate root configuration  
- **UnitOfWork** — EF‑based UoW  
- Each folder contains its own README.md

---

## Core.Infrastructure

Environment‑specific adapters:

- Clock  
- Environment variables  
- Exception handler registry  
- Observation context + metrics  
- Infrastructure assembly marker  
- DI registration helpers

Subfolders include:

- **Clock**  
- **EnvironmentVariables**  
- **ExceptionHandlers**  
- **Observations**  
- Each folder contains its own README.md

---

# Identity

The **Identity** subsystem is a fully isolated vertical slice supporting:

- Auth0 OIDC login  
- Identity resolution  
- Session management  
- User creation + lookup  
- Authorization middleware  
- Session validation  
- Identity‑specific EF Core persistence  
- Identity‑specific infrastructure adapters

Identity mirrors the structure of Core:

```
Identity/
│
├── Api/
├── Application/
├── Domain/
├── EntityFrameworkCore/
└── Infrastructure/
```

---

## Identity.Api

Contains all API‑facing identity components:

- Endpoint implementations (login URL, callback, logout, identity)  
- Authentication + authorization DI  
- Identity‑specific middleware  
- Platform‑level identity extensions  
- Settings (frontend, OIDC)  
- API assembly marker

Subfolders include:

- **Abstractions** — response models  
- **Authentication**  
- **Authorization**  
- **Endpoints**  
- **Middleware** — authorization, observations, session validation  
- **Platform**  
- **Settings**  
- Each folder contains its own README.md

---

## Identity.Application

Contains identity‑specific application logic:

- OIDC callback pipeline  
- Token validation  
- User info retrieval  
- Session creation, lookup, revocation  
- User creation + lookup  
- Identity unit of work  
- Identity assembly marker

Subfolders include:

- **Abstractions** — audit logging, OIDC, sessions, users  
- **Callback** — OIDC + save pipelines  
- **Oidc** — token clients, validators, user info  
- **Sessions** — create/get/revoke  
- **Users** — create/get  
- Each folder contains its own README.md

---

## Identity.Domain

Pure identity domain model:

- Session aggregate  
- User aggregate  
- Value objects (Email, FirstName, LastName, PhoneNumber, ExternalId, etc.)  
- Domain exceptions  
- Domain assembly marker

Subfolders include:

- **Sessions**  
- **Users**  
- Each folder contains its own README.md

---

## Identity.EntityFrameworkCore

Identity‑specific EF Core integration:

- DbContext  
- DbContext factory  
- Migrations  
- Session + user persistence  
- Identity EF assembly marker  
- Identity EF DI registration

Subfolders include:

- **DbContexts**  
- **Migrations**  
- **Sessions**  
- **Users**  
- **Settings**  
- **UnitOfWork**  
- Each folder contains its own README.md

---

## Identity.Infrastructure

Identity‑specific environment adapters:

- Auth0 OIDC clients  
- Audit logging  
- Current user resolution  
- OIDC settings  
- Infrastructure assembly marker  
- DI registration helpers

Subfolders include:

- **AuditLogging**  
- **Auth0**  
- **Settings**  
- **Users**  
- Each folder contains its own README.md

---

# Testing

The **Testing** subsystem provides a deterministic, mutation‑driven integration
testing harness.

Subfolders include:

- **Contexts** — immutable configuration objects  
- **Factories** — host builders applying context mutations  
- **Endpoints** — diagnostic test‑only endpoints  
- **AssemblyMarker.cs** — stable anchor for assembly discovery  
- Each folder contains its own README.md

Testing supports:

- Environment simulation  
- Authentication simulation  
- PostgreSQL test containers  
- Fake service injection  
- Cookie rewrite behavior  
- Diagnostic endpoints  
- Deterministic host construction

---

# Summary

The **Frank** root folder defines a clean, modular, vertical‑slice‑driven
architecture with:

- Pure domain logic  
- Explicit application boundaries  
- Thin, replaceable infrastructure  
- A fully isolated identity subsystem  
- Deterministic testing harness  
- Comprehensive documentation at every level  

Frank is built for clarity, composability, and long‑term maintainability.

