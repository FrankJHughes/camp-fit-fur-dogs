# Code Conventions

Code conventions define **how code is written** across backend and frontend slices.  
They complement the architecture, workflow, and documentation conventions and ensure consistency, testability, and maintainability.

These conventions describe **implementation‑level rules**, not architectural governance.

Frank provides cross‑cutting primitives, DI auto‑registration for attributed interfaces, endpoint discovery, validator scanning, hosting provider abstractions, security headers, environment seams, the StartupEngine, and the HostingEngine.  
Application, Infrastructure, and Api remain responsible for their own slice‑specific DI.

---

# 1. Backend Code Conventions

The backend follows a strict layering model enforced by guardrails:

- **Domain** — business rules  
- **Application** — use‑case orchestration  
- **Infrastructure** — persistence + external integrations  
- **Api** — endpoints + request/response shaping  
- **Frank** — cross‑cutting primitives and auto‑registration engine

## 1.1 Layer Responsibilities

### Domain
- Aggregates  
- Entities  
- Value objects  
- Domain events  
- Invariants  
- Business rules  

Domain must not depend on Application, Infrastructure, Api, or Frank.

### Application
- Commands, queries, handlers  
- Validation  
- Domain interaction  
- Transaction boundaries  
- Domain event publishing  

Application must not depend on Infrastructure or Api.

### Infrastructure
- EF Core persistence  
- Repositories  
- Unit of Work  
- External integrations  
- Hosting provider implementations  

Infrastructure must not depend on Api.

### Api
- Endpoints  
- DTO binding  
- Authorization  
- Dispatching commands/queries  
- Security headers  
- Endpoint discovery  
- Startup modules  

Api must not contain business logic.

### Frank
- DI auto‑registration  
- CQRS dispatchers  
- Validator scanning  
- Endpoint discovery  
- Security headers middleware  
- Hosting abstractions  
- Environment abstraction  
- StartupEngine  
- HostingEngine  
- Test seams  

Frank must not depend on product code.

---

# 2. CQRS Conventions

## 2.1 Commands and Queries

Commands and queries:

- Live in Application slices  
- Represent use‑case intent  
- Must be immutable  
- Must have validators when input exists  
- Must not contain behavior  
- Must not reference Infrastructure or Api  

## 2.2 Handlers

Handlers must:

- Validate inputs  
- Load aggregates or required state  
- Invoke domain behavior  
- Persist changes via repositories + Unit of Work  
- Publish domain events  

Handlers must not:

- Contain business rules  
- Access EF Core directly  
- Access HttpContext  
- Invoke other handlers directly  
- Return domain entities  
- Perform cross‑slice logic  

Handlers return DTOs or primitives.

## 2.3 Registration

Handlers are auto‑registered via Frank’s DI engine:

- Interfaces marked with `[AutoRegister]` are discovered  
- Handlers must not be manually registered  
- Handlers must not bypass the dispatcher pipeline  

---

# 3. Immutable Context Builder Conventions

Authentication callback flows and other multi‑stage transformations use **ImmutableContextBuilder** in both Frank and Application layers.

## 3.1 Required Structure

Each builder must define:

- `TRequest` — immutable input  
- `TContext` — immutable working context  
- `TResult` — immutable output  

All three types must:

- Be immutable  
- Use required members for all non‑optional data  
- Contain no behavior  
- Contain no side effects  
- Contain no direct Infrastructure or Api dependencies  

## 3.2 Frank Pipeline Builder

Frank pipeline builders:

- Handle **protocol logic only**  
- May call external identity providers via abstractions  
- Must not contain business logic  
- Must not access persistence  
- Must not construct aggregates  
- Must not issue cookies  
- Must not compute redirect URLs  

They produce a protocol‑normalized result consumed by Application.

## 3.3 Application Pipeline Builder

Application pipeline builders:

- Handle **business logic only**  
- May perform identity resolution  
- May create sessions via abstractions  
- Must not contain protocol logic  
- Must not decode or validate tokens directly  
- Must not call external identity providers  
- Must not issue cookies (only compute cookie value)  
- Must not write to HttpContext  

They produce a result that includes all data the Api endpoint needs.

## 3.4 Required Members for Authentication

