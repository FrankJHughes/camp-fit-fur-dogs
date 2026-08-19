# CampFitFurDogs.Api.Abstractions.Endpoints

The **CampFitFurDogs.Api.Abstractions.Endpoints** namespace defines the public
HTTP contract for all vertical slices in the Camp Fit Fur Dogs API.  
It contains request and response DTOs, along with syntactic validators, used by
endpoint implementations under `CampFitFurDogs.Api.Endpoints`.

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

## Vertical Slice Structure

Each vertical slice defines its own DTOs under a dedicated sub‑namespace:

```
CampFitFurDogs.Api.Abstractions.Endpoints
└── Dogs
    ├── EditDogEndpointRequest.cs
    ├── EditDogEndpointRequestValidator.cs
    ├── GetDogEndpointResponse.cs
    ├── GetDogSummaryEndpointResponse.cs
    ├── ListDogsByCurrentUserEndpointResponse.cs
    ├── RegisterDogEndpointRequest.cs
    ├── RegisterDogEndpointRequestValidator.cs
    └── RegisterDogEndpointResponse.cs
```

Slices may add additional folders as needed, but all DTOs remain within
`Abstractions/Endpoints`.

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
