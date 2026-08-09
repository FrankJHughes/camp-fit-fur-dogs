# Identity EntityFrameworkCore — Sessions

The **Sessions** folder contains the complete vertical slice for session
management in the Identity subsystem.  
It includes EF Core persistence components, configuration binding, and DI
registration for creating, reading, and revoking authentication sessions.

This slice is responsible for securely issuing, storing, evaluating, and
revoking session tokens.

---

## Purpose

The Sessions subsystem provides:

- A persistence model for `Session` aggregates
- Writers for creating and revoking sessions
- A reader for retrieving and evaluating session state
- Configuration binding for session TTL and related settings
- DI registration for all session components

This folder forms the infrastructure layer for the session lifecycle.

---

## Folder Contents

### **CreateSessionWriter**
Persists newly created `Session` aggregates.

Responsibilities:

- Adds a new `Session` to the EF Core change tracker
- Does **not** call `SaveChangesAsync` (unit of work handles commit)
- Implements `ICreateSessionWriter` from the Application layer

Used during:

- Session creation
- Login flows
- Token issuance

---

### **GetSessionReader**
Retrieves and evaluates session state.

Responsibilities:

- Loads sessions using `AsNoTracking` for read‑only queries
- Matches sessions by `SessionTokenHash`
- Computes expiration using:
  - `CreatedAt`
  - `SessionSettings.Ttl`
- Uses `IClock` for deterministic evaluation time
- Implements `IGetSessionReader`

Used during:

- Token validation
- Session introspection
- Authentication middleware

---

### **RevokeSessionWriter**
Revokes an existing session.

Responsibilities:

- Loads the session by token hash
- Invokes domain behavior: `session.Revoke(DateTimeOffset.UtcNow)`
- Relies on EF Core tracking for persistence
- Implements `IRevokeSessionWriter`

Used during:

- Logout flows
- Security events
- Token invalidation

---

### **ServiceCollectionExtensions**
Registers all session services and binds configuration.

Responsibilities:

- Binds `SessionSettings` from `Identity:Session`
- Validates settings using data annotations
- Registers:
  - `ICreateSessionWriter`
  - `IGetSessionReader`
  - `IRevokeSessionWriter`

Used during:

- Application startup
- Host configuration

---

### **SessionSettings**
Configuration object defining session TTL and related parameters.

Responsibilities:

- Defines the lifetime of a session
- Supports validation via data annotations
- Loaded via `IOptionsMonitor<SessionSettings>`

Used during:

- Session expiration evaluation
- Security policy enforcement

---

## Design Principles

The Sessions subsystem follows these architectural principles:

- **Vertical slice isolation**  
  Each operation (create, read, revoke) has its own interface and implementation.

- **Domain purity**  
  EF Core handles persistence; domain handles behavior.

- **Configuration‑driven TTL**  
  Session expiration is controlled by `SessionSettings`.

- **Testability**  
  `IClock` ensures deterministic time evaluation.

- **EF Core best practices**  
  - `AsNoTracking` for reads  
  - Value object conversions  
  - Change tracking for revocation  

---

## Session Lifecycle Overview

1. **Create**  
   - Domain constructs a `Session`  
   - Writer attaches it to the DbContext  
   - Unit of work commits  

2. **Read**  
   - Reader loads session by token hash  
   - TTL applied  
   - Expiration evaluated  

3. **Revoke**  
   - Writer loads session  
   - Domain marks it revoked  
   - Unit of work commits  

---

## Summary

The **Sessions** folder provides the complete infrastructure for managing
authentication sessions:

- Creation  
- Retrieval  
- Expiration evaluation  
- Revocation  
- Configuration  
- Dependency injection  

It is a fully isolated vertical slice aligned with the Identity subsystem’s
architecture and domain‑driven design principles.

