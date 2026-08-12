# CampFitFurDogs.Api

The **CampFitFurDogs.Api** assembly defines the production HTTP boundary of the Camp Fit Fur Dogs platform.  
It exposes all public API endpoints, configures routing, registers vertical slices, and applies API‑level middleware.  
This assembly contains **no domain logic** and **no test endpoints**.

---

## Purpose

This project provides:

- The production API surface for all vertical slices  
- Endpoint registration via `IEndpoint` implementations  
- API‑level middleware and exception handling  
- Hosting modules for external integrations  
- Assembly markers for endpoint discovery  
- Service collection extensions for API‑specific DI wiring  

All business logic is delegated to the Application and Domain layers.

---

## Project Structure

```
CampFitFurDogs.Api
├── Abstractions
│   └── Endpoints
│       └── Dogs
│           ├── EditDogEndpointRequest.cs
│           ├── GetDogEndpointResponse.cs
│           ├── GetDogSummaryEndpointResponse.cs
│           ├── ListDogsByCurrentUserEndpointResponse.cs
│           ├── RegisterDogEndpointRequest.cs
│           └── RegisterDogEndpointResponse.cs
├── Endpoints
│   ├── Dogs
│   │   ├── EditDogEndpoint.cs
│   │   ├── GetDogEndpoint.cs
│   │   ├── ListDogsByCurrentUserEndpoint.cs
│   │   ├── RegisterDogEndpoint.cs
│   │   ├── RemoveDogEndpoint.cs
│   │   └── ServiceCollectionExtensions.cs
│   ├── Health
│   │   ├── GetHealthEndpoint.cs
│   │   └── ServiceCollectionExtensions.cs
│   └── ServiceCollectionExtensions.cs
├── ExceptionHandlers
│   ├── BadConfigurationExceptionHandler.cs
│   ├── BadRequestExceptionHandler.cs
│   ├── DomainExceptionHandler.cs
│   ├── DuplicateEmailExceptionHandler.cs
│   ├── ExternalAuthProviderExceptionHandler.cs
│   ├── UnexpectedExceptionHandler.cs
│   ├── UserIdClaimNotFoundExceptionHandler.cs
│   ├── UserNotAuthenticatedExceptionHandler.cs
│   └── ValidationExceptionHandler.cs
├── HostingModules
│   ├── GitHubArtifactClient.cs
│   ├── IGitHubArtifactClient.cs
│   ├── IRenderPrParser.cs
│   ├── RenderPrParser.cs
│   └── RenderPrPreviewHostingModule.cs
├── Platform
│   └── ServiceCollectionExtensions.cs
├── Helpers
│   └── Hosting.cs
├── AssemblyMarker.cs
└── Program.cs
```

---

## Key Components

### **AssemblyMarker**

A zero‑logic type used to anchor assembly scanning:

```csharp
public sealed class AssemblyMarker { }
```

This allows the framework to:

- Discover endpoints in this assembly  
- Apply API‑wide conventions  
- Register vertical slices cleanly  

### **Endpoints**

Endpoints follow the Frank.Core model:

- Implement `IEndpoint`  
- Map routes using `RouteGroupBuilder`  
- Contain no domain logic  
- Return safe DTOs defined in `Abstractions/Endpoints`  

Vertical slices live under:

- `Endpoints/Dogs`
- `Endpoints/Health`

Each slice has:

- Request/response DTOs  
- Endpoint implementations  
- Slice‑specific DI extensions  

### **Exception Handlers**

The `ExceptionHandlers` folder contains API‑level exception mapping:

- Domain exceptions → HTTP responses  
- Validation errors → structured payloads  
- Authentication/authorization errors → safe responses  

These handlers ensure consistent error semantics across the API.

### **Hosting Modules**

The `HostingModules` folder contains integrations used during hosting:

- GitHub artifact retrieval  
- Render PR preview parsing  
- External hosting utilities  

These modules are optional and environment‑specific.

### **Platform Extensions**

The `Platform` folder contains API‑level DI extensions for platform‑wide concerns.

---

## Routing Model

The API uses a single top‑level group:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

All endpoints map **relative** to this group:

- `/dogs/{id}` → `/api/dogs/{id}`
- `/health` → `/api/health`

No endpoint ever hard‑codes `/api`.

---

## Design Principles

### **Purity**
Endpoints contain no domain logic.

### **Delegation**
Endpoints orchestrate; pipelines execute.

### **Safety**
Sensitive data is never returned.

### **Minimalism**
Endpoints return only what the client needs.

### **Predictability**
Routing, error handling, and DTO shape are consistent across slices.

---

## What Does *Not* Belong Here

Do **not** add:

- Test endpoints (`/__test__/…`)  
- Domain logic  
- Application pipelines  
- Infrastructure concerns  
- Business rules  

Those belong in their respective layers or in `CampFitFurDogs.TestUtilities`.

---

## When to Add Code Here

Add code to **CampFitFurDogs.Api** when:

- You are exposing a new production endpoint  
- You are adding a new vertical slice  
- You are configuring API‑level middleware  
- You are wiring API‑specific DI  

---

