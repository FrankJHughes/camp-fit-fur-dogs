# OIDC Protocol Code Conventions (Frank)

Frank provides a **pure OIDC protocol pipeline** that performs all protocol‑level mechanics required for OpenID Connect authentication.  
This pipeline is consumed by product authentication layers (e.g., CampFitFurDogs) but does **not** perform any business logic, persistence, session creation, or redirect computation.

These conventions define **how the OIDC protocol pipeline must be implemented**.

---

## Purpose

The OIDC protocol pipeline in Frank exists to:

- exchange authorization codes  
- retrieve tokens from the identity provider  
- validate tokens and protocol parameters  
- extract and normalize claims  
- enforce required claims  
- produce a **protocol‑normalized result**  

The pipeline must remain **pure**, **deterministic**, and **side‑effect‑free**.

---

## Immutable Context Builder Requirements

The OIDC protocol pipeline must be implemented using Frank’s `ImmutableContextBuilder` pattern.

Each builder defines:

- **TRequest** — immutable input  
- **TContext** — immutable working context  
- **TResult** — immutable output  

All types must:

- be immutable  
- use required members  
- contain no behavior  
- contain no side effects  
- contain no product dependencies  

The pipeline must operate exclusively through **immutable context transitions** and **step‑level observability**.

---

## Responsibilities

The OIDC protocol pipeline must:

- validate the authorization code  
- call the IdP token endpoint via HttpClient abstractions  
- validate:
  - issuer  
  - audience  
  - signature  
  - expiry  
  - nonce  
  - state  
- extract claims from the ID token  
- normalize provider‑specific claim formats  
- enforce required claims (e.g., subject, email if required)  
- produce a **protocol context** containing:
  - subject identifier  
  - normalized claims  
  - token metadata  
  - protocol status  

The pipeline must **not**:

- create or load users  
- create sessions  
- compute redirect URLs  
- issue cookies  
- perform persistence  
- call product services  
- depend on product types  

Frank is the **protocol layer only**.

---

## HttpClient Usage

All outbound calls to the identity provider must:

- use **named HttpClients**  
- use Frank’s HttpClient seam  
- never use `new HttpClient()`  
- never perform direct network calls in tests  

Tests must replace HttpClient with:

- `FakeHttpMessageHandler`  
- `WithServiceOverride`  

This ensures deterministic, isolated, fully testable protocol behavior.

---

## Error Behavior

The OIDC protocol pipeline must:

- throw mapped exceptions for protocol failures  
- never throw raw HttpClient exceptions  
- never expose IdP error payloads directly  
- never leak sensitive token data  

Products are responsible for shaping these exceptions into HTTP responses.

---

## Required Validation Rules

The pipeline must validate:

- `code` is present  
- `state` matches expected value  
- token issuer matches configured issuer  
- token audience matches configured client ID  
- token signature is valid  
- token is not expired  
- nonce matches expected value  
- required claims exist  

Missing or invalid values must result in a **protocol failure**.

---

## Claim Normalization

The pipeline must normalize:

- subject identifier  
- email (if provided)  
- name (if provided)  
- provider‑specific claim keys  

Normalization must produce a **stable, provider‑agnostic** claim set.

---

## DI and Registration

The OIDC protocol pipeline:

- must be auto‑registered via Frank’s DI engine  
- must not require manual registration  
- must not depend on product DI  
- must expose only protocol‑level abstractions  

Frank remains isolated from product concerns.

---

## Testability Requirements

The pipeline must be fully testable using:

- fake HttpClient handlers  
- fake environment seams  
- fake configuration seams  

Tests must assert:

- correct token validation behavior  
- correct claim normalization  
- correct error mapping  
- correct protocol context output  

Tests must not:

- hit real identity providers  
- require real environment variables  
- require real network calls  

The protocol pipeline must be **fully deterministic and hermetic**.

---

## Prohibitions

The OIDC protocol pipeline must not:

- depend on product code  
- depend on product configuration formats  
- depend on product domain models  
- perform any persistence  
- perform any business logic  
- compute redirect URLs  
- issue cookies  
- create sessions  
- load users  

It is strictly a **protocol‑only** component.
