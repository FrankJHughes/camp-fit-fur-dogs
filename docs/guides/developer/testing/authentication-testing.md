# Authentication Testing Guide  
**Aligned With Exclusive OIDC Authentication & Auth Callback Refactor**

This guide explains how to test the authentication system implemented in:

- **US‑110 — Authentication: Owner Login (OIDC)**
- **US‑111 — Session Management**
- **US‑184 — De‑feature Local Identity**

It documents the **testing strategy**, **test layers**, **test responsibilities**, and **test patterns** used to validate:

- OIDC login initiation  
- Frank Auth Callback Pipeline (protocol)  
- Application Auth Callback Pipeline (business)  
- Identity mapping  
- Session creation  
- Cookie issuance  
- Redirect computation (including `returnUrl`)  

This guide does **not** define rules or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on **how to test the authentication implementation that exists today**, aligned with the **exclusive OIDC authentication model**, **new callback architecture**, **new test harness**, and **new guardrail boundaries**.

---

# Testing Strategy Overview

Authentication is tested across **four layers**, each with a distinct purpose:

1. **Unit Tests** — Validate individual pipeline components  
2. **Application Pipeline Tests** — Validate business‑layer behavior  
3. **Integration Tests** — Validate the full callback flow end‑to‑end  
4. **Guardrail Tests** — Validate security‑critical invariants  

This layered approach ensures:

- Deterministic behavior  
- High coverage  
- Isolation of failures  
- Protection against regressions  

---

# Test Layer 1 — Unit Tests (Post‑Refactor)

Unit tests now target **pure components**, not step classes.

There are no more:

- `ValidateConfigurationStep`  
- `ExchangeCodeStep`  
- `FetchUserStep`  
- `ValidateUserStep`  
- `ResolveIdentityStep`  
- `IssueCookieStep`  
- `CreateSessionStep`  
- `BuildRedirectStep`  

These no longer exist.

Instead, unit tests cover **pure, isolated components** inside the Frank and Application pipelines.

---

## 1. Frank Pipeline Components (Protocol Layer)

### Token Exchange Component
- Valid authorization code → returns token payload  
- Missing/invalid code → throws protocol exception  
- External provider failure → throws protocol exception  

### Userinfo Component
- Valid token → returns normalized userinfo  
- Missing `sub` → throws protocol exception  
- Provider failure → throws protocol exception  

### Claims Normalization
- Normalizes provider‑specific claims  
- Produces stable identity payload  
- Never performs business logic  

---

## 2. Application Pipeline Components (Business Layer)

### Identity Resolution
- Existing Owner → reused  
- Missing Owner → created  
- Missing `sub` → throws business exception  
- Email is never used for identity  
- No protocol logic  

### Session Creation
- Creates session with hashed token  
- Persists via repository abstraction  
- Commits via `IUnitOfWork`  

### Cookie Value Computation
- Generates opaque token  
- Hashes token  
- Produces cookie value  
- Never issues cookies (API layer only)  

### Redirect Computation
- Uses configured `PostLoginRedirectUrl`  
- Validates optional `returnUrl`  
- Rejects unsafe URLs  
- Deterministic  

---

## Unit Test Example

```csharp
public class IdentityResolverTests
{
    [Fact]
    public async Task CreatesOwner_WhenExternalIdNotFound()
    {
        var resolver = new IdentityResolver(fakeRepo);

        var result = await resolver.ResolveAsync("auth0|abc123", profile);

        result.ShouldNotBeNull();
    }
}
```

Unit tests live in:

```
tests/Api.Tests/Authentication/Unit
```

---

# Test Layer 2 — Application Pipeline Tests

These tests validate the **Application Auth Callback Pipeline** end‑to‑end, using:

- Real pipeline  
- Real DI container  
- Fake Frank pipeline result  
- Fake repositories  
- Fake clock  

These tests verify:

### Identity Mapping
- Owner creation  
- Owner reuse  
- Missing `sub` → failure  

### Session Creation
- Session persisted  
- Token hash stored  
- Session associated with Owner  

### Cookie Value Computation
- Opaque token  
- No dots (`.`)  
- Not a JWT  
- Not base64 of user data  

### Redirect Computation
- Uses configured `PostLoginRedirectUrl`  
- Applies validated `returnUrl` when provided  
- Rejects unsafe `returnUrl` values  
- Deterministic  

---

## Application Pipeline Test Example

```csharp
public class ApplicationAuthCallbackPipelineTests
{
    [Fact]
    public async Task Pipeline_CreatesSession_And_ComputesCookieValue()
    {
        var result = await _pipeline.BuildAsync(request, default);

        result.CookieValue.ShouldNotBeNull();
        result.SessionId.ShouldNotBe(Guid.Empty);
    }
}
```

