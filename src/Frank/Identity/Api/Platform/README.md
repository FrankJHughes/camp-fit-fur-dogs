# Identity API — Platform Layer

The **Platform** folder contains the composition root for the API surface hosted
in this assembly.  
It defines how the Identity subsystem is **registered** (services) and **executed**
(middleware pipeline).  
Although these components live in the Identity API project, they are **not limited
to identity endpoints**.  
They reside here because they depend on Identity abstractions such as:

- Identity authentication configuration  
- Identity authorization policies  
- Identity application pipelines  
- Identity EF Core persistence  
- Identity infrastructure services  
- Identity session + user resolution middleware

The Platform layer ensures that all of these subsystems are assembled in the
correct order and exposed as a unified API runtime.

---

## Folder Structure

```
Platform/
├── ApplicationBuilderExtensions.cs
└── ServiceCollectionExtensions.cs
```

---

# ApplicationBuilderExtensions

Configures the **runtime pipeline** for the API.

### Responsibilities

- Install Observations middleware  
- Enable ASP.NET Core authentication  
- Apply forwarded headers (reverse proxy support)  
- Validate and attach session principals  
- Apply ASP.NET Core authorization  
- Enforce Identity authorization rules  

### Middleware Order

The ordering is intentional and critical:

```
1. Observations Middleware
2. Authentication (ASP.NET Core)
3. Forwarded Headers
4. Session Validation Middleware
5. Authorization (ASP.NET Core)
6. Identity Authorization Middleware
```

This ensures:

- Full request lifecycle telemetry  
- Session cookies validated before authorization  
- Authenticated principals available to authorization  
- Identity purity rules enforced consistently  

### Contract

```csharp
app.UseFrankIdentityApiPlatform();
```

---

# ServiceCollectionExtensions

Configures the **service graph** for the Identity subsystem.

### Responsibilities

Registers:

- **Authentication** — Identity session scheme configuration  
- **Authorization** — Identity authorization policies  
- **Application Layer** — Identity pipelines (commands, readers, writers)  
- **Entity Framework Core** — Identity persistence  
- **Infrastructure** — token hashing, correlation, error boundaries, etc.  
- **Core API Middleware** — shared middleware used across the API surface  

### Contract

```csharp
services.AddFrankIdentityApiPlatform(configuration);
```

### Why It Lives Here

This is the **service‑composition root** for the Identity API assembly.  
It wires together all Identity subsystems and exposes them as a cohesive platform.

---

# How the Platform Layer Fits Into the Architecture

```
[ ServiceCollectionExtensions ]
        ↓ registers
[ Identity Authentication ]
[ Identity Authorization ]
[ Identity Application ]
[ Identity EF Core ]
[ Identity Infrastructure ]
[ Core API Middleware ]
        ↓
[ ApplicationBuilderExtensions ]
        ↓ configures
[ Observations ]
[ Authentication ]
[ Forwarded Headers ]
[ Session Validation ]
[ Authorization ]
[ Identity Authorization ]
        ↓
[ Endpoints ]
        ↓
[ Application Pipelines ]
        ↓
[ Domain ]
```

This ensures:

- Identity subsystems are registered correctly  
- Middleware layers execute in the correct order  
- The API surface behaves consistently and predictably  
- Cross‑cutting concerns (sessions, authorization, observability) are unified  

---

# Summary

The Platform folder provides:

### **ServiceCollectionExtensions**
The DI composition root for the Identity subsystem.

### **ApplicationBuilderExtensions**
The middleware composition root for the API runtime.

Together, these components define the **Identity API Platform** — the unified,
structured, predictable runtime environment for all endpoints hosted in this
assembly.

---
