# Architecture Conventions

This document defines the architectural boundaries, hosting model, layering rules, and cross‑cutting conventions for the Camp Fit Fur Dogs system.  
It is the canonical source of truth for architectural decisions and must be followed by all code, documentation, and automation.

---

### DDD Layered Architecture

The backend follows a layered architecture with strict boundaries.

- **Api** depends on **Application** and **Infrastructure**.  
- **Application** depends on **Domain**.  
- **Infrastructure** depends on **Application** and **Domain** where appropriate.  
- All layers may depend on **SharedKernel**.

**Rules**

- **Domain** must not depend on Application, Infrastructure, or Api.  
- **Application** must not depend on Infrastructure or Api.  
- **Infrastructure** must not depend on Api.  
- **Api** must not contain business logic.  
- **SharedKernel** is the only allowed cross‑layer dependency.

**Primary flow**

```text
Api → Application → Domain
Application → Infrastructure (persistence, external concerns)
```

---

### CQRS Pipelines

Commands and queries are separated and follow consistent pipeline behavior.

**Commands**

- Implement `ICommand<TResponse>`.  
- Handled by `ICommandHandler<TCommand, TResponse>`.  
- Dispatched via `ICommandDispatcher`.

**Queries**

- Implement `IQuery<TResponse>`.  
- Handled by `IQueryHandler<TQuery, TResponse>`.  
- Dispatched via `IQueryDispatcher`.

**Dispatchers**

- Resolve validators for the command or query type.  
- Run validation before invoking handlers.  
- Throw on validation failure.  
- Ensure consistent pipeline behavior across all use‑cases.

---

### Domain Model

The domain model uses:

- **Value Objects** — immutable, equality by components.  
- **Entities** — identity and equality by Id.  
- **Aggregate Roots** — own consistency boundaries and raise domain events.  
- **Aggregate Ids** — value objects wrapping a `Guid`.

**Rules**

- Domain code must not depend on Application, Infrastructure, or Api.  
- Business rules live in aggregates, value objects, and domain services.  
- Domain events are raised inside aggregates and dispatched after persistence.

---

### Repositories and Unit of Work

Repositories provide persistence operations for aggregates:

- Get by Id; Add; Update; Delete.

`IUnitOfWork` coordinates saving changes.

**Infrastructure responsibilities**

- Implement repositories using EF Core.  
- Implement Unit of Work using `DbContext`.  
- Ensure `CommitAsync` is called by Application after successful command handling.

**Rules**

- Domain does not know about repositories or unit of work.  
- Unit of Work contains no business logic.

---

### EF Core Conventions

Infrastructure provides base configurations for aggregates.

- Aggregate root configuration maps the aggregate to a table.  
- Id is configured as the key and is not value‑generated.  
- Domain events are ignored and never persisted.  
- Derived configurations extend the base configuration to map properties, relationships, and indexes.

**Unit of Work**

- Uses `DbContext` to save changes.  
- Is the only layer that talks directly to the database.

---

### SharedKernel Architecture

**SharedKernel** contains cross‑cutting building blocks used by all product layers:

