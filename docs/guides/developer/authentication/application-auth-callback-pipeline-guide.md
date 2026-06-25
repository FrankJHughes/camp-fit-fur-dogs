# Application Auth Callback Pipeline — Developer Guide

The Application Auth Callback Pipeline processes a normalized external identity and produces a fully authenticated internal identity. It performs all **business‑level authentication work**:

- identity mapping  
- Owner resolution  
- Owner creation  
- session creation  
- redirect computation  
- cookie value computation  

This pipeline contains **no protocol logic** and **no HTTP concerns**.  
It is the **business core** of the authentication flow.

---

# Purpose

The pipeline transforms a normalized external identity (from the Frank pipeline) into:

- an internal `CustomerId`  
- a persisted Owner record (if first login)  
- a persisted session  
- a computed redirect URL  
- a cookie value for the API to issue  

It ensures that identity mapping, Owner creation, and session creation follow consistent business rules.

---

# Inputs and Outputs

## Input

A normalized external identity containing:

- `sub` (stable external identifier)  
- email (optional)  
- name fields (optional)  
- provider metadata  

This identity is already:

- retrieved via OIDC  
- normalized by the Frank pipeline  
- free of protocol‑level concerns  

## Output

A result object containing:

- `CustomerId`  
- `SessionId`  
- `RedirectUrl`  
- `CookieValue` (opaque session token)  
- any additional metadata needed by the API boundary  

---

# Responsibilities

The pipeline performs the following steps:

---

## 1. Validate external identity

- ensure required claims (such as `sub`) are present  
- enforce minimal business rules on identity data  

---

## 2. Identity mapping

- map external identity → internal identity model  
- determine the stable lookup key for the Owner  

---

## 3. Owner resolution

- look up an existing Owner by external ID  
- if found → load the Owner  
- if not found → proceed to Owner creation  

---

## 4. Owner creation (Create Customer slice)

- create a new Owner aggregate  
- enforce domain invariants  
- persist the Owner  
- produce a new `CustomerId`  

---

## 5. Session creation

- create a new session for the Owner  
- generate a session token  
- hash and store the token  
- persist the session  
- produce `SessionId` and raw token  

---

## 6. Redirect computation

- determine the post‑login redirect URL  
- validate and apply `returnUrl` (if provided)  
- apply business rules for first‑time vs returning users  

---

## 7. Cookie value computation

- convert the raw session token into an opaque cookie value  
- ensure it is suitable for secure cookie issuance by the API  

The pipeline returns a **single cohesive result object**.

---

# Architecture and Structure

The pipeline is implemented using the **ImmutableContextBuilder** pattern.

## Conceptual structure

````text
Application/Authentication/Callback/
    ApplicationAuthCallbackPipeline.cs
    IdentityMappingBehavior.cs
    OwnerResolutionBehavior.cs
    CreateCustomerBehavior.cs
    SessionCreationBehavior.cs
    RedirectComputationBehavior.cs
    CookieComputationBehavior.cs
````

Each behavior:

- accepts an immutable context  
- produces a new context with additional data  
- contains no side effects outside its responsibility  
- is composable and testable in isolation  

The pipeline composes these behaviors into a single flow.

---

# Separation of Concerns

The Application pipeline is strictly separated from:

---

## Frank pipeline (protocol)

- no HTTP  
- no token exchange  
- no userinfo retrieval  
- no protocol error handling  

---

## API endpoint (boundary)

- no cookie issuance  
- no redirect execution  
- no HTTP response construction  

---

## Frontend

- no UI logic  
- no routing logic  

---

The pipeline focuses solely on business decisions:

- who is this user?  
- do they already exist?  
- should a new Owner be created?  
- what session should be created?  
- where should they be redirected?  

---

# Identity Mapping

Identity mapping converts the normalized external identity into the internal identity model.

## Responsibilities

- determine the stable identity key  
- map external → internal identity  
- handle optional fields  
- provide consistent lookup rules  

## Typical rules

- use `sub` as the primary stable identifier  
- treat email as profile data, not identity  
- never derive identity from mutable fields  

Identity mapping is pure, deterministic, and isolated from Infrastructure except through abstractions.

---

# Owner Resolution

Owner resolution determines whether the user already exists.

## Steps

1. lookup Owner by external ID  
2. if found → reuse existing Owner  
3. if not found → create new Owner  

Resolution is:

- deterministic  
- pure  
- invariant‑checked  
- performed through repository abstractions  

---

# Owner Creation

Owner creation is delegated to the **Create Customer** slice.

## Responsibilities

- construct a valid Owner aggregate  
- enforce domain invariants  
- persist the Owner  
- return the new `CustomerId`  

Owner creation is performed **only** when identity mapping indicates a first‑time login.

---

# Session Creation

Session creation produces a new authenticated session.

## Responsibilities

- generate a 256‑bit random token  
- hash the token using SHA‑256  
- persist the session  
- return `SessionId` and raw token  

The raw token is never stored — only the hash is persisted.

---

# Redirect Computation

Redirect computation determines where the user should go after login.

## Rules

- if `returnUrl` is provided → validate + use it  
- otherwise → use default post‑login redirect  
- ensure redirect is local, safe, and non‑malicious  

Redirect computation is pure business logic.

---

# Cookie Value Computation

The cookie value is derived from the raw session token.

## Responsibilities

- produce an opaque, secure cookie value  
- ensure compatibility with API cookie issuance  
- ensure no identity data is embedded  

The API boundary issues the cookie — the Application pipeline only computes the value.

---

# Purity and Boundaries

The Application Auth Callback Pipeline must remain pure:

- no protocol logic  
- no HTTP  
- no cookie issuance  
- no redirect execution  
- no environment access  
- no hosting logic  

It orchestrates business decisions using:

- immutable context  
- deterministic transformations  
- invariant‑checked behaviors  

---

# Testing Strategy

Testing occurs at three levels:

---

## Unit Tests

- identity mapping  
- Owner resolution  
- Owner creation  
- session creation  
- redirect computation  
- cookie value computation  

---

## Pipeline Tests

- full Application pipeline  
- correct ordering  
- correct invariant enforcement  
- correct context transitions  

---

## Integration Tests

- full callback flow  
- Owner creation on first login  
- Owner reuse on subsequent logins  
- session creation  
- cookie issuance (API boundary)  
- redirect correctness  

Integration tests use `ApiContext` + `ApiFactory`.

---

# Summary

The Application Auth Callback Pipeline:

- receives a normalized external identity  
- resolves or creates an Owner  
- creates a session  
- computes redirect and cookie value  
- returns a cohesive result for the API boundary  

It is:

- pure  
- deterministic  
- invariant‑checked  
- strongly typed  
- implemented using ImmutableContextBuilder  
- the business core of the authentication flow  
