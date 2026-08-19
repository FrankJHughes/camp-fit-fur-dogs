# Request Validation Observability

The **CampFitFurDogs.Api** layer includes a dedicated observability pipeline for API‑level request validation.  
This system provides structured, correlation‑aware diagnostics for all validation activity that occurs **before** the Application layer receives a request.

Request validation observability is **implemented inside the API layer**, but it **uses the Frank.Core observability engine** to emit structured events.  
Frank.Core supplies the *mechanism* (event pipeline, correlation IDs, logging), while CampFitFurDogs.Api supplies the *events* that describe validation behavior at the HTTP boundary.

---

## Relationship to Frank.Core

Frank.Core provides:

- the observations pipeline  
- correlation ID propagation  
- structured logging  
- duration measurement  
- exception observation  
- environment‑aware sinks  

However, Frank.Core does **not** define:

- API‑specific validation events  
- DTO‑specific metadata  
- slice‑specific validation behavior  
- FluentValidation integration  
- HTTP‑boundary validation semantics  

Those belong to the **CampFitFurDogs.Api** layer.

### In short:

- **Frank.Core = observability engine**  
- **CampFitFurDogs.Api = validation observability events**  

This separation keeps Frank.Core generic and reusable, while allowing the API layer to emit product‑specific diagnostics.

---

## Why Validation Observability Exists

API‑level validation failures often disappear into generic `400 Bad Request` responses, making it difficult to understand:

- which request failed  
- why it failed  
- how long validation took  
- whether the failure was syntactic or unexpected  
- whether the validator itself threw an exception  

The validation observability pipeline solves this by emitting structured events for every validation attempt, using Frank.Core’s observation engine.

This enables:

- correlation‑aware tracing  
- consistent diagnostics across slices  
- improved debugging during development  
- visibility into malformed client requests in production  
- PR preview introspection via hosting modules

---

## Event Taxonomy

The API layer emits four structured events for each validation attempt:

### **`api.validation.start`**
Emitted when validation begins.

Includes:
- correlation ID  
- route pattern  
- endpoint name  
- request DTO type  

### **`api.validation.end`**
Emitted when validation completes successfully.

Includes:
- correlation ID  
- duration (ms)  
- validator type  

### **`api.validation.failed`**
Emitted when validation rules fail.

Includes:
- correlation ID  
- validation errors  
- field‑level messages  
- number of failures  

### **`api.validation.exception`**
Emitted when the validator throws unexpectedly.

Includes:
- correlation ID  
- exception type  
- stack trace (if enabled)  

These events are **API‑specific**, but emitted through **Frank.Core’s observation engine**.

---

## How Validation Observability Works

Validation observability is implemented as part of the API’s request pipeline:

1. The Host project activates the API platform middleware.  
2. The API platform attaches a validation filter to all endpoints.  
3. The filter:
   - extracts the request DTO  
   - invokes the corresponding FluentValidation validator  
   - emits observability events via Frank.Core  
   - returns structured `ProblemDetails` on failure  
4. Only valid requests proceed to the Application layer.

This ensures that **invalid requests never reach business logic**.

---

## Integration with FluentValidation

Validators live in the API’s `Abstractions/Endpoints` folder.

Example:

```csharp
public class RegisterDogEndpointRequestValidator : AbstractValidator<RegisterDogEndpointRequest>
{
    public RegisterDogEndpointRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Breed).NotEmpty();
        RuleFor(x => x.DateOfBirth).NotEmpty();
    }
}
```

When this validator runs:

- `api.validation.start` is emitted  
- rules are evaluated  
- if successful → `api.validation.end`  
- if failed → `api.validation.failed`  
- if an exception occurs → `api.validation.exception`  

All events use Frank.Core’s correlation and observation infrastructure.

---

## Separation from Application Validation

Validation observability applies **only to API‑level syntactic validation**.

It does **not** replace or overlap with:

- Application‑level semantic validation  
- Domain invariants  
- Aggregate rules  
- Business logic validation  

API validation ensures the request is *well‑formed*.  
Application validation ensures the request is *meaningful*.  
Domain validation ensures the request is *allowed*.

Each layer emits its own observability events through Frank.Core.

---

## Host Activation

The validation observability pipeline is activated by the Host project:

```csharp
app.UseFrankCoreApiPlatform();
```

This ensures:

- observability is enabled in all environments  
- hosting modules can adjust behavior (e.g., PR preview logging)  
- the API assembly remains host‑agnostic  

The Host project does **not** implement validation logic — it simply wires the pipeline.

---

## Benefits

- **Full visibility** into malformed requests  
- **Consistent diagnostics** across slices  
- **Improved debugging** during development  
- **Better PR preview introspection**  
- **Safer API boundary** (invalid requests never reach business logic)  
- **Structured events** emitted through Frank.Core  

---

## Summary

Request validation observability is an API‑defined, Frank.Core‑powered diagnostic layer that provides structured, correlation‑aware visibility into all validation activity at the HTTP boundary.

It ensures:

- predictable validation behavior  
- consistent error reporting  
- clear separation of concerns  
- host‑activated, API‑defined observability  
- clean integration with Frank.Core’s observation engine  

This system strengthens the API boundary and improves reliability across all environments.