- CQRS abstractions and dispatchers  
- Validation pipeline integration  
- DI conventions and auto‑registration helpers  
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`)  
- Domain event abstractions and dispatcher  
- EF Core base classes for aggregate configuration and unit of work  
- Endpoint discovery infrastructure  
- `SharedKernelOptions` for configuring infrastructure and endpoint discovery  
- Architecture guardrails

**Rules**

- Product layers must not reimplement these building blocks.  
- Use SharedKernel for common primitives and infrastructure helpers.

---

### Endpoint Discovery

Api endpoints are implemented as classes that implement `IEndpoint`.  
Each endpoint defines a `Map` method that receives an `IEndpointRouteBuilder`.

**Behavior**

- SharedKernel.Api scans assemblies for `IEndpoint` implementations.  
- The Api assembly is registered for endpoint discovery.  
- All discovered endpoints are instantiated and mapped at startup.

This section covers discovery mechanics only. Behavioral rules for endpoints live in Code Conventions.

---

### Architecture Guardrails

The following guardrails must be respected:

- Domain does not reference Application, Infrastructure, or Api.  
- Application does not reference Infrastructure or Api.  
- Infrastructure does not reference Api.  
- All layers may reference SharedKernel.

**Additional rules**

- Commands and queries must go through their dispatchers.  
- Domain entities and aggregates must not cross the Api boundary.  
- Endpoints must resolve identity from the current user service, not from request bodies.  
- Guardrail tests enforce layering, purity, and dependency rules.

---

### Frontend Architecture

The frontend follows a layer + aggregate convention mirroring backend aggregate grouping.

**Structure**

- `layer/aggregate/filename`  
- Slice identity is encoded in the filename, not a subfolder.

**Frontend layers**

- `api/` — server‑call functions (one per slice)  
- `components/` — presentational React components  
- `lib/` — pure logic and action functions  
- `hooks/` — behavioral hooks orchestrating UI state, API calls, navigation  
- `app/` — Next.js routing layer (untouched by this convention)

**Shared infrastructure**

- `lib/api/` (API client, `CommandResult`, `QueryResult`)  
- `lib/hooks/` (`useApiQuery`, `useCommand`, `useConfirmDialog`)  
- `lib/components/` (`ConfirmDialog`, `ActionsCard`)

**Conventions**

- Slice subfolders appear only when an aggregate accumulates 10+ files in a single layer.  
- The `test/` directory mirrors `src/` exactly.

---

### Hosting and Deployment Architecture

#### Overview

The system uses a cloud‑hosted deployment model that separates the API, database, and frontend into independently deployable units. Each component is deployed to a platform that matches its operational requirements while keeping cost at or near zero.

---

### API Hosting

**Platform**

- Hosted on **Render** as a Dockerized .NET 10 Web Service.  
- Containerized runtime using a multi‑stage Dockerfile at `src/CampFitFurDogs.Api/Dockerfile`.  
- HTTPS termination handled by Render.

**Runtime characteristics**

- Health check endpoint: `/health`.  
- Environment variables injected at runtime.  
- **Main branch** deploys automatically on push (Render Git‑backed).  
- **Free tier cold‑start behavior**: services sleep after inactivity and may require warmup on first request.

**Operational notes**

- The API is stateless and horizontally scalable.  
- All state is stored in the database.  
- The API must tolerate transient 502s and route propagation delays during Render deploys.

---

### Database Hosting

**Platform**

- Hosted on **Neon** (PostgreSQL).

**Model**

- Persistent production branch for main.  
- Ephemeral preview branches for PRs.

**Characteristics**

- Connection strings provided via environment variables.  
- Branch‑per‑PR workflow for isolated integration testing.  
- Automatic expiration of preview branches.  
- SSL‑required connections.  
- The application accesses the database exclusively through EF Core and SharedKernel abstractions.

---

### Frontend Hosting

**Platform**

- Hosted on a static‑optimized platform (Vercel or Render Static Sites).

**Characteristics**

- Communicates with the API over HTTPS.  
- CORS configured in the API to allow the frontend host.  
- Deployment triggered on push to `main`.

---

### PR Preview Architecture

This section documents the **actual** PR Preview orchestration used by CI and Render.

**Key principle**

- Render PR Previews are configured in **Manual** mode. The presence of the `render-preview` label on a PR is the deployment switch.

**Lifecycle**

1. **Label removed** → Render destroys the preview instance.  
2. **Workflow ensures teardown** → waits for `/health` to return `404` for **3 consecutive** checks (timeout 300s).  
3. **Workflow provisions a fresh Neon branch** → branch named `pr-<number>` (workflow uses `pr-<number>` as the canonical preview branch name).  
4. **Workflow converts Neon URI to Npgsql** → composite action normalizes `postgres://...` into an Npgsql connection string.  
5. **Workflow applies EF Core migrations** → `dotnet ef database update` runs in CI using the converted connection string.  
6. **Infrastructure Integration Tests run** → run against the fresh Neon preview DB; tests must not run migrations.  
7. **Workflow uploads DB connection artifact** → `db-conn.txt` is stored as an artifact for debugging.  
8. **Workflow adds `render-preview` label** → Render creates a new PR Preview instance.  
9. **Workflow waits for API readiness** → probes `/api/dogs` and accepts `200,400,401` as healthy; requires **3 consecutive** successes; timeout 300s.  
10. **API Integration Tests run** → run against the real hosted preview URL `https://campfitfurdogsapi-pr-<number>.onrender.com`.  
11. **Cleanup** → Neon branch and Render preview are deleted when the PR closes (label removal triggers Render destroy).

**Readiness and teardown probes**

- **Teardown probe**: `/health` expected to return `404` when the old preview is destroyed.  
- **Startup probe**: `/api/dogs` used as the practical readiness check for the new preview; accepts `200,400,401` and requires 3 consecutive successes.

**Rationale**

- Label‑driven deployment gives explicit control over when Render creates or destroys previews.  
- Recreating the Neon branch on each run prevents schema drift and allows migrations to be iteratively fixed within the same PR.  
- Running DB migrations and infrastructure tests before adding the label ensures the API deploys against a ready database.

---

### CI/CD Sequencing Conventions

**Order of operations for PR previews**

1. Remove `render-preview` label (if present).  
2. Wait for old preview teardown.  
3. Create Neon preview branch.  
4. Convert connection string and apply migrations.  
5. Run Infrastructure Integration Tests.  
6. Upload DB connection artifact.  
7. Add `render-preview` label.  
8. Wait for API readiness.  
9. Run API Integration Tests.

**Timeouts and retries**

- Readiness and teardown waits use a **300 second** timeout and require **3 consecutive** healthy responses to account for Render cold starts and transient errors.  
- Poll interval is 5 seconds.

**Artifacts**

- DB connection string is written to `db-conn.txt` and uploaded as a workflow artifact for debugging and reproducibility.

---

### Security and Secrets

- Secrets (Neon API key, DB credentials) are stored in GitHub Secrets and never printed in logs.  
- Connection strings are passed to processes via environment variables only.  
- The composite action that converts `postgres://` URIs to Npgsql strings runs inside CI and emits only the normalized connection string to the artifact; treat artifacts as sensitive.

---

### Operational Guardrails

- **Migrations** are applied only by the workflow; tests must not apply migrations.  
- **Preview DBs** are ephemeral and must not be treated as persistent test fixtures.  
- **Label control** is the single source of truth for Render preview lifecycle.  
- **Readiness checks** must use the configured endpoints and consecutive success rules.  
- **Infrastructure tests** must be deterministic and not depend on external state beyond the fresh Neon branch.

---

### Summary

This modernized architecture document codifies:

- strict layering and DDD guardrails  
- consistent CQRS and dispatch patterns  
- EF Core and repository conventions  
- SharedKernel as the canonical cross‑cutting library  
- a label‑driven, deterministic PR Preview pipeline using Neon and Render  
- explicit readiness and teardown rules to handle Render behavior

All code, scripts, and documentation must follow these conventions.
