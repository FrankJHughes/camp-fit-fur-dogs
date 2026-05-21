# ADR-0032 — CreateAccount Contract Specification

## Status  
Accepted

## Context  
During implementation of the **Create Customer Account** vertical slice (US‑027, US‑126), integration tests revealed significant **contract drift** between the frontend, backend, and domain layers:

- The **frontend** and **backend** enforce different required fields  
- The **backend** and **domain** enforce different validation rules  
- ValueObjects are currently **skeletons** with minimal invariants  
- “Bad” values (invalid email, invalid phone, weak password) can pass through the backend and domain and be persisted  
- Integration tests surfaced mismatches in expectations around validation, domain errors, and persistence behavior  

This demonstrated the need for a **single canonical contract** governing the CreateAccount flow across all layers.

Without a shared contract:

- The frontend may send data the backend rejects  
- The backend may accept data the domain should reject  
- The domain may accept data the database should not store  
- Tests become brittle and inconsistent  
- Future slices (email verification, welcome email, login) inherit the same inconsistencies  

A unified contract is required to ensure deterministic, predictable behavior across the entire system.

## Decision  
We establish a **canonical CreateAccount Contract Specification** that defines:

- Required fields  
- Field formats  
- Normalization rules  
- Domain invariants  
- Backend validation rules  
- Error semantics  
- Persistence expectations  

This contract becomes the **single source of truth** for:

- Frontend implementation  
- Backend request validation  
- Domain ValueObject invariants  
- Integration tests  
- Future slices that depend on customer identity  

### 1. Request DTO (Canonical)

```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "phone": "string",
  "password": "string"
}
```

All fields are **required**.

### 2. Field‑Level Rules

#### FirstName  
- Required  
- Trimmed  
- Length 1–100  
- Allowed: letters, spaces, hyphens, apostrophes  
- Domain invariant: not empty after trimming  

#### LastName  
- Same rules as FirstName  

#### Email  
- Required  
- Trimmed  
- Lowercased  
- Must be syntactically valid (frontend + backend)  
- Domain invariant (current): contains `'@'`  
- Future: RFC‑lite validation  
- Must be unique  

#### Phone  
- Required  
- Trimmed  
- Must be a valid phone number (frontend + backend)  
- Domain invariant (current): not empty  
- Future: E.164 or US‑specific normalization  

#### Password  
- Required  
- Minimum length 8  
- Must contain at least one letter and one number  
- Domain hashes using **BCrypt**  
- Domain does not enforce strength (future consideration)  

### 3. Domain Invariants

#### Current  
- Email: not empty, contains `'@'`, lowercased  
- Phone: not empty  
- PasswordHash: must be hashed  
- CustomerId: wraps a Guid  

#### Future (Planned)  
- Email: RFC‑lite validation, no double dots, valid domain  
- Phone: E.164 normalization  
- Names: disallow digits, enforce length  
- Password: optional domain‑level strength enforcement  

### 4. Backend Validation (FluentValidation)

Backend must enforce **exactly the same rules** as the frontend:

- Required fields  
- Email format  
- Phone format  
- Password minimum strength  
- Error semantics:  
  - Validation errors → `400 BadRequest`  
  - Domain errors → `400 BadRequest`  
  - Duplicate email → `409 Conflict`  

### 5. Response Semantics

#### 201 Created  
`Location: /api/customers/{id}`

#### 400 Validation Error  
```json
{
  "title": "Validation Error",
  "errors": { "email": ["Email is invalid"] }
}
```

#### 400 Domain Error  
```json
{
  "title": "Domain Error",
  "detail": "Phone number cannot be empty"
}
```

#### 409 Conflict  
```json
{
  "title": "Conflict",
  "detail": "Email already exists"
}
```

### 6. Frontend Requirements  
The frontend must:

- Enforce the same required fields  
- Use the same validation rules  
- Normalize email to lowercase  
- Trim all fields  
- Display inline validation errors  
- Handle 400 + 409 responses  

### 7. Integration Test Requirements  
Integration tests must verify:

- Success path (201)  
- Validation errors (400)  
- Domain errors (only for actual domain invariants)  
- Duplicate email (409)  
- Password hashing (BCrypt)  
- Persistence using `CustomerId` ValueObject  

Tests must **not** assert domain errors for invariants that do not exist.

## Consequences  

### Positive  
- Eliminates frontend/backend/domain drift  
- Ensures consistent validation across all layers  
- Prevents invalid data from reaching the database  
- Makes integration tests stable and meaningful  
- Provides a foundation for future slices (email verification, welcome email, login)  
- Creates a durable architectural artifact for future developers  

### Negative  
- Requires updates to frontend validation  
- Requires updates to backend FluentValidation  
- Requires updates to ValueObjects  
- Requires updating existing tests to match the contract  
- Future domain hardening will require additional refactoring  

## Notes  
This ADR establishes the **contract**.  
A future ADR will define **domain invariant hardening** (email normalization, phone normalization, name rules, etc.).
