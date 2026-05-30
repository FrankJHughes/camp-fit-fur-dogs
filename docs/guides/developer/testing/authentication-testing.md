# Authentication Testing Guide

This guide explains how to test the authentication system implemented in **US‑110 (Authentication: Owner Login)**.  
It documents the *testing strategy*, *test layers*, *test responsibilities*, and *test patterns* used to validate:

- OIDC login initiation  
- Auth callback pipeline  
- Identity mapping  
- Session creation  
- Cookie issuance  

This guide does **not** define rules or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on **how to test the authentication implementation that exists today**.

---

# Testing Strategy Overview

Authentication is tested across **three layers**, each with a different purpose:

1. **Unit Tests** — Validate individual pipeline steps  
2. **Integration Tests** — Validate the full callback flow end‑to‑end  
3. **Guardrail Tests** — Validate security‑critical invariants  

This layered approach ensures:

- Deterministic behavior  
- High coverage  
- Isolation of failures  
- Protection against regressions  

---

# Test Layer 1 — Unit Tests

Unit tests validate the behavior of **individual pipeline steps** in isolation.

## What unit tests cover

### 1. ExchangeAuthorizationCodeStep  
- Handles missing authorization code  
- Handles Auth0 token exchange failures  
- Stores tokens in the pipeline context  

### 2. FetchUserProfileStep  
- Fetches profile using access token  
- Handles missing/invalid access token  
- Handles Auth0 profile fetch failures  

### 3. ResolveIdentityStep  
- Extracts `sub` from ID token  
- Looks up existing Owner  
- Creates new Owner if missing  
- Rejects missing/invalid `sub`  

### 4. CreateSessionCookieStep  
- Creates session record  
- Hashes token correctly  
- Issues cookie with correct flags  
- Handles repository failures  

### 5. ReturnCallbackResultStep  
- Redirects to correct frontend URL  
- Handles error states  

## Unit Test Example Structure

```csharp
public class ResolveIdentityStepTests
{
    [Fact]
    public async Task CreatesOwner_WhenExternalIdNotFound()
    {
        // Arrange
        var step = new ResolveIdentityStep(...);

        // Act
        await step.ExecuteAsync(context);

        // Assert
        context.OwnerId.ShouldNotBeNull();
    }
}
```

Unit tests live in:

```
tests/Api.Tests/Authentication/Steps
```

---

# Test Layer 2 — Integration Tests

Integration tests validate the **entire callback flow**:

- Exchange code  
- Fetch profile  
- Resolve identity  
- Create session  
- Issue cookie  
- Redirect  

These tests run against:

- The real pipeline  
- The real DI container  
- The real session repository  
- The real Owner repository  
- A test Auth0 client (mocked)  

## Integration Test Responsibilities

### 1. Happy Path  
- Valid code → valid tokens → valid profile  
- Owner created or reused  
- Session created  
- Cookie issued  
- Redirect returned  

### 2. Missing Authorization Code  
- Returns 400  
- No session created  
- No cookie issued  

### 3. Missing Access Token  
- Returns 500  
- No session created  

### 4. Missing User Profile  
- Returns 500  
- No session created  

### 5. Identity Mapping Failure  
- Returns 500  
- No session created  

### 6. Session Creation Failure  
- Returns 500  
- No cookie issued  

## Integration Test Example Structure

```csharp
public class AuthCallbackEndpointTests
{
    [Fact]
    public async Task Callback_IssuesSessionCookie_OnSuccess()
    {
        var response = await _client.GetAsync("/api/auth/callback?code=abc");

        response.StatusCode.ShouldBe(HttpStatusCode.Redirect);
        response.Headers.ShouldContain(h => h.Key == "Set-Cookie");
    }
}
```

Integration tests live in:

```
tests/Api.Tests/Authentication/Callback
```

---

# Test Layer 3 — Guardrail Tests

Guardrail tests ensure **security‑critical invariants** never regress.

These tests do not validate business logic — they validate **safety**.

## Guardrail Responsibilities

### 1. Cookie Flags  
- `HttpOnly=true`  
- `Secure=true` (preview/prod)  
- `SameSite=Lax`  
- `Path=/`  
- No sensitive data in cookie value  

### 2. Token Opacity  
- Cookie value is random  
- Cookie value contains no user data  
- Cookie value is not a JWT  

### 3. Identity Mapping Purity  
- Email is never used for identity  
- Only `sub` determines identity  
- External ID is required  

### 4. No Leaks  
- No tokens logged  
- No profile data logged  
- No internal IDs leaked to Auth0  

## Guardrail Test Example Structure

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

Authentication tests rely on:

### 1. Test Auth0 Client  
- Returns deterministic tokens  
- Returns deterministic profiles  
- Simulates error conditions  

### 2. Test Owner Repository  
- In‑memory implementation  
- Supports lookup + creation  

### 3. Test Session Repository  
- In‑memory implementation  
- Supports creation + lookup  

### 4. Test Clock  
- Ensures deterministic timestamps  

### 5. Test Cookie Issuer  
- Extracts cookie values for assertions  

---

# Common Testing Pitfalls

### 1. Missing `sub` in test profiles  
Identity mapping will fail.

### 2. Using real Auth0 endpoints  
Never required — always mock.

### 3. Forgetting to assert cookie flags  
Security regressions can slip through.

### 4. Not testing Owner reuse  
Critical for idempotent logins.

### 5. Not testing failure modes  
Authentication has many.

---

# Related Documents

- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Authentication Operations](ca://s?q=Generate_Authentication_Operations_Guide)**  
- **[Create Account Form](ca://s?q=Generate_Create_Account_Form_Guide)**  
- **[Create Account Feature Slice](ca://s?q=Generate_Create_Account_Slice_Guide)**
