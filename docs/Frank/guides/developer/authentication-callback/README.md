# Authentication Callback — Developer Guide

The Authentication Callback capability implements the **OIDC authorization‑code callback pipeline** for Frank.  
It exchanges an authorization code for tokens, validates the ID token, fetches user information, and produces a normalized `FrankAuthCallbackResult`.

This guide documents the architecture, invariants, and extension points for developers implementing or integrating with the Authentication Callback capability.

---

# 1. Core Purpose

The Authentication Callback pipeline:

1. Receives an authorization code from an external identity provider (Auth0).
2. Exchanges the code for tokens.
3. Validates the ID token cryptographically.
4. Fetches user profile information from the UserInfo endpoint.
5. Normalizes identity into a Frank‑specific result object.
6. Returns a `FrankAuthCallbackResult` for downstream identity resolution.

This pipeline is **pure**, **deterministic**, and **side‑effect free** except for HTTP calls to the identity provider.

---

# 2. Core Models

## 2.1 AuthCallbackContext (base)

````csharp
public abstract record AuthCallbackContext : ImmutableContextBase;
````

A marker base type for callback contexts.

---

## 2.2 FrankAuthCallbackRequest

````csharp
public record FrankAuthCallbackRequest : ImmutableContextBuilderRequestBase
{
    public required string Code { get; init; }
}
````

Represents the inbound request from the callback endpoint.

---

## 2.3 FrankAuthCallbackResult

````csharp
public sealed record FrankAuthCallbackResult : ImmutableContextBuilderResultBase
{
    public required string SubjectId { get; init; }
    public required IReadOnlyDictionary<string, string> Claims { get; init; }

    public string? Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
    public string? Picture { get; init; }

    public string Provider { get; init; } = "unknown";
}
````

This is the **normalized identity** returned by the pipeline.

---

# 3. OIDC Callback Context

````csharp
public sealed record OidcAuthCallbackContext : ImmutableContextBase
{
    public required string Code { get; init; }
    public required DateTimeOffset Now { get; init; }

    public string? AccessToken { get; init; }
    public string? IdToken { get; init; }

    public string? SubjectId { get; init; }
    public IReadOnlyDictionary<string, string>? Claims { get; init; }

    public string? Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
    public string? Picture { get; init; }

    public string Provider => "auth0";
}
````

### Invariants

- `Code` and `Now` are **immutable**.
- Pipeline steps may add:
  - tokens
  - claims
  - userinfo fields
- Steps may **not** modify immutable fields.

---

# 4. OIDC Clients

## 4.1 Auth0OidcTokenClient

Responsible for exchanging the authorization code for tokens.

Key behaviors:

- POSTs to `/oauth/token`
- Extracts `access_token` and `id_token`
- Throws on failure

---

## 4.2 Auth0OidcUserInfoClient

Responsible for retrieving user profile information.

Key behaviors:

- GET `/userinfo` with Bearer token
- Extracts:
  - subject
  - email
  - given/family name
  - picture
  - all string claims

---

# 5. Pipeline Steps

Each step implements:

````csharp
IImmutableContextBuildStep<OidcAuthCallbackContext>
````

Steps execute in order, each transforming the context.

---

## 5.1 ExchangeCodeStep

- Executes if `ctx.Code` is present.
- Exchanges the authorization code for:
  - `AccessToken`
  - `IdToken`

Failure → `OidcProtocolException`.

---

## 5.2 ValidateTokensStep

- Executes if `ctx.IdToken` is present.
- Downloads JWKS.
- Validates:
  - issuer
  - audience
  - signature
  - lifetime
- Extracts:
  - subject
  - string claims

Failure → `OidcProtocolException`.

---

## 5.3 FetchUserInfoStep

- Executes if `ctx.AccessToken` is present.
- Calls UserInfo endpoint.
- Extracts:
  - subject
  - claims
  - email
  - given/family name
  - picture

---

# 6. OIDC Callback Context Builder

````csharp
public sealed class OidcAuthCallbackContextBuilder
    : ImmutableContextBuilderBase<OidcAuthCallbackContext, IImmutableContextBuildStep<OidcAuthCallbackContext>>,
      IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>
````

### Responsibilities

- Initialize context with:
  - `Code`
  - `Now = UtcNow`
- Execute all steps in order.
- Enforce immutability invariants.
- Emit diagnostic events.
- Produce a `FrankAuthCallbackResult`.

### Critical invariant

If the pipeline completes without a `SubjectId`, the builder throws:

```
OidcProtocolException("OIDC pipeline completed without a SubjectId.")
```

---

# 7. DI Registration

````csharp
public static IServiceCollection AddFrankAuthCallback(this IServiceCollection services)
{
    services.AddOptions<AuthCallbackOidcSettings>()
            .BindConfiguration("Authentication:Callback:Oidc")
            .ValidateDataAnnotations()
            .ValidateOnStart();

    services.AddTransient<IImmutableContextBuildStep<OidcAuthCallbackContext>, ExchangeCodeStep>();
    services.AddTransient<IImmutableContextBuildStep<OidcAuthCallbackContext>, FetchUserInfoStep>();
    services.AddTransient<IImmutableContextBuildStep<OidcAuthCallbackContext>, ValidateTokensStep>();

    services.AddTransient<IImmutableContextBuilder<FrankAuthCallbackRequest, OidcAuthCallbackContext, FrankAuthCallbackResult>,
        OidcAuthCallbackContextBuilder>();

    return services;
}
````

### Notes

- Steps are registered as **transient**.
- The builder is registered as **transient**.
- Settings are validated on startup.

---

# 8. Identity Resolution

The pipeline produces a `FrankAuthCallbackResult`.

Downstream, the identity resolver maps this to a **Frank internal identity**:

````csharp
public interface IIdentityResolver
{
    Task<Guid> ResolveAsync(
        FrankAuthCallbackResult authCallbackResult,
        CancellationToken cancellationToken);
}
````

This is where:

- new users may be created  
- existing users may be matched  
- identity is normalized into a Frank `Guid`  

---

# 9. Developer Invariants

Developers must ensure:

- Pipeline steps **never mutate immutable fields** (`Code`, `Now`).
- Steps must return a **new context** via `with`.
- Steps must be **pure** except for HTTP calls.
- Steps must throw `OidcProtocolException` on protocol violations.
- The builder must enforce:
  - non‑null context transitions
  - immutability checks
  - diagnostic events

---

# 10. Anti‑Patterns

Avoid:

- Mutating the context directly.
- Returning `null` from a step.
- Swallowing OIDC protocol errors.
- Adding side effects (database writes, logging to external systems).
- Mixing domain logic into the callback pipeline.
- Persisting identity provider tokens.

---

# 11. Summary

The Authentication Callback capability provides:

- A deterministic, immutable OIDC callback pipeline.
- Strict validation of tokens and claims.
- A normalized identity result for downstream systems.
- A clean separation between:
  - protocol handling
  - identity resolution
  - application logic

This Developer Guide documents everything needed to extend or integrate with the Authentication Callback capability.
