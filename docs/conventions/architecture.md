# Architecture Conventions

## DDD Layered Architecture

The backend follows a layered architecture with clear boundaries.

Api depends on Application and Infrastructure.  
Application depends on Domain.  
Infrastructure depends on Application and Domain where appropriate.  
All layers may depend on SharedKernel.

Domain must not depend on Application, Infrastructure, or Api.  
Application must not depend on Infrastructure or Api.  
Infrastructure must not depend on Api.

The primary flow is:

Api → Application → Domain  
Application → Infrastructure for persistence and external concerns.

## CQRS Pipelines

Commands and queries are separated.

Commands:

- Implement `ICommand<TResponse>`.
- Are handled by `ICommandHandler<TCommand, TResponse>`.
- Are dispatched via `ICommandDispatcher`.

Queries:

- Implement `IQuery<TResponse>`.
- Are handled by `IQueryHandler<TQuery, TResponse>`.
- Are dispatched via `IQueryDispatcher`.

Dispatchers:

- Resolve validators for the command or query type.
- Run validation before invoking handlers.
- Throw on validation failure.

## Domain Model

The domain model uses:

- **Value Objects**: immutable, equality by components.
- **Entities**: identity and equality by Id.
- **Aggregate Roots**: entities that own a consistency boundary and raise domain events.
- **Aggregate Ids**: value objects wrapping a `Guid`.

Domain events represent significant state changes inside aggregates.  
Aggregates collect domain events which are later dispatched by the domain event dispatcher.

Domain code must not depend on Application, Infrastructure, or Api.  
Business rules live in aggregates, value objects, and domain services where needed.

## Repositories And Unit Of Work

Repositories provide persistence operations for aggregates:

- Get by Id
- Add
- Update
- Delete

The `IUnitOfWork` abstraction coordinates saving changes.  
Infrastructure implements `IUnitOfWork` using the chosen data access technology (for example, EF Core).  
Application code calls `CommitAsync` on the unit of work after successful command handling.

Domain does not know about repositories or unit of work.

## EF Core Conventions

Infrastructure provides base configurations for aggregates.

- Aggregate root configuration maps the aggregate to a table.
- The Id is configured as the key and is not value generated.
- Domain events are ignored and never persisted.
- Derived configurations extend the base configuration to map properties, relationships, and indexes.

The EF Core unit of work implementation:

- Uses the DbContext to save changes.
- Does not embed business logic.
- Is the only layer that talks directly to the database.

## SharedKernel Architecture

SharedKernel contains cross‑cutting building blocks that all product layers may use.

It provides:

- CQRS abstractions and dispatchers.
- Validation pipeline integration.
- DI conventions and auto‑registration helpers.
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`).
- Domain event abstractions and a domain event dispatcher.
- EF Core base classes for aggregate configuration and unit of work.
- Endpoint discovery infrastructure.
- `SharedKernelOptions` for configuring infrastructure and endpoint discovery.
- Architecture guardrails.

Product layers must not reimplement these building blocks.  
They depend on SharedKernel instead.

## Endpoint Discovery

Api endpoints are implemented as classes that implement `IEndpoint`.  
Each endpoint class defines a `Map` method that receives an `IEndpointRouteBuilder`.

SharedKernel.Api:

- Scans assemblies for `IEndpoint` implementations.
- Registers the Api assembly for endpoint discovery.
- Instantiates and maps all discovered endpoints at startup.

This section covers **discovery mechanics only**.  
Behavioral rules for endpoints live in Code Conventions.

## Architecture Guardrails

The following guardrails must be respected:

- Domain does not reference Application, Infrastructure, or Api.
- Application does not reference Infrastructure or Api.
- Infrastructure does not reference Api.
- All layers may reference SharedKernel.

Commands and queries must go through their dispatchers.  
Domain entities and aggregates must not cross the Api boundary.  
Endpoints must resolve identity from the current user service, not from request bodies.

These guardrails should be enforced by tests, analyzers, and reviews.

## Frontend Architecture

The frontend follows a **layer + aggregate** convention that mirrors the backend's aggregate grouping.

Structure: `layer/aggregate/filename` — slice identity is encoded in the filename, not a subfolder.

Frontend layers:

- `api/` — server-call functions (one per slice).
- `components/` — presentational React components.
- `lib/` — pure logic and action functions.
- `app/` — Next.js routing layer (untouched by this convention).

Within each layer, files are grouped by aggregate:

- `api/dogs/getDogProfile.ts`
- `components/dogs/DogProfileCard.tsx`
- `lib/dogs/dogProfileActions.ts`

Shared infrastructure (e.g., API client) lives in `lib/api/` with no aggregate subfolder.

Slice subfolders are introduced only when an aggregate accumulates 10+ files in a single layer.

The `app/` directory is owned by Next.js routing conventions and is not restructured.

The `test/` directory mirrors `src/` exactly.
