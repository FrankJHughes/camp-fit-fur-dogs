# Routing

Routing in the CampFitFurDogs API is built around the platform’s endpoint abstraction. All HTTP routes are defined through `IEndpoint` implementations and automatically discovered at startup. This keeps the API layer declarative, consistent, and aligned with the vertical‑slice architecture.

## Registration Pattern

Every endpoint implements `IEndpoint` and provides a `Map(RouteGroupBuilder api)` method. The endpoint binds request data, delegates to the application layer, and returns a typed response.

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

## Route Prefix

All product endpoints are automatically grouped under the `/api` prefix:

```csharp
app.MapRegisteredApiEndpoints("/api");
```

This means:

- `POST /dogs` → `POST /api/dogs`  
- `GET /dogs/{id}` → `GET /api/dogs/{id}`  

The prefix ensures a clean separation between product endpoints and platform endpoints (e.g., identity).

## Endpoint Discovery

Endpoints are discovered via assembly scanning during startup. Two extension methods register all known endpoint sets:

- `AddFrankIdentityApiEndpoints()` — identity endpoints (login, logout, callback)  
- `AddCampFitFurDogsApiEndpoints()` — product endpoints (dog management)  

These methods scan their respective assemblies for `IEndpoint` implementations and register them automatically.

This approach ensures:

- no manual route registration  
- consistent route grouping  
- easy extension when new vertical slices are added  

## Request Flow

A typical endpoint follows a predictable flow:

1. **Accept the HTTP request** and bind the request body or route parameters  
2. **Resolve dependencies** from DI (e.g., `ICurrentUser`, `ICommandDispatcher`)  
3. **Construct a command or query** representing the requested operation  
4. **Dispatch through the CQRS pipeline**  
5. **Return a typed response**, typically:  
   - `201 Created`  
   - `200 OK`  
   - `204 No Content`  

All validation and exception handling are performed by platform middleware, keeping endpoint code minimal and focused.

## Summary

The routing system provides:

- automatic endpoint discovery  
- consistent route prefixing  
- thin, declarative endpoint definitions  
- seamless integration with CQRS and platform middleware  

This ensures that routing remains predictable, maintainable, and aligned with the vertical‑slice architecture.

