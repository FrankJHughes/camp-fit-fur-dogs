# Identity API — Authorization

The **Authorization** folder contains the configuration responsible for enforcing
authorization rules across the Identity API surface.  
This subsystem establishes a **fallback authorization policy** that requires all
requests to be authenticated unless explicitly overridden, ensuring a secure and
predictable baseline for all identity‑related endpoints.

Authorization in the Identity subsystem follows the purity and safety rules
defined in stories such as US‑110, US‑111, US‑133, and US‑148:

- Authorization is minimal and free of domain logic  
- All endpoints require authentication by default  
- Additional policies can be layered without modifying endpoint code  
- No identity provider tokens or sensitive metadata are exposed  

This folder defines the public authorization configuration used by the Identity
API.

---

## Files

```
Authorization/
└── AuthorizationServiceCollectionExtensions.cs
```

---

## AuthorizationServiceCollectionExtensions

This class configures all authorization services used by the Identity API.

### Responsibilities

- **Fallback Authorization Policy**  
  Adds a fallback policy named `"RequireAuthenticatedUser"` that enforces
  authentication for all endpoints unless explicitly marked as anonymous.

- **Policy Builder for Future Features**  
  Provides a central place to add future identity‑specific authorization policies
  (e.g., owner‑only access, verified‑email access) as additional identity stories
  are implemented.

### Contract

```csharp
public static IServiceCollection AddFrankIdentityApiAuthorization(
    this IServiceCollection services)
```

### Notes

- The fallback policy ensures secure defaults across the entire Identity API.  
- Additional policies can be added without modifying endpoint code.  
- Authorization remains intentionally minimal and provider‑agnostic.  
- No claims, tokens, or sensitive identity details are exposed.

---

## Design Principles

- **Purity**  
  Authorization configuration contains no domain logic.

- **Safety**  
  All endpoints require authentication unless explicitly overridden.

- **Minimalism**  
  Only essential authorization rules are defined.

- **Extensibility**  
  Future policies can be added cleanly as identity features evolve.

- **Separation of Concerns**  
  Authorization configuration is isolated from authentication, middleware, and
  endpoint logic.

---

## How Authorization Fits Into the Identity Architecture

Authorization sits between:

```
[ Authentication ]
      ↓
[ Authorization ]
      ↓
[ Identity API Endpoints ]
```

It ensures:

- Every identity endpoint is protected by default  
- Authorization rules remain simple and predictable  
- Future identity features (e.g., verified email, owner‑only access) can be added
  without architectural disruption  

---
