# Identity API — Endpoints

The **Endpoints** folder contains all HTTP endpoint implementations that make up
the public Identity API surface.  
Each endpoint is intentionally minimal, free of domain logic, and delegates all
identity operations to the appropriate application‑layer pipelines.

Endpoints follow the Identity purity rules defined in stories such as  
US‑110 (Owner Login), US‑111 (Session Management), US‑133 (Account Lockout),  
and US‑148 (Email Verification):

- No identity provider tokens are ever returned  
- No domain logic is embedded in endpoints  
- All sensitive operations occur inside application pipelines  
- Endpoints return only safe, client‑consumable DTOs  
- Redirect behavior is explicit and predictable  

---

## Folder Structure

```
Endpoints/
├── CallbackEndpoint.cs
├── GetIdentityEndpoint.cs
├── GetLoginUrlEndpoint.cs
├── LogoutEndpoint.cs
└── ServiceCollectionExtensions.cs
```

---

## CallbackEndpoint

Handles the OIDC callback from the external identity provider.

### Responsibilities

- Validates `state` and `code` query parameters  
- Runs the OIDC pipeline to exchange the authorization code  
- Runs the application pipeline to create a session cookie  
- Issues the real CFFD session cookie  
- Redirects the user to the `return_url` encoded in state  

### Returns

Redirect only — no tokens, no claims, no provider metadata.

---

## GetIdentityEndpoint

Returns the authenticated user’s resolved identity information.

### Responsibilities

- Requires authorization  
- Uses `ICurrentUser` to resolve identity  
- Returns a minimal DTO containing only the user’s display name  

### Returns

```csharp
GetIdentityEndpointResponse { Name }
```

---

## GetLoginUrlEndpoint

Generates the next URL required to begin the OIDC login flow.

### Responsibilities

- Validates OIDC configuration (authority, client ID, callback)  
- Validates frontend configuration  
- Determines `return_url` (query parameter or frontend base URL)  
- Encodes OIDC `state`  
- Constructs the full authorization URL  

### Returns

```csharp
GetLoginUrlEndpointResponse { NextUrl }
```

---

## LogoutEndpoint

Logs out the user by revoking their session and deleting the session cookie.

### Responsibilities

- Reads plaintext session token from cookie  
- Hashes token and dispatches `RevokeSessionCommand`  
- Deletes the session cookie  
- Determines post‑logout redirect URL  
- Returns a DTO containing the redirect URL  

### Returns

```csharp
LogoutEndpointResponse { NextUrl }
```

---

## ServiceCollectionExtensions

Registers all Identity API endpoints and validates required configuration.

### Responsibilities

- Binds and validates `FrontendSettings`  
- Restricts endpoint discovery to the Identity API namespace  
- Registers all endpoint implementations via Frank Core’s endpoint loader  

---

## Design Principles

- **Purity**  
  Endpoints contain no domain logic and expose no sensitive identity data.

- **Safety**  
  All redirect URLs, callback URLs, and state values are validated.

- **Minimalism**  
  Endpoints return only the data required by the client.

- **Delegation**  
  All identity operations (OIDC, session creation, revocation) occur in
  application pipelines.

- **Predictability**  
  Redirect behavior is explicit and consistent across login, callback, and logout.

---

## How Endpoints Fit Into the Identity Architecture

```
[ Client ]
   ↓
[ Endpoints ]
   ↓
[ Authentication / Authorization ]
   ↓
[ Application Pipelines ]
   ↓
[ Identity Domain ]
```

Endpoints act as the thin HTTP boundary of the Identity subsystem, delegating all
real work to the application layer while enforcing purity, safety, and
predictability.

---
