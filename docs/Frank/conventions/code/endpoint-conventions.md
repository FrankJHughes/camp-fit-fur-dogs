# Endpoint Conventions (Frank)

Frank governs endpoint discovery and registration, but does not define product
routes or business behavior. Endpoints remain product‑owned; Frank provides the
infrastructure.

---

## Discovery

Endpoints are discovered automatically when they implement:

````csharp
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
````

Frank scans assemblies and registers all endpoint types.

Products must not manually register endpoints.

---

## Responsibilities

Frank ensures:

- endpoint discovery  
- endpoint ordering  
- consistent mapping behavior  
- integration with security headers middleware  
- integration with hosting abstractions  

Frank does **not**:

- define routes  
- define authorization rules  
- define DTOs  
- define business logic  

---

## Prohibitions

Endpoints must not:

- contain protocol logic  
- contain business logic  
- perform persistence  
- construct aggregates  
- bypass the dispatcher  
