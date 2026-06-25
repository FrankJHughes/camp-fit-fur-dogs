# Authentication Architecture — Developer Guide

This guide documents the current authentication architecture implemented across:

- US‑110 — Authentication: Owner Login (OIDC)  
- US‑111 — Authentication: Session Management  
- US‑184 — De‑feature Local Identity  

It describes how the system performs OIDC login, executes the authentication callback, resolves identity, creates sessions, and issues cookies — using the exclusive OIDC authentication model and the three‑layer authentication callback architecture:

1. OIDC Login Initiation  
2. Frank Auth Callback Pipeline (protocol)  
3. Application Auth Callback Pipeline (business)  
4. API Callback Endpoint (boundary orchestration)  

This guide explains **how authentication works today**.  
Governance rules live in:

- Architecture Governance  
- API Governance  
- Security Governance  
- Code Conventions  
- ADRs  

---

# Purpose

The authentication architecture orchestrates the complete OIDC authorization‑code flow:

- configuration validation  
- authorization code exchange  
- claims extraction and normalization  
- identity resolution  
- session creation  
- redirect URL computation  
- cookie value computation  
- cookie issuance + redirect  

Each responsibility is implemented in the correct layer:

- **Frank** — protocol  
- **Application** — business  
- **API** — boundary  

The architecture is:

- deterministic  
- immutable  
- testable  
- governed  
- cross‑cutting  
- aligned with Frank’s engine model  

---

# High‑Level Architecture

Authentication consists of three cooperating components:

1. OIDC Login Initiation  
2. Auth Callback Architecture (Frank + Application + API)  
3. Session Issuance + Identity Mapping + Audit Logging  

Authentication spans:

- **API** — endpoint orchestration, cookie issuance, redirect  
- **Frank** — OIDC protocol pipeline  
- **Application** — identity resolution, session creation, redirect + cookie value  
- **Domain** — Owner + Session invariants  
- **Infrastructure** — Auth0 client, repositories, audit logger  

Local identity, local registration, and password flows have been fully removed.

---

# OIDC Login Initiation

The login initiation endpoint:

- generates the Auth0 authorization URL  
- includes the correct redirect URI  
- includes PKCE parameters  
- includes the optional `returnUrl` parameter  
- redirects the browser to Auth0  

This endpoint:

- contains no identity logic  
- contains no session logic  
- contains no business logic  
- uses only configuration + URL generation  
- relies on Frank middleware for security headers and CORS  

It simply begins the OIDC flow.

---

# Auth Callback Architecture

The old dispatcher‑based callback engine has been replaced by a three‑layer architecture:

````text
API Callback Endpoint
    ↓
Frank Auth Callback Pipeline (protocol)
    ↓
Application Auth Callback Pipeline (business)
    ↓
API issues cookie + redirect
````

This architecture is:

- deterministic  
- immutable  
- testable  
- governed  
- cross‑cutting  
- aligned with Frank’s engine model  

---

# Frank Auth Callback Pipeline (Protocol Layer)

Frank owns the OIDC protocol.

````csharp
IImmutableContextBuilder<
    FrankAuthCallbackRequest,
    OidcAuthCallbackContext,
    FrankAuthCallbackResult>
````

## Responsibilities

Frank must:

- validate OIDC configuration  
- exchange authorization code for tokens  
- validate issuer, audience, signature, nonce, state  
- fetch userinfo (if required)  
- normalize provider‑specific claims  
- produce a stable, provider‑agnostic result  

## Non‑Responsibilities

Frank must **not**:

- perform business logic  
- resolve identity  
- create sessions  
- compute redirect URLs  
- interpret or validate `returnUrl`  
- compute cookie values  

Frank’s pipeline is **pure protocol**.

---

# Application Auth Callback Pipeline (Business Layer)

Application owns identity + session + redirect logic.

````csharp
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
````

## Responsibilities

Given the normalized protocol result, Application must:

- resolve identity (find or create Owner)  
- create or load Owner  
- create session  
- compute redirect URL  
  - if `returnUrl` is provided → validate + use it  
  - otherwise → use default post‑login redirect  
- compute cookie value (opaque, secure)  

