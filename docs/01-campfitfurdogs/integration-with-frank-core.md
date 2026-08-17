# Integration with Frank Core

CampFitFurDogs is built on top of the **Frank Core Platform**, a reusable foundation that provides identity, persistence, routing, request handling, and cross‑cutting concerns. The product application consumes these abstractions rather than re‑implementing them, allowing the team to focus exclusively on business logic.

This document explains how CampFitFurDogs integrates with Frank Core across all layers of the system.

---

## Dependency Flow

The full request pipeline flows through Frank Core before reaching CampFitFurDogs logic:

```
User Request
    ↓
Frank.Core.Api.Middleware (CORS, Security, Observations)
    ↓
Frank.Core.Api.Routing (IEndpoint discovery)
    ↓
CampFitFurDogs.Api.Endpoints (Feature endpoints)
    ↓
Frank.Core.Application (ICommandDispatcher, IQueryDispatcher)
    ↓
CampFitFurDogs.Application (Handlers with business logic)
    ↓
Frank.Core.Infrastructure (Result, Validation, Clock)
    ↓
CampFitFurDogs.Domain (Dog aggregate, value objects, rules)
    ↓
Frank.Core.EntityFrameworkCore (DbContext, migrations)
    ↓
CampFitFurDogs.Infrastructure (Persistence mappers and writers)
    ↓
PostgreSQL Database
```

Frank provides the platform; CampFitFurDogs provides the product‑specific behavior.

---

## What CampFitFurDogs Uses from Frank

### Composition Root (Program.cs)

CampFitFurDogs composes its platform by layering Frank modules:

```csharp
builder.Services
    .AddCampFitFurDogsApiPlatform(builder.Configuration)
    .AddFrankCoreApiPlatform(builder.Configuration)
    .AddFrankIdentityApiPlatform(builder.Configuration);

app
    .UseFrankCoreApiPlatform()
    .UseFrankIdentityApiPlatform()
    .MapRegisteredApiEndpoints("/api");
```

**Frank provides:**

- API middleware (CORS, security headers, exception handling)
- endpoint discovery and routing
- identity and authentication
- platform‑level configuration

CampFitFurDogs only adds its own endpoints and application logic.

---

## Application Layer Integration

Command handlers use Frank’s CQRS abstractions:

```csharp
public sealed class RegisterDogCommandHandler 
    : ICommandHandler<RegisterDogCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(
        RegisterDogCommand command,
        CancellationToken ct)
    {
        // Validate using Frank's Result pattern
        // Dispatch domain logic using Frank's Clock
        // Persist using Frank's UnitOfWork
        // Return Result<Guid>
    }
}
```

Frank provides:

- `ICommandDispatcher`  
- `IQueryDispatcher`  
- `Result<T>`  
- validation helpers  
- clock/time abstractions  

CampFitFurDogs provides the actual business logic.

---

## Domain Layer Integration

Domain models inherit Frank’s base classes:

```csharp
public sealed class Dog : AggregateRoot<DogId>
{
    public UserId OwnerId { get; private set; }  // From Frank.Identity

    public static Result<Dog> Create(...)
    {
        // Apply domain rules
        // Return success or failure using Frank.Core.Result<T>
    }
}
```

Frank provides:

- `AggregateRoot<TId>`  
- `ValueObject`  
- `AggregateId`  
- identity primitives (`UserId`)  

CampFitFurDogs defines the domain rules and invariants.

---

## Persistence Layer Integration

Persistence uses Frank’s EF Core foundation:

```csharp
public sealed class RegisterDogWriter : IRegisterDogWriter
{
    private readonly AppDbContext _context;  // From Frank.Core.EntityFrameworkCore
    
    public async Task AddAsync(Dog dog, CancellationToken ct)
    {
        var entity = DogEntityMapper.ToEntity(dog);
        _context.Dogs.Add(entity);
        // Commit via IAppUnitOfWork (Frank pattern)
    }
}
```

Frank provides:

- base DbContext patterns  
- migrations infrastructure  
- unit of work base class  
- EF Core configuration helpers  

CampFitFurDogs provides the actual entity mappings and persistence logic.

---

## Endpoint Layer Integration

Endpoints use Frank’s routing, DI, and identity abstractions:

```csharp
public sealed class RegisterDogEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapPost("/dogs", RegisterAsync)
            .Produces(StatusCodes.Status201Created)
            .WithOpenApi();
    }

    private static async Task<IResult> RegisterAsync(
        RegisterDogEndpointRequest request,
        ICurrentUser currentUser,        // From Frank.Identity
        ICommandDispatcher dispatcher,   // From Frank.Core
        CancellationToken ct)
    {
        // Validate, dispatch, return results using Frank's abstractions
    }
}
```

Frank provides:

- endpoint discovery (`IEndpoint`)
- routing conventions
- identity (`ICurrentUser`)
- result helpers (`IResult`)

CampFitFurDogs provides the endpoint logic and request/response models.

---

## Benefits of Integrating with Frank

- **Focus on business logic**  
  CampFitFurDogs implements dog management rules, not framework plumbing.

- **Reuse proven patterns**  
  CQRS, DDD, EF Core conventions, identity, exception handling.

- **Code sharing across products**  
  Frank.Core is reusable by multiple applications.

- **Simplified testing**  
  Mock Frank abstractions instead of EF Core or HTTP.

- **Consistent architecture**  
  Every feature follows the same patterns and lifecycle.

---

## Extension Points

Frank provides hooks for product‑specific behavior:

- **Hosting Modules** — environment‑specific configuration  
- **Custom Middleware** — product‑specific request handling  
- **Domain Events** — publish/subscribe across vertical slices  
- **Validation Rules** — extend Frank’s validation pipeline  

CampFitFurDogs can extend the platform without modifying it.

---

## Versioning

- Frank.Core and Frank.Identity are versioned independently  
- Breaking changes are coordinated across dependent products  
- Products may migrate to new Frank versions at different times  

This allows CampFitFurDogs to evolve without being tightly coupled to Frank’s release cycle.

