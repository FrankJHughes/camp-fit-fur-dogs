# Code Conventions

This document defines the coding‑level rules for backend and frontend code.  
It complements the architectural conventions and ensures all code remains consistent, testable, and aligned with the system’s guardrails.

---

# Backend Conventions

Backend code follows the layered architecture and CQRS pipelines defined in `architecture.md`.

## Layer Responsibilities

- **Api** — endpoints, DTO binding, authorization, dispatching commands/queries.
- **Application** — use‑case orchestration, validation, domain interaction.
- **Domain** — business rules, aggregates, value objects, domain events.
- **Infrastructure** — persistence, external integrations, EF Core, repositories, unit of work.
- **SharedKernel** — cross‑cutting primitives, CQRS abstractions, endpoint discovery, DI conventions.

Domain logic must never leak into Api, Application, or Infrastructure plumbing.

---

# CQRS Handler Conventions

Commands and queries live in Application slices.

Handlers must be thin coordinators:

- Validate inputs (via validators).
- Load aggregates or state.
- Invoke domain behavior.
- Persist changes via repositories and unit of work.
- Publish domain events (via dispatcher).

Handlers must not:

- Contain business rules.
- Perform persistence logic directly.
- Bypass the dispatcher pipeline.

---

# Endpoint Conventions

Endpoints are **slice‑scoped** and discovered automatically via SharedKernel.Api.

## Structure

- Each vertical slice defines its own endpoint class.
- Endpoints live alongside the slice’s commands, queries, and DTOs.
- Endpoints implement `IEndpoint` and expose:

```
void Map(IEndpointRouteBuilder app)
```

## Behavioral Rules

Endpoints may only perform:

- DTO binding
- Shape validation
- Authorization
- Dispatching commands or queries
- Returning DTOs

Endpoints must not:

- Contain business logic
- Return domain entities
- Reference Infrastructure directly
- Accept identity from the request body

Identity is resolved exclusively via `ICurrentUserService`.

## Routing Rules

- Routes map one‑to‑one to commands or queries.
- Routing must be explicit inside each endpoint class.
- No attribute routing.
- No monolithic `Endpoints.cs` files.

## Testability

- Endpoint logic should be extractable into pure functions where possible.
- Endpoints must be testable via unit or integration tests.

---

# Identity and Security

## Identity

- Always resolved via `ICurrentUserService`.
- Never accepted from request bodies.
- Application code depends only on the abstraction.

## Security

- Never store plaintext passwords.
- Use BCrypt with work factor 11.
- Authorization checks belong in endpoints and, when necessary, in handlers.
- Avoid duplicating security logic; centralize policies.

---

# Password Hashing

Password hashing is implemented via the `PasswordHash` value object in the Domain.

Entry points:

- `PasswordHash.Create(rawPassword)`
- `PasswordHash.Verify(rawPassword)`
- `PasswordHash.From(existingHash)`

Rules:

- No custom hashing logic outside this value object.
- No plaintext passwords persisted or logged.
- Application and Infrastructure must use the value object.

---

# SharedKernel Usage

SharedKernel provides:

- CQRS abstractions and dispatchers
- Validation pipeline integration
- DI conventions and auto‑registration
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`)
- Domain event abstractions and dispatcher
- EF Core base classes
- Endpoint discovery infrastructure
- `SharedKernelOptions`

Product layers:

- Must not reimplement these patterns.
- Must depend on SharedKernel types and extensions.
- Must keep SharedKernel free of product‑specific concerns.

---

# PR Preview Coding Rules

The system uses Neon + Render PR Previews. Code must be preview‑safe.

## Requirements

- API must expose `/health` for readiness checks.
- API must not assume a persistent database.
- Migrations must run cleanly against empty Neon branches.
- Connection strings must be read from `ConnectionStrings__DefaultConnection`.
- No environment‑specific branching in code.
- No hardcoded URLs or secrets.

## Behavior

- All preview instances use ephemeral Neon branches.
- All preview instances run the same code as production.
- Code must tolerate cold starts and transient connections.

---

# EF Core Conventions

Infrastructure provides base configurations for aggregates.

Rules:

- Aggregate root maps to a table.
- Id is the key and not value‑generated.
- Domain events are ignored by EF Core.
- Derived configurations map properties, relationships, and indexes.
- DbContext is the only layer that talks directly to the database.

Unit of Work:

- Uses DbContext to save changes.
- Contains no business logic.
- Coordinates domain event dispatch after persistence.

---

# Frontend Conventions

The frontend follows a **layer + aggregate** structure that mirrors backend slices.

## Directory Structure

```
frontend/
  src/
    api/<aggregate>/...
    components/<aggregate>/...
    lib/<aggregate>/...
    hooks/<aggregate>/...
    lib/api/...
    lib/hooks/...
    lib/components/...
    app/...
  test/...
```

### Layer Responsibilities

- `api/` — server‑call functions (one per slice)
- `components/` — presentational components
- `lib/` — pure logic and action functions
- `hooks/` — behavioral hooks orchestrating UI state, API calls, navigation
- `app/` — Next.js routing layer (untouched by conventions)

### Shared Infrastructure

- `lib/api/` — API client, `CommandResult`, `QueryResult`
- `lib/hooks/` — shared hooks (`useApiQuery`, `useCommand`, `useConfirmDialog`)
- `lib/components/` — cross‑aggregate components (`ConfirmDialog`, `ActionsCard`)

### Naming Conventions

- `api/`: `camelCaseVerb.ts` (e.g., `getDogProfile.ts`)
- `components/`: `PascalCase.tsx` (e.g., `DogProfileCard.tsx`)
- `lib/`: `camelCase.ts` (e.g., `dogProfileActions.ts`)
- `app/`: `page.tsx` (Next.js convention)

### Dynamic Route Safety

PowerShell treats `[id]` as a wildcard.  
All file operations involving dynamic route folders must use:

```
-LiteralPath
```

Never use `-Path` with bracket characters.

### Testing

- Vitest v3 multi‑project setup
- React Testing Library
- `@testing-library/jest-dom` assertions
- `test/` mirrors `src/` exactly

Rules:

- Keep components small and focused.
- Prefer composition over inheritance.
- Keep side‑effects at the edges (hooks, data‑fetching boundaries).

---

# Testing Conventions

Tests must be:

- Fast
- Deterministic
- Isolated

## Backend Testing

- Unit tests cover domain logic, application handlers, and small infrastructure pieces.
- Integration tests cover persistence, endpoint behavior, and cross‑layer flows.
- Endpoint tests verify routing, binding, authorization, and dispatch behavior.

## Frontend Testing

- Component tests verify rendering and behavior.
- Use React Testing Library and jest‑dom.
- Avoid testing implementation details.

## Naming

- Tests should describe behavior, not implementation.
- Prefer scenario‑style names that read like specifications.

---

# Summary

These conventions ensure:

- Clean layering  
- Predictable behavior  
- Preview‑safe code  
- Consistent frontend structure  
- Guardrail‑compliant endpoints  
- Testable, maintainable slices  

All code must follow these rules.
