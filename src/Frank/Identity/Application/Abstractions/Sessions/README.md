# Identity Application — Sessions Subsystem

The **Sessions** subsystem defines all application‑layer abstractions involved in
creating, retrieving, evaluating, and revoking authenticated sessions.

Sessions represent secure, durable authentication relationships between an owner
and the application.  
This subsystem provides a clean separation between:

- **Domain logic** (session model, token hashing)
- **Application logic** (commands, queries, readers, writers)
- **Infrastructure logic** (database, cache, distributed session store)

All files in this folder are **pure application‑layer contracts** or **immutable
response models**.  
No persistence, hashing, or storage concerns appear here.

---

## Folder Structure

```
Sessions/
├── GeneratedSessionToken.cs
├── ISessionTokenGenerator.cs
│
├── CreateSession/
│   └── ICreateSessionWriter.cs
│
├── GetSession/
│   ├── GetSessionQuery.cs
│   ├── GetSessionResponse.cs
│   └── IGetSessionReader.cs
│
└── RevokeSession/
    ├── RevokeSessionCommand.cs
    └── IRevokeSessionWriter.cs
```

---

# Token Generation

## GeneratedSessionToken

Represents the pair of values produced when generating a new session token:

- `PlaintextToken` — returned to the client, never persisted  
- `HashedToken` — stored server‑side for lookup, validation, and revocation  

Immutable and safe for transport.

## ISessionTokenGenerator

Defines the contract for:

- Generating cryptographically secure plaintext tokens  
- Hashing tokens into non‑reversible `SessionTokenHash` values  

This abstraction ensures deterministic hashing and zero exposure of plaintext
tokens beyond creation time.

---

# Session Creation

## ICreateSessionWriter

Application‑layer contract for persisting a newly created session.

Responsibilities:

- Write the domain `Session` to durable storage  
- Ensure atomicity and consistency  
- Support cancellation  
- Abstract away infrastructure concerns (SQL, NoSQL, Redis, etc.)

Used by the OIDC callback save pipeline after token generation.

---

# Session Retrieval

## GetSessionQuery

CQRS query used to retrieve a session by its token hash.

- Carries only the secure hash  
- Returns `GetSessionResponse?`  
- Query handler injects `IClock` and captures `EvaluatedAt`

## GetSessionResponse

Immutable representation of a resolved session.

Contains:

- `Id`, `OwnerId`, `CreatedAt`, `RevokedAt`, `ExpiresAt`
- `EvaluatedAt` — timestamp captured by the query handler

Deterministic evaluation:

- `IsExpired` — `ExpiresAt <= EvaluatedAt`
- `IsRevoked` — `RevokedAt != null`
- `IsActive` — not expired and not revoked

No ambient time (`UtcNow`) is used.

## IGetSessionReader

Contract for retrieving a session from persistent storage.

Responsibilities:

- Lookup by token hash  
- Return `GetSessionResponse?`  
- Support cancellation  

Does **not** evaluate expiration or revocation — that logic lives in
`GetSessionResponse`.

---

# Session Revocation

## RevokeSessionCommand

CQRS command used to revoke a session.

Carries:

- Secure, non‑reversible token hash

Command handler:

- Injects `IClock`  
- Captures revocation timestamp  
- Delegates persistence to `IRevokeSessionWriter`

## IRevokeSessionWriter

Contract for marking a session as revoked in durable storage.

Responsibilities:

- Locate session by hash  
- Mark as revoked  
- Persist revocation timestamp  
- Support cancellation  

Infrastructure implementations may use SQL, NoSQL, Redis, or distributed caches.

---

# Session Lifecycle Overview

```
[ Generate Token ]
       ↓
ISessionTokenGenerator
       ↓
GeneratedSessionToken
       ↓
[ Create Session ]
       ↓
ICreateSessionWriter
       ↓
[ Retrieve Session ]
       ↓
IGetSessionReader
       ↓
GetSessionResponse (EvaluatedAt)
       ↓
[ Revoke Session ]
       ↓
RevokeSessionCommand
       ↓
IRevokeSessionWriter
```

This lifecycle ensures:

- Deterministic evaluation  
- Immutable session state  
- Replayable authentication flows  
- Clean separation of concerns  
- Zero plaintext token persistence  

---

# Summary

The Sessions subsystem defines the complete application‑layer abstraction for
secure, deterministic session management:

### Token Generation
`GeneratedSessionToken`, `ISessionTokenGenerator`

### Session Creation
`ICreateSessionWriter`

### Session Retrieval
`GetSessionQuery`, `GetSessionResponse`, `IGetSessionReader`

### Session Revocation
`RevokeSessionCommand`, `IRevokeSessionWriter`

Together, these abstractions form a clean, testable, replayable, and
production‑grade session‑management architecture within Identity.

---
