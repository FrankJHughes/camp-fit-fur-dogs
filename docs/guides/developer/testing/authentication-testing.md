# Authentication Testing Guide

This guide explains how to test the authentication system implemented in **US‑110 (Authentication: Owner Login)** and **US‑111 (Authentication: Session Management)**.  
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

1. **[Unit Tests](ca://s?q=Explain_auth_unit_tests)** — Validate individual pipeline steps  
2. **[Integration Tests](ca://s?q=Explain_auth_integration_tests)** — Validate the full callback flow end‑to‑end  
3. **[Guardrail Tests](ca://s?q=Explain_auth_guardrail_tests)** — Validate security‑critical invariants  

This layered approach ensures:

- Deterministic behavior  
- High coverage  
- Isolation of failures  
- Protection against regressions  

---

# Test Layer 1 — Unit Tests

Unit tests validate the behavior of **individual pipeline steps** in isolation.

## What unit tests cover

### **[ValidateConfigurationStep](ca://s?q=Explain_ValidateConfigurationStep_tests)**  
- Missing OIDC configuration → throws `BadConfigurationException`

### **[ExchangeCodeStep](ca://s?q=Explain_ExchangeCodeStep_tests)**  
- Requires `ctx.Code`  
- Calls Auth0 token exchange  
- Stores `ctx.Token`

### **[FetchUserStep](ca://s?q=Explain_FetchUserStep_tests)**  
- Requires `ctx.Token`  
- Calls Auth0 `/userinfo`  
- Handles null userinfo → `AuthCallbackError.UserInfoFailure`

### **[ValidateUserStep](ca://s?q=Explain_ValidateUserStep_tests)**  
- Requires `ctx.User`  
- Missing `ExternalId` → `AuthCallbackError.MissingExternalId`

### **[ResolveIdentityStep](ca://s?q=Explain_ResolveIdentityStep_tests)**  
- Maps external ID to internal `CustomerId`  
- Creates Owner if missing  
- Returns existing Owner if present  

### **[AuditLoginStep](ca://s?q=Explain_AuditLoginStep_tests)**  
- Requires `ctx.User` + `ctx.CustomerId`  
- Calls audit logger exactly once  

### **[IssueCookieStep](ca://s?q=Explain_IssueCookieStep_tests)**  
- Always runs  
- Generates token + hash  
- Creates `SessionCookie`  

### **[CreateSessionStep](ca://s?q=Explain_CreateSessionStep_tests)**  
- Requires `ctx.TokenHash` + `ctx.CustomerId`  
- Persists session  
- Commits unit of work  

### **[BuildRedirectStep](ca://s?q=Explain_BuildRedirectStep_tests)**  
- Requires `ctx.Session` + `ctx.SessionCookie`  
- Sets `ctx.RedirectUrl`  

---

## Unit Test Example Structure

```csharp
public class ResolveIdentityStepTests
{
    [Fact]
    public async Task CreatesOwner_WhenExternalIdNotFound()
    {
        var step = new ResolveIdentityStep(resolver);

        var result = await step.ExecuteAsync(context, default);

        result.CustomerId.ShouldNotBeNull();
    }
}
```

Unit tests live in:

```
tests/Api.Tests/Authentication/Steps
```

---

# Test Layer 2 — Integration Tests

Integration tests validate the **entire callback flow**, including:

- Configuration validation  
- Code exchange  
- Userinfo retrieval  
- User validation  
- Identity resolution  
- Audit logging  
- Cookie issuance  
- Session creation  
- Redirect construction  

These tests run against:

- The real pipeline  
- The real DI container  
- The real session repository  
- The real Owner repository  
- A fake Auth0 client  

---

## Integration Test Responsibilities

### **Happy Path**  
- Valid code → valid token → valid userinfo  
- Owner created or reused  
- Session created  
- Cookie issued  
- Redirect returned  

### **Missing Authorization Code**  
- Returns 400  
- No session created  
- No cookie issued  

### **Missing Access Token**  
- Exchange returns null token  
- Next step fails with 502  

### **Missing User Profile**  
- `/userinfo` returns null  
- Returns 502  

### **Missing `sub` Claim**  
- `ValidateUserStep` throws `MissingExternalId`  
- Returns 502  

### **Identity Mapping Failure**  
- Resolver throws  
- Returns 500  

### **Session Creation Failure**  
- Repository throws  
- Returns 500  
- No cookie issued  

---

## Integration Test Example Structure

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

# Test Layer 3 — Guardrail Tests

Guardrail tests ensure **security‑critical invariants** never regress.

These tests validate **safety**, not business logic.

---

## Guardrail Responsibilities

### **Cookie Flags**  
- `HttpOnly=true`  
- `Secure=true` (preview/prod)  
- `SameSite=Lax`  
- `Path=/`  
- No sensitive data in cookie value  

### **Token Opacity**  
- Token is random  
- Token contains no user data  
- Token is not a JWT  
- Token contains no dots (`.`)  

### **Identity Mapping Purity**  
- Email is never used for identity  
- Only `ExternalId` (`sub`) determines identity  
- Missing `sub` → 502  

### **No Leaks**  
- No tokens logged  
- No userinfo logged  
- No internal IDs leaked to Auth0  

---

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

### **Test Auth0 Client**  
- Deterministic token exchange  
- Deterministic userinfo  
- Simulated failures  

### **Test Owner Repository**  
- In‑memory  
- Supports lookup + creation  

### **Test Session Repository**  
- In‑memory  
- Supports creation + lookup  

### **Test Clock**  
- Deterministic timestamps  

### **Test Cookie Extractor**  
- Parses `Set-Cookie` headers  

---

# Common Testing Pitfalls

### **Missing `sub` in test profiles**  
`ValidateUserStep` will fail with 502.

### **Using real Auth0 endpoints**  
Never required — always mock.

### **Forgetting to assert cookie flags**  
Security regressions can slip through.

### **Not testing Owner reuse**  
Identity mapping must be idempotent.

### **Not testing failure modes**  
Authentication has many.

---

# Related Documents

- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Authentication Operations](ca://s?q=Generate_Authentication_Operations_Guide)**  
- **[Create Account Form](ca://s?q=Generate_Create_Account_Form_Guide)**  
- **[Create Account Feature Slice](ca://s?q=Generate_Create_Account_Slice_Guide)**  
