# Authentication Testing Guide  
Aligned With Exclusive OIDC Authentication & Auth Callback Refactor

This guide explains how to test the authentication system implemented in:

- US‑110 — Authentication: Owner Login (OIDC)
- US‑111 — Session Management
- US‑184 — De‑feature Local Identity

It documents the testing strategy, test layers, responsibilities, and patterns used to validate:

- OIDC login initiation  
- Frank Auth Callback Pipeline (protocol)  
- Application Auth Callback Pipeline (business)  
- Identity mapping  
- Session creation  
- Cookie issuance  
- Redirect computation (including `returnUrl`)  

This guide does not define rules or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on how to test the authentication implementation that exists today, aligned with the exclusive OIDC authentication model, new callback architecture, new test harness, and guardrail boundaries.

---

## Testing Strategy Overview

Authentication is tested across four layers:

1. Unit Tests — validate individual pipeline components  
2. Application Pipeline Tests — validate business‑layer behavior  
3. Integration Tests — validate the full callback flow end‑to‑end  
4. Guardrail Tests — validate security‑critical invariants  

This layered approach ensures:

- deterministic behavior  
- high coverage  
- isolation of failures  
- protection against regressions  

---

## Unit Tests (Pure Components)

Unit tests target pure components inside the Frank and Application pipelines.

The old step engine no longer exists. There are no tests for:

- `ValidateConfigurationStep`  
- `ExchangeCodeStep`  
- `FetchUserStep`  
- `ValidateUserStep`  
- `ResolveIdentityStep`  
- `IssueCookieStep`  
- `CreateSessionStep`  
- `BuildRedirectStep`  

Unit tests now cover pure, isolated components.

---

### Frank Pipeline Components (Protocol Layer)

#### Token Exchange Component
- valid authorization code → returns token payload  
- missing/invalid code → throws protocol exception  
- external provider failure → throws protocol exception  

#### Userinfo Component
- valid token → returns normalized userinfo  
- missing `sub` → throws protocol exception  
- provider failure → throws protocol exception  

#### Claims Normalization
- normalizes provider‑specific claims  
- produces stable identity payload  
- never performs business logic  

---

### Application Pipeline Components (Business Layer)

#### Identity Resolution
- existing Owner → reused  
- missing Owner → created  
- missing `sub` → throws business exception  
- email is never used for identity  
- no protocol logic  

#### Session Creation
- creates session with hashed token  
- persists via repository abstraction  
- commits via `IUnitOfWork`  

#### Cookie Value Computation
- generates opaque token  
- hashes token  
- produces cookie value  
- never issues cookies (API layer only)  

#### Redirect Computation
- uses configured `PostLoginRedirectUrl`  
- validates optional `returnUrl`  
- rejects unsafe URLs  
- deterministic  

---

### Unit Test Example

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

## Application Pipeline Tests

These tests validate the Application Auth Callback Pipeline end‑to‑end, using:

- real pipeline  
- real DI container  
- fake Frank pipeline result  
- fake repositories  
- fake clock  

These tests verify:

### Identity Mapping
- Owner creation  
- Owner reuse  
- missing `sub` → failure  

### Session Creation
- session persisted  
- token hash stored  
- session associated with Owner  

### Cookie Value Computation
- opaque token  
- no dots (`.`)  
- not a JWT  
- not base64 of user data  

### Redirect Computation
- uses configured `PostLoginRedirectUrl`  
- applies validated `returnUrl` when provided  
- rejects unsafe `returnUrl` values  
- deterministic  

---

### Application Pipeline Test Example

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

## Integration Tests (Full Callback Flow)

Integration tests validate the entire callback flow:

- Frank pipeline (protocol)  
- Application pipeline (business)  
- API callback endpoint (boundary)  
- cookie issuance  
- redirect behavior  
- `returnUrl` propagation  

These tests run against:

- the real API  
- the real DI container  
- a deterministic fake Auth0 client  
- real repositories (in‑memory or Testcontainers)  

Integration tests use **ApiContext + ApiFactory**, not the guardrail harness.

---

### Integration Test Responsibilities

#### Happy Path
- valid code → valid token → valid userinfo  
- Owner created or reused  
- session created  
- cookie issued  
- redirect returned  
- `returnUrl` applied when valid  

#### Missing Authorization Code
- returns 400  
- no session created  
- no cookie issued  

#### Protocol Failures
- token exchange failure → 502  
- userinfo failure → 502  
- missing `sub` → 502  
- invalid or tampered `state` → 400  

#### Business Failures
- identity resolver failure → 500  
- session creation failure → 500  
- invalid `returnUrl` → 400  
- no cookie issued  

---

### Integration Test Example

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

## Guardrail Tests

Guardrail tests ensure security‑critical invariants never regress.

Guardrails validate safety, not business logic.

Guardrails use minimal DI unless testing routing or persistence.

---

### Guardrail Responsibilities

#### Cookie Flags
- `HttpOnly=true`  
- `Secure=true` (preview/prod)  
- `SameSite=Lax`  
- `Path=/`  
- no sensitive data in cookie value  
- cookie value is opaque and random  

#### Token Opacity
- token is random  
- token contains no user data  
- token is not a JWT  
- token contains no dots (`.`)  
- token is not base64 of any recognizable structure  

#### Identity Mapping Purity
- email is never used for identity  
- only `ExternalId` (`sub`) determines identity  
- missing `sub` → 502  
- no identity logic in API or Frank layers  

#### No Leaks
- no tokens logged  
- no userinfo logged  
- no internal IDs leaked to Auth0  
- no redirect URLs logged if they contain sensitive data  

---

### Guardrail Test Example

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

## Test Data Setup

Authentication tests rely on deterministic fakes:

### Fake Frank Pipeline
- deterministic token exchange  
- deterministic userinfo  
- simulated protocol failures  
- no business logic  

### Fake Application Pipeline Dependencies
- in‑memory Owner repository  
- in‑memory Session repository  
- deterministic clock  
- deterministic redirect computation  

### Test Cookie Extractor
- parses `Set-Cookie` headers  
- validates cookie flags  
- validates opacity  

---

## Common Testing Pitfalls

- using Testcontainers in guardrails  
- using real Auth0 endpoints  
- forgetting to assert cookie flags  
- not testing Owner reuse  
- not testing `returnUrl` validation  
- not testing failure modes  
- using old step‑based tests  

---

## Related Documents

- Session Management Guide  
- Identity Mapping Guide  
- Authentication Architecture Guide  
- Authentication Operations Guide  
- Architecture Governance  
- Security Governance  
