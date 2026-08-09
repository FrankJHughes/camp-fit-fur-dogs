# Identity Application — GetSession

The **GetSession** folder contains the application‑layer components responsible
for retrieving an authenticated session using its token hash.  
This functionality is used during authentication, cookie validation, and
session introspection.

The folder follows the CQRS query pattern:

- A **query** (`GetSessionQuery`)
- A **response** (`GetSessionResponse`)
- A **reader abstraction** (`IGetSessionReader`)
- A **handler** (`GetSessionByIdHandler`)

This separation ensures clarity, testability, and a clean boundary between
application logic and persistence.

---

## Folder Structure

```
GetSession/
├── GetSessionByIdHandler.cs
```

(Other abstractions such as `GetSessionQuery`, `GetSessionResponse`, and
`IGetSessionReader` live in the Abstractions layer.)

---

# GetSessionByIdHandler

The handler responsible for retrieving a session by its token hash.

### Responsibilities

- Accept a `GetSessionQuery`
- Use `IGetSessionReader` to load the session
- Return a `GetSessionResponse` if found
- Throw `SessionNotFoundException` if no session exists

### Notes

- The handler returns `null` only if the reader returns `null` *and* the handler
  is explicitly allowed to do so — otherwise it throws.
- This handler is typically used during:
  - Cookie validation
  - Session introspection
  - Authentication middleware

---

# Query Flow

```
[ TokenHash ]
     ↓
GetSessionQuery
     ↓
GetSessionByIdHandler
     ↓
IGetSessionReader.ReadAsync()
     ↓
[ Session or SessionNotFoundException ]
```

---

# Abstractions (in Identity.Application.Abstractions)

### **GetSessionQuery**
Represents a request to retrieve a session by token hash.

### **GetSessionResponse**
Represents the session data returned to the caller.

### **IGetSessionReader**
Abstraction for reading sessions from the persistence layer.

### **SessionNotFoundException**
Thrown when no session exists for the provided token hash.

---

# Summary

The GetSession folder defines the application‑layer handler for retrieving
sessions:

### Core Handler
- `GetSessionByIdHandler`

### Abstractions (external to this folder)
- `GetSessionQuery`
- `GetSessionResponse`
- `IGetSessionReader`
- `SessionNotFoundException`

Together, these components form a clean, testable, and predictable session
retrieval flow within the Identity subsystem.

---
