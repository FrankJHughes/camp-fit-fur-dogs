# Identity API — Endpoint Response Contracts

The **Endpoints** folder contains the response DTOs used by the Identity API
surface.  
These types are intentionally minimal, stable, and free of domain logic.  
They define the public contract for identity‑related endpoints such as login
initiation, logout, and identity resolution.

All DTOs in this folder follow the Identity purity rules established in the
authentication stories (US‑110, US‑111, US‑133):

- No tokens  
- No provider‑specific metadata  
- No session artifacts  
- No domain logic  
- Only safe, client‑consumable values  

These contracts define the public shape of the Identity API.

---

## Files

```
Endpoints/
├── GetIdentityEndpointResponse.cs
├── GetLoginUrlEndpointResponse.cs
└── LogoutEndpointResponse.cs
```

---

## GetIdentityEndpointResponse

Represents the resolved identity information for the authenticated user.

### Purpose

- Returned by the `GET /identity` endpoint  
- Provides the user’s display name  
- Contains no tokens, claims, or provider details  

### Contract

```csharp
public sealed class GetIdentityEndpointResponse
{
    public string Name { get; init; }
}
```

### Notes

- Must remain minimal  
- Safe for client consumption  
- Mirrors the identity purity rules in US‑110 and US‑111  

---

## GetLoginUrlEndpointResponse

Represents the next URL the client must navigate to in order to begin the OIDC
login flow.

### Purpose

- Returned by the `GET /identity/login-url` endpoint  
- Initiates external login  
- Contains no provider tokens or session state  

### Contract

```csharp
public sealed record GetLoginUrlEndpointResponse(string NextUrl);
```

### Notes

- Pure redirect instruction  
- No embedded logic  
- Must remain stable  

---

## LogoutEndpointResponse

Represents the next URL the client must navigate to in order to complete the OIDC
logout flow.

### Purpose

- Returned by the `POST /identity/logout` endpoint  
- Completes external logout  
- Contains no tokens or session identifiers  

### Contract

```csharp
public sealed record LogoutEndpointResponse(string NextUrl);
```

### Notes

- Mirrors login flow purity  
- No provider metadata  
- No session artifacts  

---

## Design Principles

- **Purity**  
  Endpoint responses contain no logic, only data.

- **Safety**  
  No sensitive identity information is ever returned.

- **Stability**  
  DTOs change rarely and intentionally.

- **Predictability**  
  All identity endpoints return simple, consistent shapes.

---

## How These DTOs Fit Into the Identity Architecture

These response types are consumed by:

- Identity API endpoints  
- Frontend clients  
- OIDC login/logout flows  
- Session management middleware  

They form the public contract between the Identity subsystem and external
clients, ensuring a stable and predictable authentication experience.

---
