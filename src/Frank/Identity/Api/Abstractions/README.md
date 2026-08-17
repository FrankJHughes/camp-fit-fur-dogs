# Identity API — Abstractions

The **Abstractions** folder defines the public contracts used by the Identity API
surface.  
These contracts are intentionally minimal, stable, and free of domain logic.  
They describe *what* the Identity API returns, not *how* identity is resolved,
authenticated, or managed internally.

Abstractions form the boundary between the Identity subsystem and external
clients.  
They ensure that identity‑related endpoints expose a predictable, safe, and
provider‑agnostic API surface.

---

## Folder Structure

```
Abstractions/
└── Endpoints/
    ├── GetIdentityEndpointResponse.cs
    ├── GetLoginUrlEndpointResponse.cs
    └── LogoutEndpointResponse.cs
```

The **Endpoints** folder contains all response DTOs returned by Identity API
endpoints.

---

## Purpose of the Abstractions Folder

The Abstractions folder provides:

- Stable response contracts for identity endpoints  
- Provider‑agnostic DTOs that do not leak OIDC or infrastructure details  
- Purity‑aligned shapes following the rules in US‑110, US‑111, and US‑133  
- Minimal, safe representations of identity‑related data  
- Clear separation between API surface and identity implementation  

These abstractions ensure that the Identity API remains predictable and easy to
consume across frontend clients, services, and automated systems.

---

## Endpoint Response Contracts

### GetIdentityEndpointResponse

Represents the authenticated user’s resolved identity information.

```csharp
public sealed class GetIdentityEndpointResponse
{
    public string Name { get; init; }
}
```

### GetLoginUrlEndpointResponse

Represents the next URL the client must navigate to in order to begin the OIDC
login flow.

```csharp
public sealed record GetLoginUrlEndpointResponse(string NextUrl);
```

### LogoutEndpointResponse

Represents the next URL the client must navigate to in order to complete the OIDC
logout flow.

```csharp
public sealed record LogoutEndpointResponse(string NextUrl);
```

---

## Design Principles

- **Purity** — Endpoint responses contain no logic, only data.  
- **Minimalism** — DTOs contain only the fields required by the client.  
- **Stability** — Response shapes change rarely and intentionally.  
- **Safety** — No sensitive identity information is ever returned.  
- **Separation of concerns** — Abstractions define the API surface; implementation lives elsewhere.

---

## How Abstractions Fit Into the Identity Architecture

These DTOs are consumed by:

- Identity API endpoints  
- Frontend clients  
- OIDC login/logout flows  
- Session management middleware  

They form the public contract between the Identity subsystem and external
clients, ensuring a stable and predictable authentication experience.

---
