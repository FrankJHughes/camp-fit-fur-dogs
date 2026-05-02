# Architecture Conventions

This document defines the architectural boundaries, hosting model, layering rules, and cross‑cutting conventions for the Camp Fit Fur Dogs system.  
It is the canonical source of truth for architectural decisions and must be followed by all code, documentation, and automation.

---

# DDD Layered Architecture

The backend follows a layered architecture with strict boundaries.

Api depends on Application and Infrastructure.  
Application depends on Domain.  
Infrastructure depends on Application and Domain where appropriate.  
All layers may depend on SharedKernel.

Rules:

- **Domain** must not depend on Application, Infrastructure, or Api.  
- **Application** must not depend on Infrastructure or Api.  
- **Infrastructure** must not depend on Api.  
- **Api** must not contain business logic.  
- **SharedKernel** is the only allowed cross‑layer dependency.

Primary flow:

```
Api → Application → Domain
Application → Infrastructure (persistence, external concerns)
```

---

# CQRS Pipelines

Commands and queries are separated.

## Commands
- Implement `ICommand<TResponse>`.
- Handled by `ICommandHandler<TCommand, TResponse>`.
- Dispatched via `ICommandDispatcher`.

## Queries
- Implement `IQuery<TResponse>`.
- Handled by `IQueryHandler<TQuery, TResponse>`.
- Dispatched via `IQueryDispatcher`.

## Dispatchers
- Resolve validators for the command or query type.
- Run validation before invoking handlers.
- Throw on validation failure.
- Ensure consistent pipeline behavior across all use‑cases.

---

# Domain Model

The domain model uses:

- **Value Objects** — immutable, equality by components.  
- **Entities** — identity and equality by Id.  
- **Aggregate Roots** — own consistency boundaries and raise domain events.  
- **Aggregate Ids** — value objects wrapping a `Guid`.

Domain events represent significant state changes inside aggregates.  
Aggregates collect domain events which are later dispatched by the domain event dispatcher.

Rules:

- Domain code must not depend on Application, Infrastructure, or Api.  
- Business rules live in aggregates, value objects, and domain services.  
- Domain events are raised inside aggregates and dispatched after persistence.

---

# Repositories and Unit of Work

Repositories provide persistence operations for aggregates:

- Get by Id  
- Add  
- Update  
- Delete  

`IUnitOfWork` coordinates saving changes.

Infrastructure implements:

- Repositories using EF Core  
- Unit of Work using DbContext  

Application code calls `CommitAsync` after successful command handling.  
Domain does not know about repositories or unit of work.

---

# EF Core Conventions

Infrastructure provides base configurations for aggregates.

- Aggregate root configuration maps the aggregate to a table.  
- Id is configured as the key and is not value‑generated.  
- Domain events are ignored and never persisted.  
- Derived configurations extend the base configuration to map properties, relationships, and indexes.

Unit of Work:

- Uses DbContext to save changes.  
- Contains no business logic.  
- Is the only layer that talks directly to the database.

---

# SharedKernel Architecture

SharedKernel contains cross‑cutting building blocks used by all product layers:

