# Auth Callback Architecture

This guide explains the authentication callback architecture used by
CampFitFurDogs after the refactor to:

- a Frank‑owned **OIDC protocol pipeline**
- an Application‑owned **business pipeline**
- a thin **API callback endpoint**

The previous `AuthCallbackExecutor` / `IAuthCallbackStep` engine has been fully
replaced by immutable context builders and a strict three‑layer architecture.

Local identity, local registration, and password flows have been removed.  
The system is now **OIDC‑exclusive**.

---

## Purpose

The authentication callback architecture orchestrates the OIDC
authorization‑code callback flow in a deterministic, testable, and governed way.

The flow performs:

- configuration validation  
- authorization code exchange  
- claims extraction and normalization  
- identity resolution  
- customer creation or lookup  
- session creation  
- redirect URL computation  
- cookie value computation  
- cookie issuance + redirect  

Each responsibility is implemented in the correct layer:

- **Frank** — protocol  
- **Application** — business  
- **API** — boundary  

---

## When to Use This Guide

Use this guide when:

- implementing or modifying authentication callback behavior  
- adding a new identity provider  
- updating redirect logic  
- writing tests for callback behavior  
- onboarding new engineers to the authentication system  

---

## Architecture Overview

```text
API Callback Endpoint
    ↓
Frank Auth Callback Pipeline (protocol)
    ↓
Application Auth Callback Pipeline (business)
    ↓
API issues cookie + redirect
```

| Layer | Responsibility |
|-------|----------------|
| API | HTTP boundary, parameter extraction, cookie issuance, redirect |
| Frank | OIDC protocol, token exchange, claims extraction |
| Application | identity resolution, session creation, redirect + cookie value |

Each pipeline is implemented as an immutable context builder.

---

## Frank Auth Callback Pipeline

Frank owns the **protocol layer**.

```csharp
IImmutableContextBuilder<
    FrankAuthCallbackRequest,
    OidcAuthCallbackContext,
    FrankAuthCallbackResult>
```

### Responsibilities

- validate OIDC configuration  
- exchange authorization code  
- fetch and parse userinfo (if required)  
- normalize provider‑specific claims  
- produce a protocol‑level result for Application  

### Non‑Responsibilities

Frank must not:

- perform business logic  
- create customers or sessions  
- issue cookies  
- compute redirect URLs  
- interpret `returnUrl`  

Frank is **pure protocol**.

---

## Application Auth Callback Pipeline

Application owns the **business layer**.

```csharp
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
```

### Responsibilities

Given the normalized protocol result, Application must:

- resolve identity  
- create or load customer  
- create session  
- compute redirect URL  
- compute cookie value  
- produce a complete result for the API boundary  

`ApplicationAuthCallbackContextBuilderResult` includes:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue`  
- `RedirectUrl`  

### Redirect URL Rules

Application must:

- accept `returnUrl` from the API  
- validate that it is local and safe  
- sanitize or reject unsafe values  
- compute the final redirect target  

### Non‑Responsibilities

Application must not:

- perform OIDC protocol logic  
- exchange tokens  
- issue cookies  
- access HttpContext  

Application is **pure business**.

---

## API Callback Endpoint

The API callback endpoint is a thin orchestrator.

### Responsibilities

- extract `code`  
- extract optional `returnUrl`  
- validate that `code` is present  
- invoke Frank pipeline  
- invoke Application pipeline  
- issue session cookie using `CookieValue`  
- redirect to `RedirectUrl`  

### Non‑Responsibilities

The API endpoint must not:

- perform protocol logic  
- perform business logic  
- compute redirect URLs  
- validate `returnUrl`  
- generate cookie values  

The endpoint is **boundary‑only**.

---

## Immutable Context Builder Pattern

Both pipelines use:

```csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
```

### Rules

- request, context, and result types must be immutable  
- builders must be deterministic  
- builders must not mutate external state  
- Application builders must not perform HTTP  
- Frank builders must not perform persistence  

---

## Error Model

### API

- missing `code` → `400 Bad Request`  
- all other errors → global error boundary  

### Frank

- protocol failures  
- token exchange errors  
- invalid provider responses  

### Application

- identity resolution failures  
- session creation failures  

Errors are shaped by:

- Frank’s error boundary  
- ProblemDetails conventions  
- API governance  

---

## Observability

Logs must capture:

- API: callback invocation, redirect target (non‑PII)  
- Frank: protocol events  
- Application: identity/session events  

Logs must never contain:

- authorization codes  
- tokens  
- PII  
- provider error messages  

---

## Testing Strategy

### Frank Pipeline Tests

- protocol success/failure  
- invalid configuration  
- claims normalization  

### Application Pipeline Tests

- new/existing customer flows  
- session creation  
- redirect URL selection  
- cookie value computation  

### API Endpoint Tests

- missing `code` → `400`  
- valid `code` → cookie + redirect  
- `returnUrl` flows correctly  
- error propagation  

Tests must use:

- fake Frank pipeline  
- fake Application pipeline  
- `ApiFactory` + `ApiContext`  

---

## Migration Notes

The old engine:

- `AuthCallbackExecutor`  
- `IAuthCallbackStep`  
- `AuthCallbackContext`  

has been replaced by:

- Frank immutable context builder  
- Application immutable context builder  
- thin API endpoint  

Redirect logic now lives entirely in Application.  
`returnUrl` is validated only in Application.

---

## Summary

The Auth Callback Architecture is:

- OIDC‑exclusive  
- three‑layered  
- immutable  
- deterministic  
- governed  
- safe  
- extensible  

This is the canonical model for all authentication callback behavior going forward.
