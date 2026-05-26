`````markdown
# Validation Boundaries  
**API vs Application vs Domain**

This document defines the validation responsibilities across the three backend layers: **API**, **Application**, and **Domain**.  
It ensures contributors understand *where* validation belongs, *why*, and *how* each layer interacts with the others.

This is a foundational architecture guide and complements the existing API Endpoint Purity and Dispatcher Pipeline guides.

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

## 1. API Layer — **Syntactic Validation**
The API layer validates **shape**, **format**, and **presence** of incoming data.

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

### Error Type
- `ValidationError` → **400 Bad Request**

### Example
- `"email" must be a valid email format`  
- `"firstName" is required`  
- `"page" must be a positive integer`

---

## 2. Application Layer — **Semantic Validation**
The Application layer validates **business meaning** and **cross‑field rules**.

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
- No domain invariant enforcement (that’s domain)  
- No persistence logic outside use cases  

### Error Type
- `ValidationError` → **400 Bad Request**  
- `DomainError` → **400 Bad Request** (if domain throws)

### Example
- `"email" must be unique`  
- `"owner must exist before creating a pet"`  
- `"cannot delete a customer with active bookings"`

---

## 3. Domain Layer — **Invariant Validation**
The Domain layer enforces **rules that must always be true**, regardless of context.

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

### Error Type
- `DomainError` → **400 Bad Request**

### Example
- `"Email" value object requires a valid email format`  
- `"Name" cannot be empty`  
- `"BookingDate" must be in the future`  
- `"PasswordHash" must be a valid hash format`

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

---

# Testing Strategy

## API Tests
Validate **syntactic** rules:
- Missing fields  
- Invalid formats  
- Bad query parameters  
- Bad route parameters  

## Application Tests
Validate **semantic** rules:
- Uniqueness  
- Existence  
- Authorization  
- Workflow rules  

## Domain Tests
Validate **invariants**:
- Value object construction  
- Entity creation  
- Aggregate rules  

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

See also:  
- [API Endpoint Purity Guide](ca://s?q=Show_API_endpoint_purity_guide)  
- [Dispatcher Pipeline](ca://s?q=Show_dispatcher_pipeline_guide)  
- [Authentication Overview](ca://s?q=Show_authentication_overview)
`````