The result object includes:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue`  
- `RedirectUrl`  

## Redirect URL Rules

Application must:

- accept `returnUrl` from the API endpoint  
- validate that `returnUrl` is:
  - local  
  - safe  
  - non‑malicious  
- reject or sanitize unsafe values  
- produce the final `RedirectUrl`  

## Non‑Responsibilities

Application must **not**:

- perform OIDC protocol logic  
- exchange tokens  
- call identity providers  
- issue cookies  
- access HttpContext  

Application’s pipeline is **pure business**.

---

# API Callback Endpoint (Boundary Layer)

The API callback endpoint is a thin orchestrator.

## Responsibilities

- extract the `code` query parameter  
- extract the optional `returnUrl` query parameter  
- validate that `code` is present  
- invoke the Frank pipeline  
- invoke the Application pipeline  
- issue the session cookie using `CookieValue`  
- redirect to `RedirectUrl`  

## Non‑Responsibilities

The API endpoint must **not**:

- perform protocol logic  
- perform business logic  
- perform persistence  
- call identity providers  
- compute redirect URLs  
- validate or sanitize `returnUrl`  
- generate cookie values  

The endpoint is responsible only for **HTTP boundary behavior**.

---

# Immutable Context Builder Pattern

Both pipelines use the same architectural pattern:

````csharp
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
{
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
````

## Rules

- all types (`TRequest`, `TContext`, `TResult`) must be immutable  
- builders must be deterministic  
- builders must not mutate external state  
- builders must not perform HTTP in Application layer  
- builders must not perform persistence in Frank layer  

This replaces the old step engine with a simpler, strongly typed pipeline.

---

# Identity Mapping

Identity mapping is performed exclusively in the Application pipeline.

- external identity → internal domain identity  
- implemented via `IIdentityResolver`  
- pure and deterministic  
- never uses email  
- never exposes internal IDs to Auth0  
- never touches Infrastructure directly  
- never performs protocol logic  

Identity mapping is part of the OIDC‑exclusive authentication model.  
The external identity (`sub`) is the only identity input into the system.

See the Identity Mapping Guide for details.

---

# Session Issuance

Session issuance occurs only in the Application pipeline and the API boundary.

1. token generation (Application)  
2. token hashing (Application)  
3. session creation (Application)  
4. cookie issuance (API)  
5. redirect (API using Application result)  

Session tokens:

- are 256‑bit random values  
- are hashed using SHA‑256  
- are stored only as hashes  
- are issued as secure cookies  
- are opaque (never JWTs, never contain identity data)  

Production cookie flags:

- `Secure=true`  
- `HttpOnly=true`  
- `SameSite=Lax`  

Local development uses `Secure=false`.

---

# Audit Logging

Audit logging occurs in the Application pipeline, immediately after identity resolution:

- logs `CustomerId` + external identity (`sub`)  
- does not mutate state  
- runs before session creation  
- ensures login events are recorded even if session creation fails  
- never logs tokens, authorization codes, or PII  

---

# Architectural Boundaries

Authentication spans four layers:

## API Layer
- defines endpoints  
- extracts `code` and optional `returnUrl`  
- orchestrates Frank + Application pipelines  
- issues cookies  
- returns redirect responses  
- uses Frank error boundary, security headers, and CORS  
- performs **no protocol logic** and **no business logic**  

## Frank Layer
- performs OIDC protocol logic only  
- exchanges authorization code  
- validates tokens  
- normalizes claims  
- produces protocol‑level result  
- contains **no business logic**  
- does not interpret or validate `returnUrl`  

## Application Layer
- performs business logic only  
- identity resolution  
- Owner creation  
- session creation  
- redirect URL computation  
- cookie value computation  
- contains **no protocol logic**  

## Domain Layer
- owns Owner aggregate  
- owns Session entity  
- owns invariants  
- contains **no Infrastructure or API references**  

---

# Data Flow Diagram

````text
Browser → /api/auth/login?returnUrl=/dashboard
    → Redirect to Auth0 (with returnUrl encoded in state)

Auth0 → /api/auth/callback?code=XYZ&state=...
    → Frank pipeline (protocol)
    → Application pipeline (business + returnUrl validation)
    → API issues cookie + redirect
````

---

# Error Handling

Authentication errors fall into these categories:

## API Errors — 400
- missing authorization code  
- missing or malformed state  
- invalid `returnUrl` (if Application rejects it)  

## Protocol Errors — 502
- token exchange failure  
- missing access token  
- userinfo failure  
- missing external ID (`sub`)  

## Business Errors — 500
- identity resolver failure  
- Owner creation failure  
- session creation failure  

All errors are surfaced via:

- Frank error boundary  
- ProblemDetails JSON  
- logged exceptions  
- no cookies issued  

---

# Observability

The callback flow must be observable at each layer:

## API
- callback invocation  
- cookie issuance  
- redirect target (non‑PII)  

## Frank
- protocol events (token exchange, claims extraction)  
- step‑level diagnostics from the ImmutableContextBuilder  
- before/after snapshots (non‑PII)  

## Application
- identity resolution  
- session creation  
- redirect decision  
- step‑level diagnostics  

Logs must **never** contain:

- authorization codes  
- tokens  
- PII  
- provider error messages  

---

# Testing Architecture

Authentication is tested at three levels:

## Frank Pipeline Tests
- protocol success and failure paths  
- claims normalization  
- provider error handling  
- state and nonce validation  

## Application Pipeline Tests
- identity resolution  
- session creation  
- redirect computation (with and without `returnUrl`)  
- cookie value computation  

## API Endpoint Tests
- missing `code` → 400  
- valid `code` → cookie issued + redirect  
- `returnUrl` flows correctly into Application  
- error propagation into global error boundary  

Tests use:

- fake Frank pipeline  
- fake Application pipeline  
- `ApiFactory` + `ApiContext`  
- deterministic configuration overrides  
- no real external services  

---

# Local Development Notes

## Auth0

Local development uses the following callback URL:

````text
http://localhost:5000/api/auth/callback
````

## Cookies

Local development:

- `Secure=false`  
- `SameSite=Lax`  

Preview and production:

- `Secure=true`  
- `HttpOnly=true`  
- `SameSite=Lax`  

These flags are enforced by the API boundary and configured via environment‑specific cookie policies.

---

# Related Documents

- Session Management Guide  
- Identity Mapping Guide  
- Authentication Testing Guide  
- Authentication Operations Guide  
- Architecture Governance  
- API Governance  
- Security Governance  
