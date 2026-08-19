# CampFitFurDogs.Api.Abstractions

The **CampFitFurDogs.Api.Abstractions** namespace defines the core public HTTP
contract for the Camp Fit Fur Dogs API.  
It contains the request and response DTOs used by all vertical slices, along with
syntactic validators that ensure incoming payloads are well‑formed before reaching
the Application layer.

This namespace contains **no domain logic**, **no application logic**, and
**no endpoint routing**. It exists solely to define the shape of the API.

---

## Purpose

This namespace provides:

- Request DTOs for incoming API operations  
- Response DTOs returned by endpoint implementations  
- FluentValidation validators for request DTOs  
- Documentation for the API’s public contract  

All business logic is delegated to the Application layer, and all persistence or
infrastructure concerns are handled by the Infrastructure layer.

---

## Structure

The namespace is organized by vertical slice:

```
CampFitFurDogs.Api.Abstractions
└── Endpoints
    ├── Dogs
    │   ├── EditDogEndpointRequest.cs
    │   ├── EditDogEndpointRequestValidator.cs
    │   ├── GetDogEndpointResponse.cs
    │   ├── GetDogSummaryEndpointResponse.cs
    │   ├── ListDogsByCurrentUserEndpointResponse.cs
    │   ├── RegisterDogEndpointRequest.cs
    │   ├── RegisterDogEndpointRequestValidator.cs
    │   └── RegisterDogEndpointResponse.cs
    └── (future slices)
```

Each slice defines its own DTOs under a dedicated sub‑namespace.

---

## Responsibilities

### **Request DTOs**
Define the shape of incoming data for each endpoint.

### **Response DTOs**
Define the shape of outgoing data returned to clients.

### **Validators**
Enforce syntactic correctness only:

- Required fields  
- Maximum lengths  
- ISO‑8601 date formats  
- Enum‑like constraints  

Example:

```csharp
RuleFor(x => x.DateOfBirth)
    .NotEmpty()
    .Matches(@"^\d{4}-\d{2}-\d{2}$");
```

Validators do **not** enforce domain rules.

---

## Relationship to Endpoint Implementations

Endpoint classes live under:

```
CampFitFurDogs.Api/Endpoints/<Slice>
```

Endpoints:

- Accept request DTOs from this namespace  
- Produce response DTOs from this namespace  
- Delegate logic to Application commands and queries  
- Contain no domain or business logic  

This separation ensures a clean, predictable vertical slice architecture.

---

## Design Principles

### **Purity**
DTOs contain no logic—only data.

### **Safety**
DTOs expose only fields intended for public API consumption.

### **Minimalism**
DTOs include only what the client needs; no internal fields are leaked.

### **Predictability**
All slice‑specific DTOs live in one place, making the API contract easy to navigate.

---

## What Does *Not* Belong Here

Do **not** add:

- Endpoint classes  
- Domain models  
- Application commands or queries  
- Infrastructure services  
- Test utilities  
- Business rules  

Those belong in their respective layers.

---

## When to Add Code Here

Add code to this namespace when:

- You introduce a new endpoint requiring a request or response DTO  
- You add a validator for a request DTO  
- You update the public API contract for a vertical slice  

This namespace should remain small, stable, and focused on the API contract.
