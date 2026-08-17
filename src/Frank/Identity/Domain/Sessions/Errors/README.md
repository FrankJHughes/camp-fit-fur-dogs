# Identity Domain — Session Errors

The **Errors** folder contains all domain‑level exceptions related to session
identity, token hashing, and lookup failures.  
These exceptions enforce invariants inside the **Sessions** domain model and
provide intention‑revealing failure modes for upstream layers such as
application handlers, middleware, and validation pipelines.

Each exception is a `DomainException`, ensuring consistent error semantics
across the Identity domain.

---

## Folder Structure

```
Errors/
├── InvalidSessionIdException.cs
├── InvalidSessionTokenHashException.cs
└── SessionNotFoundException.cs
```

---

# InvalidSessionIdException

Thrown when a `SessionId` violates domain invariants.

### When It Occurs

- A `SessionId` is constructed from `Guid.Empty`
- A raw string fails to parse into a valid GUID

### Domain Rules Enforced

- Session IDs must be **non‑empty GUIDs**
- Session IDs must be **valid GUID strings** when parsed from text

### Factory Methods

- **Empty()** — thrown when `Guid.Empty` is used  
- **InvalidFormat(raw)** — thrown when parsing fails

---

# InvalidSessionTokenHashException

Thrown when a `SessionTokenHash` violates domain invariants.

### When It Occurs

- Token hash is empty or whitespace
- Token hash is not a valid SHA‑256 hex string

### Domain Rules Enforced

- Token hashes must be **non‑empty**
- Token hashes must be **64‑character lowercase SHA‑256 hex strings**

### Factory Methods

- **Empty()** — thrown for empty or whitespace values  
- **InvalidFormat(value)** — thrown for invalid SHA‑256 hex strings

---

# SessionNotFoundException

Thrown when a session lookup fails.

### When It Occurs

- A session cannot be found for a given `SessionId`
- A session cannot be found for a given `SessionTokenHash`
- A query handler or middleware expects a session to exist but none is present

### Domain Rules Enforced

- Session retrieval must be explicit and validated  
- Missing sessions are treated as domain errors, not null‑reference failures

---

# Summary

The Errors folder defines the domain‑level exceptions that protect the integrity
of the Sessions subsystem:

### Core Exceptions
- `InvalidSessionIdException`
- `InvalidSessionTokenHashException`
- `SessionNotFoundException`

### Responsibilities
- Enforce domain invariants  
- Provide intention‑revealing error semantics  
- Prevent invalid session identifiers or token hashes  
- Ensure session lookup failures are explicit and safe  

These exceptions form the foundation of a robust, predictable session‑management
domain model.

---
