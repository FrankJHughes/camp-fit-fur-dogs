# Identity API — Assembly Root

The **API** folder is the top‑level assembly for the Identity API surface.  
It contains the composition roots, middleware orchestration, settings, and the
assembly marker used for scanning, configuration binding, and DI boundaries.

Although this assembly hosts identity‑related endpoints, it also provides
cross‑cutting infrastructure used by any API surface that depends on Identity’s
authentication, authorization, session model, and observability pipeline.

This folder defines how the Identity subsystem is **registered**, **configured**, and
**executed**.

---

## Folder Structure

```
Api/
├── Platform/
│   ├── ApplicationBuilderExtensions.cs
│   └── ServiceCollectionExtensions.cs
│
├── Settings/
│   └── FrontendSettings.cs
│
└── AssemblyMarker.cs
```

---

# Platform

The **Platform** folder contains the composition roots for the Identity API:

### **ServiceCollectionExtensions**
Registers all Identity subsystems:

- Authentication (session scheme)
- Authorization (Identity policies)
- Application layer (commands, readers, writers)
- EF Core persistence
- Infrastructure (token hashing, correlation, error boundaries)
- Core API middleware

This is the **DI composition root** for the Identity API assembly.

### **ApplicationBuilderExtensions**
Assembles the runtime middleware pipeline:

1. Observations middleware  
2. ASP.NET Core authentication  
3. Forwarded headers  
4. Session validation  
5. ASP.NET Core authorization  
6. Identity authorization middleware  

This is the **middleware composition root** for the API runtime.

---

# Settings

The **Settings** folder contains strongly‑typed configuration objects used by the
Identity API.

### **FrontendSettings**
Defines the base URL of the frontend application.  
Used for redirect flows such as login, logout, and callback navigation.

This ensures environment‑specific frontend URLs are not hard‑coded.

---

# AssemblyMarker

### **AssemblyMarker.cs**
A zero‑logic marker type used to identify the `Frank.Identity.Api` assembly.

It supports:

- Assembly scanning  
- Resource discovery  
- Configuration binding  
- DI registration boundaries  
- Middleware/endpoint discovery  

This class is never instantiated; it exists purely as a stable anchor.

---

# How the API Layer Fits Into the Identity Architecture

```
[ Api Assembly ]
       ↓
[ Platform: Services + Middleware ]
       ↓
[ Middleware: Observations, Sessions, Authorization ]
       ↓
[ Endpoints ]
       ↓
[ Application Layer ]
       ↓
[ Domain ]
       ↓
[ Persistence / Infrastructure ]
```

The API layer defines:

- **How** Identity subsystems are wired together  
- **How** requests flow through middleware  
- **How** the frontend and backend coordinate  
- **Where** assembly‑level scanning and configuration begin  

---

# Summary

The API folder provides:

### **Platform**
The service and middleware composition roots.

### **Settings**
Strongly‑typed configuration for cross‑application flows.

### **AssemblyMarker**
A stable anchor for scanning and DI boundaries.

Together, these components define the **Identity API assembly root**, ensuring a
clean, predictable, and well‑structured runtime environment for all endpoints
hosted in this project.

---
