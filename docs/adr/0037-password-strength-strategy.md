# ADR-0037 — Password Strength Strategy

## Status  
Accepted

## Context  
The domain currently hashes passwords using BCrypt but does not enforce password strength. Password strength is enforced only by frontend and backend validation.

ADR‑0032 and ADR‑0033 identify password strength as an optional but valuable domain invariant.

## Decision  
We will adopt a **two‑layer password strength strategy**:

### Layer 1 — Backend Validation (Required)  
Backend must enforce:

- Minimum length: 8  
- At least one letter  
- At least one number  
- No whitespace  

### Layer 2 — Domain Validation (Optional, Future)  
Domain may enforce:

- Minimum entropy threshold  
- Reject passwords found in common breach lists  
- Reject passwords equal to email or name  
- Reject passwords with repeated characters  

This will be implemented in a future phase.

### Hashing Strategy  
- Continue using BCrypt (`$2a$`, `$2b$`, `$2y$`)  
- Work factor remains 11  
- Domain rejects any non‑BCrypt hash passed to `PasswordHash` VO  

### Error Semantics  
Weak passwords (backend) → `400 Validation Error`  
Invalid hash (domain) → `Domain Error`

## Consequences  
### Positive  
- Stronger security posture  
- Domain becomes self‑protecting  
- Backend and domain remain aligned  
- Supports future password policies  

### Negative  
- Domain‑level strength checks may require external services  
- Increased complexity  
- Requires updates to tests and frontend  

## Notes  
This ADR implements Phase 2 of ADR‑0033 for password strength.
