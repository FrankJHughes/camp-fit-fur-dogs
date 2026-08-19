# CampFitFurDogs.Api.Abstractions.Endpoints.Dogs

The **CampFitFurDogs.Api.Abstractions.Endpoints.Dogs** namespace defines all
request and response DTOs used by the Dogs vertical slice of the Camp Fit Fur Dogs
production API.  
These types represent the **public HTTP contract** for dog‑related operations and
are consumed by endpoint implementations under `Endpoints/Dogs`.

This namespace contains **no domain logic**, **no application logic**, and
**no endpoint routing**. It exists solely to define the shape of the API.

---

## Purpose

This namespace provides:

- Request DTOs for dog‑related API operations  
- Response DTOs returned by dog endpoints  
- Input validators for request DTOs  
- Documentation for the dog slice’s public API surface  

All business logic is delegated to the Application layer, and all persistence or
infrastructure concerns are handled by the Infrastructure layer.

---

## Included DTOs

### **EditDogEndpointRequest**
Represents the payload required to edit an existing dog profile.

### **RegisterDogEndpointRequest**
Represents the payload required to register a new dog.

### **GetDogEndpointResponse**
Represents the full dog profile returned by `GET /dogs/{id}`.

### **GetDogSummaryEndpointResponse**
Represents a lightweight dog summary used in list views.

### **ListDogsByCurrentUserEndpointResponse**
Represents the collection of dogs owned by the authenticated user.

### **RegisterDogEndpointResponse**
Represents the result of registering a new dog.

---

## Validators

Each request DTO has a corresponding FluentValidation validator:

- `EditDogEndpointRequestValidator`
- `RegisterDogEndpointRequestValidator`

Validators enforce **syntactic correctness only** (required fields, formats,
lengths). They do not enforce domain rules.

Example validator:

```csharp
public sealed class RegisterDogEndpointRequestValidator : AbstractValidator<RegisterDogEndpointRequest>
{
    public RegisterDogEndpointRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Breed).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Matches(@"^\d{4}-\d{2}-\d{2}$");
        RuleFor(x => x.Sex)
            .NotEmpty()
            .Must(v => v is "Male" or "Female");
    }
}
```

---

## Relationship to Endpoint Implementations

Endpoint classes live under:

```
CampFitFurDogs.Api/Endpoints/Dogs
```

Those endpoints:

- Accept request DTOs from this namespace  
- Produce response DTOs from this namespace  
- Delegate all logic to Application commands and queries  
- Contain no domain or business logic  

This separation ensures a clean, predictable vertical slice structure.

---

## Design Principles

### **Purity**
DTOs contain no logic—only data.

### **Safety**
DTOs expose only fields intended for public API consumption.

### **Minimalism**
DTOs include only what the client needs; no internal fields are leaked.

### **Predictability**
All dog‑related DTOs live in one namespace, making the slice easy to navigate.

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

- You introduce a new dog‑related endpoint requiring a request or response DTO  
- You add a validator for a dog‑related request DTO  
- You update the public API contract for the Dogs slice  

This namespace should remain small, stable, and focused on the API contract.
