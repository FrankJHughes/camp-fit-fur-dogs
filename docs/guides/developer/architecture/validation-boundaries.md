# Validation Boundaries  
*A developer‑facing architecture guide for API, Application, and Domain validation.*

Validation in CampFitFurDogs is intentionally layered.  
Each backend layer has a **distinct validation responsibility**, and understanding these boundaries is essential for writing predictable, maintainable, and testable code.

This guide explains:

- What each layer validates  
- Why validation is split across layers  
- How validation flows through the system  
- How errors surface  
- How to test validation correctly  

It complements:

- API Endpoint Purity Guide  
- Dispatcher Pipeline Guide  
- Domain Events Architecture  
- Architecture Overview  

---

# 1. Why Validation Is Layered

Validation is not a single concern — it spans three layers:

1. **API — Syntactic validation**  
2. **Application — Semantic validation**  
3. **Domain — Invariant validation**

Each layer answers a different question:

| Layer | Question |
|-------|----------|
| **API** | “Is the request well‑formed?” |
| **Application** | “Does this make sense for the business?” |
| **Domain** | “What must always be true?” |

Keeping these responsibilities separate prevents:

- Duplicate validation  
- Business logic leaking into API endpoints  
- Domain invariants leaking into handlers  
- Repositories being used in validators  
- Incorrect error codes  
- Incorrect test placement  

---

# 2. High‑Level Flow

A request flows through validation in this order:

```
Client Request
    ↓
API Layer (syntactic validation)
    ↓
Application Layer (semantic validation)
    ↓
Domain Layer (invariant validation)
    ↓
Handler executes use case
```

Each layer builds on the previous one.

---

# 3. API Layer — Syntactic Validation  
**“Is the request well‑formed?”**

The API layer validates **shape**, **format**, and **presence** of incoming data.

This is the first line of defense.

### Responsibilities

- Required fields  
- Basic type checks  
- Length constraints  
- Email format  
- Phone format  
- Enum parsing  
- Query string validation  
- Route parameter validation  

### What the API layer does *not* do

- No business rules  
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
- `"page" must be a positive integer"`

API validation ensures the request is **syntactically correct**, nothing more.

---

# 4. Application Layer — Semantic Validation  
**“Does this make sense for the business?”**

The Application layer validates **business meaning** and **cross‑field rules**.

This is where use‑case‑specific validation lives.

### Responsibilities

- Rules requiring repository access  
- Rules requiring current user context  
- Cross‑field business rules  
- Existence checks  
- Uniqueness checks  
- Authorization checks  
- Workflow rules  

### What the Application layer does *not* do

- No syntactic validation  
- No domain invariant enforcement  
- No HTTP or API concerns  
- No persistence logic outside use cases  

### Error Type

- `ValidationError` → **400 Bad Request**  
- `DomainError` → **400 Bad Request** (if domain throws)

### Examples

- `"email" must be unique"`  
- `"owner must exist before creating a pet"`  
- `"cannot delete a customer with active bookings"`

Application validation ensures the request is **semantically valid**.

---

# 5. Domain Layer — Invariant Validation  
**“What must always be true?”**

The Domain layer enforces **rules that must always hold**, regardless of context.

These rules live inside:

- Value objects  
- Entities  
- Aggregates  

### Responsibilities

- Value object invariants  
- Entity invariants  
- Aggregate invariants  
- Canonicalization  
- Internal consistency  
- Rules that must hold *forever*  

### What the Domain layer does *not* do

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

Domain validation ensures **business invariants** are upheld.

---

# 6. Error Mapping Summary

| Layer | Error Type | HTTP Status | Notes |
|-------|------------|-------------|-------|
| API | `ValidationError` | 400 | Syntactic issues |
| Application | `ValidationError` | 400 | Semantic issues |
| Application | `DomainError` | 400 | Domain invariant violation |
| Domain | `DomainError` | 400 | Thrown by entities/VOs |
| External Provider | `ExternalAuthProviderFailure` | 502 | Auth0, email provider, etc. |
| Configuration | `BadConfiguration` | 500 | Missing config keys |
| Unexpected | `Unexpected` | 500 | Catch‑all |

This table aligns with Frank’s error primitives and the global exception middleware.

---

# 7. Testing Strategy

Validation must be tested at the correct layer.

---

## 7.1 API Tests — Syntactic

Validate:

- Missing fields  
- Invalid formats  
- Bad query parameters  
- Bad route parameters  

These tests ensure the API rejects malformed requests.

---

## 7.2 Application Tests — Semantic

Validate:

- Uniqueness  
- Existence  
- Authorization  
- Workflow rules  

These tests ensure business rules are enforced.

---

## 7.3 Domain Tests — Invariants

Validate:

- Value object construction  
- Entity creation  
- Aggregate rules  

These tests ensure domain invariants cannot be violated.

---

# 8. Examples

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

# 9. Summary

- **API = shape + format**  
- **Application = business meaning**  
- **Domain = invariants**  

Each layer has a clear, non‑overlapping responsibility.  
Following these boundaries keeps the system pure, testable, and maintainable.

---

# Related Documents

- API Endpoint Purity Guide  
- Dispatcher Pipeline Guide  
- Authentication Overview  
- Architecture Overview  
