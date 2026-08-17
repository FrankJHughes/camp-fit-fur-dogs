# Identity Domain — Sessions Subsystem

The **Sessions** folder contains all domain‑level types responsible for
representing, validating, and enforcing invariants around authenticated
sessions in the Identity subsystem.  
This includes the session aggregate, its identifiers, token hashing, cookies,
and all domain errors related to invalid or missing session data.

The subsystem is intentionally small, highly cohesive, and strictly invariant‑driven.

---

## Folder Structure

```
Sessions/
├── Session.cs
├── SessionId.cs
├── SessionTokenHash.cs
├── SessionCookie.cs
└── Errors/
    ├── InvalidSessionIdException.cs
    ├── InvalidSessionTokenHashException.cs
    └── SessionNotFoundException.cs
```

---

# Session (Aggregate Root)

Represents an authenticated session belonging to a user.

### Responsibilities

- Store the hashed session token (`SessionTokenHash`)
- Store the owner (`UserId`)
- Track creation and revocation timestamps
- Enforce invariants:
  - `CreatedAt` must be a valid timestamp
  - A session can only be revoked once
- Provide domain behaviors:
  - `IsExpired(now, ttl)`
  - `IsRevoked()`
  - `IsActive(now, ttl)`
  - `Revoke(now)`

### Notes

- Only the **hash** of the token is stored server‑side  
- The plaintext token is returned to the client via `SessionCookie`

---

# SessionId (Value Object / AggregateId)

Strongly‑typed identifier for a session.

### Responsibilities

- Wrap a non‑empty GUID
- Prevent `Guid.Empty` from entering the domain
- Provide safe creation and parsing:
  - `New()`
  - `From(Guid)`
  - `TryFrom(Guid, out SessionId?)`
  - `TryParse(string?, out SessionId?)`

### Domain Rules

- Session IDs must be valid GUIDs  
- Session IDs must never be empty  

---

# SessionTokenHash (Value Object)

Represents the SHA‑256 hash of a session token.

### Responsibilities

- Enforce strict SHA‑256 hex formatting:
  - Exactly 64 hex characters
  - Uppercase or lowercase allowed
- Prevent empty or whitespace values
- Provide safe creation and parsing:
  - `From(string)`
  - `TryFrom(string?, out SessionTokenHash?)`
  - `TryParse(string?, out SessionTokenHash?)`

### Domain Rules

- Only **hashed** tokens are stored  
- Hash must be a valid SHA‑256 hex string  

---

# SessionCookie (Value Object)

Represents the cookie sent to the client containing the **plaintext** session token.

### Responsibilities

- Encapsulate cookie name (`cfd.session`)
- Store plaintext token value
- Provide standard cookie formatting via `ToString()`

### Notes

- Server stores only the hash  
- Client stores the plaintext token in this cookie  

---

# Errors (Domain Exceptions)

The Errors folder contains all domain‑level exceptions related to session identity,
token hashing, and lookup failures.

### Included Exceptions

#### **InvalidSessionIdException**
Thrown when:
- `Guid.Empty` is used  
- A string fails GUID parsing  

#### **InvalidSessionTokenHashException**
Thrown when:
- Token hash is empty  
- Token hash is not a valid SHA‑256 hex string  

#### **SessionNotFoundException**
Thrown when:
- A session lookup fails  
- A session does not exist for the given ID or token hash  

---

# Summary

The Sessions subsystem provides the core domain model for authenticated sessions:

### Core Types
- `Session`
- `SessionId`
- `SessionTokenHash`
- `SessionCookie`

### Error Types
- `InvalidSessionIdException`
- `InvalidSessionTokenHashException`
- `SessionNotFoundException`

### Responsibilities
- Enforce strict invariants around session identity and token hashing  
- Provide safe creation and parsing APIs  
- Support expiration, revocation, and active‑session checks  
- Ensure secure handling of plaintext vs hashed tokens  

This folder forms the foundation of a robust, predictable session‑management model
within the Identity domain.

---
