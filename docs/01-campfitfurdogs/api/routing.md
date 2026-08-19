# Routing

Routing in **CampFitFurDogs.Api** is built on the Frank.Core endpoint abstraction.  
All HTTP routes are defined through `IEndpoint` implementations and automatically discovered at startup.  
The API layer remains declarative and host‑agnostic — routing is *activated* by the **CampFitFurDogs.Host** project, not by the API itself.

This separation ensures the API assembly defines *what* routes exist, while the Host assembly defines *how* they are hosted.

---

## Registration Pattern

Every endpoint implements `IEndpoint` and provides a `Map(RouteGroupBuilder api)` method.  
The endpoint binds request data, delegates to the application layer, and returns a typed response.

```csharp
public class RegisterDogEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapPost("/dogs", async (/* parameters */) =>
        {
            // handler logic
        });
    }
}
```

Endpoints remain thin and focused, containing no domain or persistence logic.

---

## Route Prefix (Applied by the Host Layer)

The API assembly **does not** apply route prefixes.  
Instead, the **Host project** groups all product endpoints under `/api`:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

This means:

- `POST /dogs` → `POST /api/dogs`  
- `GET /dogs/{id}` → `GET /api/dogs/{id}`  

This prefixing is a **hosting concern**, not an API concern.

It ensures a clean separation between:

- product endpoints (`/api/...`)
- platform endpoints (e.g., identity, health checks)

---

## Endpoint Discovery

Endpoints are discovered via assembly scanning performed by the **Host** project.

Two extension methods register all known endpoint sets:

- `AddFrankIdentityApiEndpoints()` — identity endpoints (login, logout, callback)  
- `AddCampFitFurDogsApiEndpoints()` — product endpoints (dog management)  

These methods scan their respective assemblies for `IEndpoint` implementations and register them automatically.

This approach ensures:

- no manual route registration  
- consistent route grouping  
- easy extension when new vertical slices are added  
- host‑controlled startup behavior

---

## Request Flow

A typical endpoint follows a predictable flow:

1. **Accept the HTTP request** and bind the request body or route parameters  
2. **Emit request validation observability events** (API layer)  
3. **Resolve dependencies** from DI (e.g., `ICurrentUser`, `ICommandDispatcher`)  
4. **Construct a command or query** representing the requested operation  
5. **Dispatch through the CQRS pipeline**  
6. **Return a typed response**, typically:  
   - `201 Created`  
   - `200 OK`  
   - `204 No Content`  

Validation and exception handling are performed by platform middleware activated by the Host project.

---

## Host Layer Extraction

Routing used to be configured inside the API assembly (via Program.cs).  
This is no longer the case.

Routing is now activated exclusively by **CampFitFurDogs.Host**, which:

- builds the WebApplication  
- applies hosting modules  
- registers platform services  
- maps all API endpoints under `/api`  
- configures middleware  
- runs the application

The API assembly defines endpoints; the Host assembly hosts them.

This keeps the API pure, reusable, and environment‑agnostic.

---

## Summary

The routing system provides:

- automatic endpoint discovery  
- host‑controlled route prefixing  
- thin, declarative endpoint definitions  
- seamless integration with CQRS and platform middleware  
- structured request validation observability  
- clean separation between API definition and hosting behavior  

Routing remains predictable, maintainable, and fully aligned with the vertical‑slice architecture.