Application pipeline tests live in:

```
tests/Api.Tests/Authentication/ApplicationPipeline
```

---

# Test Layer 3 — Integration Tests (Aligned With Exclusive OIDC)

Integration tests validate the **entire callback flow**, including:

- Frank pipeline (protocol)  
- Application pipeline (business)  
- API callback endpoint (boundary)  
- Cookie issuance  
- Redirect behavior  
- `returnUrl` propagation  

These tests run against:

- The real API  
- The real DI container  
- A deterministic fake Auth0 client  
- Real repositories (in‑memory or Testcontainers)  

**Important:**  
Integration tests use **ApiContext + ApiFactory**, not the guardrail harness.

---

## Integration Test Responsibilities

### Happy Path
- Valid code → valid token → valid userinfo  
- Owner created or reused  
- Session created  
- Cookie issued  
- Redirect returned  
- `returnUrl` applied when valid  

### Missing Authorization Code
- Returns 400  
- No session created  
- No cookie issued  

### Protocol Failures
- Token exchange failure → 502  
- Userinfo failure → 502  
- Missing `sub` → 502  
- Invalid or tampered `state` → 400  

### Business Failures
- Identity resolver failure → 500  
- Session creation failure → 500  
- Invalid `returnUrl` → 400  
- No cookie issued  

---

## Integration Test Example

```csharp
public class AuthCallbackEndpointTests
{
    [Fact]
    public async Task Callback_IssuesSessionCookie_OnSuccess()
    {
        var response = await _client.GetAsync("/api/auth/callback?code=abc");

        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.ShouldContainKey("Set-Cookie");
    }
}
```

Integration tests live in:

```
tests/Api.Tests/Authentication/Callback
```

---

# Test Layer 4 — Guardrail Tests

Guardrail tests ensure **security‑critical invariants** never regress.

Guardrails validate **safety**, not business logic.

Guardrails use **minimal DI**, not Testcontainers or ApiFactory, unless testing routing or persistence.

---

## Guardrail Responsibilities

### Cookie Flags
- `HttpOnly=true`  
- `Secure=true` (preview/prod)  
- `SameSite=Lax`  
- `Path=/`  
- No sensitive data in cookie value  
- Cookie value is opaque and random  

### Token Opacity
- Token is random  
- Token contains no user data  
- Token is not a JWT  
- Token contains no dots (`.`)  
- Token is not base64 of any recognizable structure  

### Identity Mapping Purity
- Email is never used for identity  
- Only `ExternalId` (`sub`) determines identity  
- Missing `sub` → 502  
- No identity logic in API or Frank layers  

### No Leaks
- No tokens logged  
- No userinfo logged  
- No internal IDs leaked to Auth0  
- No redirect URLs logged if they contain sensitive data  

---

## Guardrail Test Example

```csharp
public class SessionCookieGuardrailTests
{
    [Fact]
    public async Task Cookie_IsOpaque_And_HasCorrectFlags()
    {
        var cookie = await IssueCookieAsync();

        cookie.Value.ShouldNotContain(".");
        cookie.HttpOnly.ShouldBeTrue();
        cookie.SameSite.ShouldBe(SameSiteMode.Lax);
    }
}
```

Guardrail tests live in:

```
tests/Api.Tests/Guardrails
```

---

# Test Data Setup

Authentication tests rely on deterministic fakes:

### Fake Frank Pipeline
- Deterministic token exchange  
- Deterministic userinfo  
- Simulated protocol failures  
- No business logic  

### Fake Application Pipeline Dependencies
- In‑memory Owner repository  
- In‑memory Session repository  
- Deterministic clock  
- Deterministic redirect computation  

### Test Cookie Extractor
- Parses `Set-Cookie` headers  
- Validates cookie flags  
- Validates opacity  

---

# Common Testing Pitfalls (Updated)

### ❌ Using Testcontainers in guardrails  
Guardrails must use minimal DI unless testing persistence.

### ❌ Using real Auth0 endpoints  
Never required — always mock.

### ❌ Forgetting to assert cookie flags  
Security regressions can slip through.

### ❌ Not testing Owner reuse  
Identity mapping must be idempotent.

### ❌ Not testing `returnUrl` validation  
Unsafe URLs must be rejected.

### ❌ Not testing failure modes  
Authentication has many.

### ❌ Using old step‑based tests  
The step engine no longer exists.

---

# Related Documents

- **Session Management Guide**  
- **Identity Mapping Guide**  
- **Authentication Architecture Guide**  
- **Authentication Operations Guide**  
- **Create Account Form Guide**  
- **Create Account Feature Slice Guide**