- CQRS abstractions and dispatchers  
- Validation pipeline integration  
- DI conventions and auto‑registration helpers  
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`)  
- Domain event abstractions and dispatcher  
- EF Core base classes for aggregate configuration and unit of work  
- Endpoint discovery infrastructure  
- `SharedKernelOptions` for configuring infrastructure and endpoint discovery  
- Architecture guardrails  

Product layers must not reimplement these building blocks.  
They depend on SharedKernel instead.

---

# Endpoint Discovery

Api endpoints are implemented as classes that implement `IEndpoint`.  
Each endpoint defines a `Map` method that receives an `IEndpointRouteBuilder`.

SharedKernel.Api:

- Scans assemblies for `IEndpoint` implementations  
- Registers the Api assembly for endpoint discovery  
- Instantiates and maps all discovered endpoints at startup  

This section covers discovery mechanics only.  
Behavioral rules for endpoints live in Code Conventions.

---

# Architecture Guardrails

The following guardrails must be respected:

- Domain does not reference Application, Infrastructure, or Api.  
- Application does not reference Infrastructure or Api.  
- Infrastructure does not reference Api.  
- All layers may reference SharedKernel.  

Additional rules:

- Commands and queries must go through their dispatchers.  
- Domain entities and aggregates must not cross the Api boundary.  
- Endpoints must resolve identity from the current user service, not from request bodies.  
- Guardrail tests enforce layering, purity, and dependency rules.

---

# Frontend Architecture

The frontend follows a **layer + aggregate** convention mirroring backend aggregate grouping.

Structure: `layer/aggregate/filename`  
Slice identity is encoded in the filename, not a subfolder.

Frontend layers:

- `api/` — server‑call functions (one per slice)  
- `components/` — presentational React components  
- `lib/` — pure logic and action functions  
- `hooks/` — behavioral hooks orchestrating UI state, API calls, navigation  
- `app/` — Next.js routing layer (untouched by this convention)  

Examples:

- `api/dogs/getDogProfile.ts`  
- `components/dogs/DogProfileCard.tsx`  
- `lib/dogs/dogProfileActions.ts`  
- `hooks/dogs/useRemoveDog.ts`  
- `lib/components/ConfirmDialog.tsx`  

Shared infrastructure lives in:

- `lib/api/` (API client, `CommandResult`, `QueryResult`)  
- `lib/hooks/` (`useApiQuery`, `useCommand`, `useConfirmDialog`)  
- `lib/components/` (`ConfirmDialog`, `ActionsCard`)  

Slice subfolders appear only when an aggregate accumulates 10+ files in a single layer.  
The `test/` directory mirrors `src/` exactly.

---

# Hosting & Deployment Architecture

## Overview

The system uses a cloud‑hosted deployment model that separates the API, database, and frontend into independently deployable units.  
Each component is deployed to a platform that matches its operational requirements while keeping cost at or near zero.

---

# API Hosting (US‑140)

The API is hosted on **Render** as a Dockerized .NET 10 Web Service.

### Characteristics

- Containerized runtime using a multi‑stage Dockerfile at  
  `src/CampFitFurDogs.Api/Dockerfile`
- HTTPS termination handled by Render  
- Health check endpoint: `/health`  
- Environment variables injected at runtime  
- Automatic deployment on push to `main`  
- Free tier cold‑start behavior (sleeps after 15 minutes of inactivity)  

The API is stateless and horizontally scalable.  
All state is stored in the database.

---

# Database Hosting (US‑141)

The application database is hosted on **Neon** using:

- A persistent production branch  
- Ephemeral preview branches for PRs  

### Characteristics

- PostgreSQL  
- Connection strings provided via environment variables  
- Branch‑per‑PR workflow for isolated integration testing  
- Automatic expiration of preview branches  
- SSL‑required connections  

The application accesses the database exclusively through EF Core and SharedKernel abstractions.

---

# Frontend Hosting (US‑139)

The Next.js frontend is hosted separately from the API.

### Characteristics

- Hosted on a static‑optimized platform (Vercel or Render Static Sites)  
- Communicates with the API over HTTPS  
- CORS configured in the API to allow the frontend host  
- Deployment triggered on push to `main`  

---

# PR Preview Architecture (Neon + Render)

Pull requests targeting `main` automatically create:

## 1. Neon Ephemeral Branch
- Named `preview/pr-<number>-<branch>`  
- Connection string exported as `PREVIEW_DB_CONNECTION_STRING`  
- EF Core migrations applied before preview deployment  

## 2. Render PR Preview Instance
- Git‑backed  
- Service name: **`campfitfurdogsapi`**  
- Configured via `render.yaml`  
- Injects `PREVIEW_DB_CONNECTION_STRING` using `previewValue`  
- Preview URL:  
  `https://campfitfurdogsapi-pr-<number>.onrender.com`

## 3. Integration Tests
- Run against the live preview instance  
- Require `/health` to return 200  

## 4. Cleanup
- Neon branch deleted when PR closes  
- Render preview instance deleted automatically  

This ensures each PR has a fully isolated, production‑like environment.

---

# Summary

This architecture ensures:

- strict layering  
- predictable deployments  
- isolated PR environments  
- clean domain modeling  
- safe and testable workflows  
- consistent hosting behavior  

All code, scripts, and documentation must follow these conventions.
