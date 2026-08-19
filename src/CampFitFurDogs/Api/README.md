# CampFitFurDogs.Api

The **CampFitFurDogs.Api** assembly defines the production HTTP boundary of the Camp Fit Fur Dogs platform.  
It exposes all public API endpoints, request/response DTOs, validators, exception handlers, and API‑specific DI wiring.  
This assembly contains **no hosting logic**, **no Program.cs**, and **no domain or application behavior**.

The API layer is intentionally thin: it shapes the HTTP contract and delegates all real work to the Application and Domain layers.

---

## Purpose

This project provides:

- Production API endpoints for all vertical slices  
- Request/response DTOs and syntactic validators  
- API‑level exception handling  
- API‑specific DI registration  
- Hosting modules used by the Host project  
- Assembly markers for endpoint discovery  

All business logic is delegated to:

- `CampFitFurDogs.Application`  
- `CampFitFurDogs.Domain`

All persistence and mechanical concerns are delegated to:

- `CampFitFurDogs.Infrastructure`

---

## Project Structure

```
CampFitFurDogs.Api
├── Abstractions
│   └── Endpoints
│       └── Dogs
│           ├── EditDogEndpointRequest.cs
│           ├── EditDogEndpointRequestValidator.cs
│           ├── GetDogEndpointResponse.cs
│           ├── GetDogSummaryEndpointResponse.cs
│           ├── ListDogsByCurrentUserEndpointResponse.cs
│           ├── RegisterDogEndpointRequest.cs
│           ├── RegisterDogEndpointRequestValidator.cs
│           └── RegisterDogEndpointResponse.cs
│
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
│
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
│
├── HostingModules
│   ├── RenderPrPreviewHostingModule.cs
│   ├── GitHubArtifactClient.cs
│   ├── IGitHubArtifactClient.cs
│   ├── RenderPrParser.cs
│   └── IRenderPrParser.cs
│
├── Platform
│   └── ServiceCollectionExtensions.cs
│
├── AssemblyMarker.cs
└── README.md
```

---

## Responsibilities

### **Endpoints**
- Implement `IEndpoint`  
- Map routes using `RouteGroupBuilder`  
- Contain no domain logic  
- Return safe DTOs from `Abstractions/Endpoints`

### **DTOs & Validators**
- Define the HTTP contract  
- Enforce syntactic correctness only  
- Never enforce domain rules

### **Exception Handlers**
- Convert domain/application exceptions into consistent HTTP responses  
- Ensure predictable error semantics across slices

### **Hosting Modules**
Hosting modules live in the API assembly but are **executed by the Host project**.

They provide:

- Render PR Preview configuration overrides  
- GitHub artifact integration  
- Environment‑specific behavior

### **Platform Extensions**
API‑level DI registration for:

- Endpoint scanning  
- Exception handler registration  
- API conventions

---

## What Does *Not* Belong Here

Do **not** add:

- Program.cs  
- Hosting orchestration  
- Domain logic  
- Application pipelines  
- Infrastructure concerns  
- Test endpoints

---

## Summary

The **CampFitFurDogs.Api** assembly defines the HTTP boundary of the platform:

- Pure endpoints  
- Pure DTOs  
- Pure validators  
- Pure exception handling  
- Pure API DI

It is host‑agnostic, environment‑agnostic, and free of startup logic.