`ApplicationAuthCallbackContextBuilderResult` must include:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue` (opaque session token)  
- `RedirectUrl`  

All must be set by the builder before returning.

---

# 4. Endpoint Conventions

Endpoints implement `IEndpoint` and are discovered automatically by Frank.

## 4.1 Structure

Each endpoint:

- Lives in the Api slice  
- Implements `void Map(IEndpointRouteBuilder app)`  
- Defines routes explicitly  
- Dispatches commands/queries via dispatchers  
- Performs DTO binding and authorization  

## 4.2 Auth Callback Endpoint Rules

Authentication callback endpoints must:

- Extract the `code` query parameter  
- Throw `BadRequestException` if missing  
- Invoke the Frank pipeline builder  
- Invoke the Application pipeline builder  
- Issue the authentication cookie using `CookieValue`  
- Redirect to the `RedirectUrl` provided by the Application pipeline  

Endpoints must not:

- Contain protocol logic  
- Contain business logic  
- Construct aggregates  
- Perform persistence  
- Compute redirect URLs  
- Generate cookie values  

## 4.3 Routing Rules

- One endpoint per command/query  
- No attribute routing  
- No monolithic endpoint files  
- Explicit mapping inside the endpoint class  
- Route patterns must be stable and predictable  

---

# 5. Error Handling Conventions

## 5.1 Missing Code

Missing `code` must throw:

```csharp
throw new BadRequestException("Missing authorization code.");
```

## 5.2 Pipeline Failures

Any exception thrown inside:

- Frank pipeline  
- Application pipeline  

must be handled by global exception middleware.

Endpoints must not catch these exceptions.

---

# 6. Testing Conventions for Callback Flows

## 6.1 Fake Builders (Not Engines)

Tests must use:

- Fake Frank pipeline builder  
- Fake Application pipeline builder  

Both must implement the exact generic signature of the real builders.

## 6.2 ApiFactory Overrides

Tests must override:

- Frank pipeline builder  
- Application pipeline builder  

using:

```csharp
WithServiceOverride(services =>
{
    // Replace real builders with fakes
});
```

## 6.3 Cookie Assertions

Tests must assert:

- Cookie is issued  
- Cookie name is correct (`cfd.session`)  
- Cookie is opaque (no dots, no user data, not a JWT)  
- Cookie flags are correct:
  - HttpOnly  
  - Secure (preview/prod)  
  - SameSite=Lax  
  - Path=/  
  - Max‑Age set  

## 6.4 Redirect Assertions

Tests must assert:

- Status code = 302  
- Redirect URL matches the Application pipeline result  

## 6.5 Missing Code Tests

Tests must assert:

- Status code = 400  
- Error message contains “Missing authorization code”  

---

# 7. EF Core & Persistence Conventions

## 7.1 Mapping

- Aggregate root → table  
- Id is not value‑generated  
- Value objects mapped as owned types  
- Domain events ignored by EF Core  
- Configurations live in Infrastructure  
- Navigation properties must be explicit  
- No lazy loading  

## 7.2 Unit of Work

- Coordinates `SaveChangesAsync`  
- Dispatches domain events  
- Contains no business logic  
- Must be injected into command handlers only  

## 7.3 Repositories

- Auto‑registered via `[AutoRegister]`  
- Must not expose EF Core types  
- Must not contain business logic  
- Must not return domain entities to Api  
- Must not perform read‑model queries  

## 7.4 Readers

- Return DTOs only  
- Must not return domain entities  
- Must not mutate state  
- Must use `AsNoTracking`  
- Must not depend on repositories  

## 7.5 Migrations

- Applied by CI only  
- Must be idempotent  
- Must tolerate empty databases  
- Must be preview‑safe  
- Must not contain environment‑specific logic  

---

# 8. Hosting Provider Conventions

Hosting providers configure environment‑specific behavior at startup.  
They are executed by **HostingEngine**, not manually.

## 8.1 Provider Rules

Providers must:

- Use injected abstractions only  
- Never read environment variables directly  
- Never perform HTTP calls directly  
- Never parse JSON or ZIP files directly  
- Never write configuration directly  
- Be fully testable  

## 8.2 Provider Selection

- Providers are evaluated in order  
- First active provider wins  
- Enforced by guardrails  

## 8.3 Render Hosting Provider

Render is the canonical provider for PR Previews.

It uses:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`

Required environment variables:

- `IS_PULL_REQUEST`  
- `RENDER_GIT_REPO_SLUG`  
- `RENDER_SERVICE_NAME`  
- `GITHUB_PAT`

Required artifacts:

- `pr-{n}-db/db-conn.txt`  
- `pr-{n}-frontend/frontend-url.txt`  

---

# 9. Test Seam Conventions

The test harness uses Frank’s seams to simulate hosting, environment, and external systems.

## 9.1 HttpClient Seam

- All external HTTP calls use named HttpClients  
- Tests replace HttpClient with `FakeHttpMessageHandler`  
- No real network calls allowed in tests  

## 9.2 Hosting Provider Seam

Providers use injected abstractions; tests supply fakes for:

- `IEnvironment`  
- `IRenderPrParser`  
- `IGitHubArtifactClient`  
- `IRenderConfigurationWriter`  

## 9.3 Identity Seam

- Identity resolved via `ICurrentUserService`  
- Tests use `FakeCurrentUser`  

## 9.4 Audit Logging Seam

- Audit logging abstracted  
- Tests use `FakeAuditLogger`  

---

# 10. Frontend Conventions

The frontend mirrors backend slice structure.

## 10.1 Folder Structure

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

## 10.2 Rules

- One slice per aggregate  
- Hooks must not perform fetches directly  
- API layer owns all HTTP calls  
- Components must be pure and testable  
- No business logic in components  
- Tests must not hit real APIs  
- All API clients return `CommandResult` or `QueryState`  
- No direct use of `fetch` in components or hooks  

## 10.3 Form Conventions

- Forms use `FormCommand.run`  
- Validation uses Zod schemas  
- Form state managed via `useFormStateMachine`  
- Errors displayed via `ErrorSummary`  
- No business logic in form components  

## 10.4 Query Conventions

- Queries use `useApiQuery`  
- Query functions must be pure  
- Query state must be explicit (`loading`, `error`, `success`)  
- No implicit fetches  

---

# Summary

These conventions ensure:

- Strict layering  
- Deterministic hosting behavior  
- Strong security posture  
- Testable, maintainable slices  
- Consistent use of Frank for cross‑cutting concerns  
- Preview‑safe behavior  
- Predictable CI/CD behavior  
- Fully aligned authentication callback flows using ImmutableContextBuilder  

All contributors must follow these rules.
