# ADR-0036 — Name ValueObject Introduction

## Status  
Accepted

## Context  
Currently, `FirstName` and `LastName` are raw strings with no domain invariants. This allows invalid names (digits, symbols, empty strings, excessive length) to enter the domain and database.

ADR‑0032 and ADR‑0033 identify names as a target for domain hardening.

## Decision  
Introduce a `Name` ValueObject and use it for both `FirstName` and `LastName`.

### New Domain Invariants  
- Must not be empty after trimming  
- Length 1–100  
- Allowed characters:  
  - Letters  
  - Spaces  
  - Hyphens  
  - Apostrophes  
- Disallowed characters:  
  - Digits  
  - Symbols  
  - Emoji  
  - Control characters  

### Normalization  
- Trim whitespace  
- Normalize Unicode to NFC  
- Preserve capitalization (no forced casing)  

### Error Semantics  
Invalid names throw `InvalidNameException`.

### Backend Alignment  
FluentValidation must enforce the same rules.  
Frontend must enforce the same rules.

## Consequences  
### Positive  
- Prevents invalid names from entering the domain  
- Ensures consistent formatting  
- Aligns frontend, backend, and domain  
- Improves data quality  

### Negative  
- Requires updating DTOs, handlers, and tests  
- Requires frontend and backend updates  
- May require data migration  

## Notes  
This ADR implements Phase 1 of ADR‑0033 for Name invariants.
