# Code Conventions

## Backend Conventions

Backend code follows the layered architecture and CQRS pipelines.

- Commands and queries live in Application slices.
- Handlers encapsulate use‑case logic.
- Domain logic lives in aggregates, value objects, and domain services.
- Infrastructure implements persistence and external integrations.

Handlers should be thin coordinators that:

- Validate inputs (via validators).
- Load aggregates or other state.
- Invoke domain behavior.
- Persist changes via repositories and unit of work.

Domain logic must not leak into endpoints, controllers, or infrastructure plumbing.

## Endpoint Routing

Endpoints are **slice‑scoped**:

- Each vertical slice defines its own endpoint class.
- Endpoints live alongside the slice’s commands, queries, and DTOs.

Endpoints:

- Implement `IEndpoint`.
- Expose a `Map(IEndpointRouteBuilder app)` method.
- Register routes explicitly (no attribute routing, no reflection‑based routing).

Behavioral rules:

- Endpoints are thin.
- They may only perform:
  - DTO binding.
  - Shape validation.
  - Authorization.
  - Dispatching commands or queries.
  - Returning DTOs.
- They must not contain business logic.

Identity:

- Endpoints never accept identity from the request body.
- Identity is resolved via `ICurrentUserService`.

Data:

- Endpoints return DTOs, not domain entities.
- Domain entities never cross the Api boundary.

Routing:

- Routes map one‑to‑one to commands or queries.
  - `POST /api/dogs` → `RegisterDogCommand`
  - `GET /api/dogs/{id}` → `GetDogQuery`
- Routing must be explicit; no monolithic `Endpoints.cs` files.

Testability:

- Endpoint logic should be extractable into pure functions where possible.
- Endpoints must be testable via unit or integration tests.

## Identity And Security

Identity:

- Identity is always resolved via `ICurrentUserService`.
- No endpoint or handler should trust identity from the request body.
- Application code should depend on the abstraction, not on transport‑specific details.

Security:

- Never store plaintext passwords.
- Use strong hashing (see Password Hashing section).
- Apply authorization checks at endpoints and, where necessary, in application handlers.
- Avoid duplicating security logic; centralize policies where possible.

## Password Hashing

Password hashing:

- Algorithm: BCrypt.
- Work factor: 11.

Implementation:

- Located in a `PasswordHash` value object in the Domain.
- Entry points:
  - `Create` — from a raw password.
  - `Verify` — compare raw password to stored hash.
  - `From` — construct from an existing hash string.

Rules:

- Application and Infrastructure code must use the `PasswordHash` value object.
- No custom hashing logic outside this value object.
- No plaintext passwords persisted or logged.

## SharedKernel Usage

SharedKernel provides:

- CQRS abstractions and dispatchers.
- Validation pipeline integration.
- DI conventions and auto‑registration.
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`).
- Domain event abstractions and dispatcher.
- EF Core base classes.
- Endpoint discovery infrastructure.
- `SharedKernelOptions`.

Product layers:

- Must not reimplement these patterns.
- Should depend on SharedKernel types and extensions.
- Should keep SharedKernel free of product‑specific concerns.

## Frontend Conventions

Frontend stack:

- React with Next.
- Node version: 22.

Layout:

- `frontend/`
  - `src/`
    - `api/<aggregate>/` — server-call functions grouped by aggregate.
    - `components/<aggregate>/` — presentational components grouped by aggregate.
    - `lib/<aggregate>/` — pure logic and action functions grouped by aggregate.
    - `lib/api/` — shared infrastructure (API client, `CommandResult`, `QueryResult`).
    - `lib/hooks/` — shared hooks (`useApiQuery`, `useCommand`, `useConfirmDialog`).
- `hooks/<aggregate>/` — behavioral hooks scoped to an aggregate (e.g., `useRemoveDog`).
- `lib/components/` — cross-aggregate presentational components (e.g., `ConfirmDialog`, `ActionsCard`).
    - `app/` — Next.js routing layer.
  - `test/` — mirrors `src/` structure.

File naming:

- `api/`: `camelCaseVerb.ts` (e.g., `getDogProfile.ts`).
- `components/`: `PascalCase.tsx` (e.g., `DogProfileCard.tsx`).
- `lib/`: `camelCase.ts` (e.g., `dogProfileActions.ts`).
- `app/`: `page.tsx` (Next.js convention).

Testing:

- Vitest v3.
- React Testing Library.
- `@testing-library/jest-dom` assertions.

Principles:

- Follow TDD where practical.
- Keep components small and focused.
- Prefer composition over inheritance.
- Keep side‑effects at the edges (hooks, data‑fetching boundaries).

## Testing Conventions

Tests must be:

- Fast.
- Deterministic.
- Isolated.

Backend:

- Unit tests cover domain logic, application handlers, and small infrastructure pieces.
- Integration tests cover persistence, endpoint behavior, and cross‑layer flows.
- Endpoint tests should verify routing, binding, authorization, and dispatch behavior.

Frontend:

- Component tests verify rendering and behavior.
- Use React Testing Library and jest‑dom for user‑centric assertions.
- Avoid testing implementation details.

Naming:

- Tests should describe behavior, not implementation.
- Prefer scenario‑style names that read like specifications.

