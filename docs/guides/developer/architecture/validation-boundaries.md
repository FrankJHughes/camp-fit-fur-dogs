# Validation Boundaries  
**API vs Application vs Domain**

This document defines the validation responsibilities across the three backend layers: **API**, **Application**, and **Domain**.  
It ensures contributors understand *where* validation belongs, *why*, and *how* each layer interacts with the others.

This is a foundational architecture guide and complements:

- [API Endpoint Purity Guide](ca://s?q=Show_API_endpoint_purity_guide)  
- [Dispatcher Pipeline Guide](ca://s?q=Show_dispatcher_pipeline_guide)  
- [Architecture Governance](ca://s?q=Open_architecture_governance)

---

# Why This Matters

Clear validation boundaries prevent:

- Validation rules scattering across layers  
- Domain logic leaking into API endpoints  
- Application services becoming bloated  
- Duplicate or inconsistent validation  
- Incorrect error codes  
- Incorrect test placement  

This guide ensures the system remains **predictable**, **pure**, and **maintainable**.

---

# Layer Responsibilities Overview

Validation is divided into three categories:

1. **API Layer — Syntactic Validation**  
2. **Application Layer — Semantic Validation**  
3. **Domain Layer — Invariant Validation**

Each layer has a distinct purpose and must not leak into the others.

---

# 1. API Layer — **Syntactic Validation**

The API layer validates **shape**, **format**, and **presence** of incoming data.

This is the “Is the request well‑formed?” layer.

### Responsibilities

- Required fields  
- Basic type checks  
- Length constraints  
- Email format  
- Phone format  
- Enum parsing  
- Query string validation  
- Route parameter validation  

### What the API layer must *not* do

- No business rules  
- No cross‑field logic  
- No repository access  
- No domain invariants  
- No entity creation  
- No persistence  
- No identity resolution  

### Error Type

- `ValidationError` → **400 Bad Request**

### Examples

- `"email" must be a valid email format`  
- `"firstName" is required`  
- `"page" must be a positive integer`

API validation ensures the request is **syntactically correct**, nothing more.

---

# 2. Application Layer — **Semantic Validation**

The Application layer validates **business meaning** and **cross‑field rules**.

This is the “Does this make sense for the business?” layer.

### Responsibilities

- Rules requiring repository access  
- Rules requiring current user context  
- Cross‑field business rules  
- Existence checks  
- Uniqueness checks  
- Authorization checks  
- Workflow rules  

### What the Application layer must *not* do

- No syntactic validation  
- No domain invariant enforcement (that’s Domain)  
- No persistence logic outside use cases  
- No HTTP or API concerns  

### Error Type

- `ValidationError` → **400 Bad Request**  
- `DomainError` → **400 Bad Request** (if domain throws)

### Examples

- `"email" must be unique"`  
- `"owner must exist before creating a pet"`  
- `"cannot delete a customer with active bookings"`

Application validation ensures the request is **semantically valid**.

---

# 3. Domain Layer — **Invariant Validation**

The Domain layer enforces **rules that must always be true**, regardless of context.

This is the “What must always be true in the business universe?” layer.

### Responsibilities

- Value object invariants  
- Entity invariants  
- Aggregate invariants  
- Canonicalization  
- Internal consistency  
- Rules that must hold *forever*  

### What the Domain layer must *not* do

- No external calls  
- No repository access  
- No environment access  
- No HTTP calls  
- No configuration access  
- No user context logic  

### Error Type

- `DomainError` → **400 Bad Request**

### Examples

- `"Email" value object requires a valid email format`  
- `"Name" cannot be empty"`  
- `"BookingDate" must be in the future"`  
- `"PasswordHash" must be a valid hash format"`

Domain validation ensures the **business invariants** are upheld.

---

# Error Mapping Summary

| Layer | Error Type | HTTP Status | Notes |
|-------|------------|-------------|-------|
| API | `ValidationError` | 400 | Syntactic issues |
| Application | `ValidationError` | 400 | Semantic issues |
| Application | `DomainError` | 400 | Domain invariant violation |
| Domain | `DomainError` | 400 | Thrown by entities/VOs |
| External Provider | `ExternalAuthProviderFailure` | 502 | Auth0, email provider, etc. |
| Configuration | `BadConfiguration` | 500 | Missing config keys |
| Unexpected | `Unexpected` | 500 | Catch‑all |

This table is consistent with **Security Governance**, **Operations Governance**, and Frank’s error boundary.

---

# Testing Strategy

Validation must be tested at the correct layer.

---

## API Tests — **Syntactic**

Validate:

- Missing fields  
- Invalid formats  
- Bad query parameters  
- Bad route parameters  

These tests ensure the API rejects malformed requests.

---

## Application Tests — **Semantic**

Validate:

- Uniqueness  
- Existence  
- Authorization  
- Workflow rules  

These tests ensure business rules are enforced.

---

## Domain Tests — **Invariants**

Validate:

- Value object construction  
- Entity creation  
- Aggregate rules  

These tests ensure domain invariants cannot be violated.

---

# Examples

## Example 1 — Email Field

| Layer | Rule | Example |
|-------|------|---------|
| API | Must be valid email format | `"not-an-email"` → 400 |
| Application | Must be unique | `"email already exists"` → 400 |
| Domain | Must satisfy Email VO invariant | `"@missing-local-part"` → DomainError |

---

## Example 2 — Booking Creation

| Layer | Rule | Example |
|-------|------|---------|
| API | Required fields | Missing `startDate` |
| Application | Owner must exist | OwnerId not found |
| Application | No overlapping bookings | Conflict with existing booking |
| Domain | BookingDate must be future | Past date |

---

# Summary

- **API = shape + format**  
- **Application = business meaning**  
- **Domain = invariants**  

Each layer has a clear, non‑overlapping responsibility.  
Following these boundaries keeps the system pure, testable, and maintainable.

---

# Related Documents

- [API Endpoint Purity Guide](ca://s?q=Show_API_endpoint_purity_guide)  
- [Dispatcher Pipeline](ca://s?q=Show_dispatcher_pipeline_guide)  
- [Authentication Overview](ca://s?q=Show_authentication_overview)  
- [Architecture Governance](ca://s?q=Open_architecture_governance)
