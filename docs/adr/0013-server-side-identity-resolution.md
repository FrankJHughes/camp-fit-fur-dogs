# ADR-0013: Server-Side Identity Resolution via ICurrentUserService

| Field     | Value                              |
|-----------|-------------------------------------|
| Status    | Accepted                           |
| Date      | 2026-04-15                         |
| Deciders  | Frank Hughes                       |

## Context

As Camp Fit Fur Dogs introduced its second aggregate (Dogs), the
Register Dog endpoint initially accepted `OwnerId` in the request
body. This meant the client told the server *who it was* — an approach
that is inherently insecure in any system that will eventually enforce
authentication and authorization.

The team needed a pattern that:

- Resolves user identity server-side, never trusting the client.
- Keeps the API contract clean — request DTOs carry only the data the
  user is submitting, not who they are.
- Works today without authentication infrastructure, while establishing
  the seam where real auth plugs in later.
- Is testable — integration tests must control which user the endpoint
  sees, without modifying request payloads.
- Follows the existing DDD layered architecture (ADR-0002) — the
  abstraction lives in the Application layer, the implementation in
  Infrastructure.

## Decision

We will resolve the current user's identity server-side via an
`ICurrentUserService` abstraction. Endpoints never accept identity
fields (e.g., `OwnerId`, `UserId`, `CustomerId`) in the request body.

### Pattern Structure

**Abstraction** (`Application/Abstractions/ICurrentUserService.cs`):

```csharp
public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}
```

Single method. Returns the authenticated user's ID. Throws if no user
is authenticated (fail-closed).

**Endpoint composition pattern:**

1. Bind a DTO (e.g., `RegisterDogRequest`) containing only user-submitted
   data — no identity fields.
2. Inject `ICurrentUserService` into the endpoint.
3. Construct the command by combining the DTO fields with the identity
   from `ICurrentUserService.GetCurrentUserId()`.

This keeps the API layer responsible for *assembling* the command, while
the Application layer owns the *contract* for identity.

### Implementations

| Implementation | Layer | Lifetime | Purpose |
|----------------|-------|----------|---------|
| `DummyCurrentUserService` | Infrastructure | Singleton | Pre-auth placeholder; returns a hardcoded GUID. Replaced when authentication lands. |
| `TestCurrentUserService` | Api.Tests | — | Test double with a settable `CurrentUserId`. The `CampFitFurDogsApiFactory` overrides the DI registration so each test controls which user the endpoint sees. |
| *(future)* `HttpContextCurrentUserService` | Infrastructure | Scoped | Reads identity from `HttpContext.User` claims. Ships with the authentication story. |

### DI Lifetime Transition

The placeholder is `Singleton` because it is stateless. The real
implementation **must** be `Scoped` — it reads from `HttpContext`, which
is per-request. This transition requires only a one-line change in
`DependencyInjection.cs` and no changes to consuming code.

### Testing Strategy

Integration tests control identity through `TestCurrentUserService`:

1. `CampFitFurDogsApiFactory` creates a singleton `TestCurrentUserService`
   and overrides the `ICurrentUserService` registration.
2. Each test sets `_testUserService.CurrentUserId = ownerId` before
   calling the endpoint.
3. The endpoint calls `ICurrentUserService.GetCurrentUserId()` and gets
   the test-controlled value.

This eliminates the need for fake auth middleware, bearer tokens, or
claims manipulation in tests.

### Scope

`ICurrentUserService` answers one question: *who is the current user?*
It does not answer *what can this user do?* Authorization (role checks,
permission guards, resource ownership validation) is a separate concern
that will build on top of this identity seam when needed.

### Alternatives Considered

| Alternative | Strengths | Why Not |
|-------------|-----------|---------|
| **Accept identity in request body** | Simple; no abstraction needed | Insecure — the client controls identity. Cannot enforce ownership or authorization server-side. Every future endpoint would repeat the same vulnerability. |
| **Read `HttpContext.User` directly in endpoints** | No abstraction; works with real auth | Couples endpoints to ASP.NET Core; untestable without fake auth middleware; violates ADR-0002 layering (Application layer cannot depend on HttpContext). |
| **Custom middleware that sets a scoped identity value** | Centralizes identity extraction | Adds middleware ordering complexity; still needs an abstraction for the Application layer to consume; solves the same problem with more moving parts. |
| **`ClaimsPrincipal` passed through command** | Carries full claims context | Leaks ASP.NET Core types into the Application/Domain layers; commands should carry data, not framework types. |

### Why an Abstraction

- **Security by default** — Endpoints cannot accidentally accept
  client-supplied identity. The pattern makes the secure path the only
  path.
- **Testability** — A settable test double is simpler and faster than
  fake auth middleware or token generation.
- **Layer compliance** — Application layer depends on its own interface,
  not on `HttpContext` or any ASP.NET Core type (ADR-0002).
- **Minimal seam** — One interface, one method. When real auth ships,
  the swap is mechanical: replace the registration, delete the
  placeholder. No endpoint changes.

## Consequences

### Positive

- Every endpoint that needs identity uses the same pattern. New
  contributors see the convention in one place.
- The API contract is clean — DTOs carry user intent, not user identity.
- Integration tests control identity with a single property assignment.
  No auth ceremony.
- The security posture is established before authentication ships.
  Retrofitting identity resolution into existing endpoints is avoided.

### Negative

- `DummyCurrentUserService` returns a hardcoded GUID, meaning the
  running application does not enforce real identity until
  authentication lands. This is acceptable for the current development
  phase but must not ship to production.
- One additional interface and two implementations for a behavior that
  could be a single `HttpContext.User` read. The abstraction pays for
  itself at the second endpoint; until then it is speculative investment
  (justified by the security and testability benefits).

### Neutral

- The pattern does not prescribe how authentication is implemented.
  JWT, cookie-based, or external identity provider — all are compatible.
  The only contract is `Guid GetCurrentUserId()`.
- The `Singleton` → `Scoped` lifetime transition is a known future
  change. It is documented here and in `copilot-instructions.md` so it
  is not a surprise.
