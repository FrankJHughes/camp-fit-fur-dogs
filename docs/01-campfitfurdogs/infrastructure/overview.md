# Infrastructure Layer

The infrastructure layer provides the concrete persistence implementation for the CampFitFurDogs platform. It fulfills the contracts defined by the application layer using Entity Framework Core, ensuring that domain aggregates are stored, retrieved, and updated consistently.

This layer is intentionally isolated from domain and application logic. It focuses solely on persistence, transactions, and runtime integration with the broader platform.

---

## Organization

The infrastructure layer is structured around EF Core and persistence abstractions:

- `DbContexts/AppDbContext.cs` — EF Core database context for all aggregates  
- `Dogs/RegisterDogWriter.cs` — implements `IRegisterDogWriter`  
- `Dogs/GetDogReader.cs` — optimized read‑side projection for single dogs  
- `Dogs/ListDogsReader.cs` — optimized read‑side projection for dog lists  
- `UnitOfWork/AppUnitOfWork.cs` — transactional boundary for write operations  
- `ServiceCollectionExtensions.cs` — DI registration for infrastructure services  
- `Migrations/` — EF Core migration files for schema evolution  

This structure keeps persistence concerns cohesive and discoverable.

---

## Dependency Inversion

The infrastructure layer implements contracts defined by the application layer. This ensures the application layer remains independent of EF Core, SQL, and database concerns.

### Application Defines the Abstraction

```csharp
public interface IRegisterDogWriter
{
    Task AddAsync(Dog dog, CancellationToken ct);
}
```

### Infrastructure Implements It

```csharp
public sealed class RegisterDogWriter : IRegisterDogWriter
{
    private readonly AppDbContext _db;

    public async Task AddAsync(Dog dog, CancellationToken ct)
    {
        _db.Dogs.Add(dog);
        await _db.SaveChangesAsync(ct);
    }
}
```

This inversion keeps the application layer pure and testable, while allowing infrastructure to evolve independently.

---

## Database Context

`AppDbContext` is the EF Core gateway for all persistence operations.

### Responsibilities

- Defines `DbSet<T>` collections for aggregates (`Dogs`, `Users`)  
- Configures entity mappings using fluent API  
- Applies value converters for strongly typed IDs and value objects  
- Manages database connections and transactions  
- Integrates with the Unit of Work  

### Lifetime

The DbContext is registered as **Scoped**, meaning:

- one instance per HTTP request  
- shared across all readers/writers within the request  
- disposed automatically at the end of the request  

This aligns with EF Core best practices and ensures consistent change tracking.

---

## Unit of Work

The `AppUnitOfWork` class provides a consistent transactional boundary for write operations:

```csharp
public sealed class AppUnitOfWork :
    EntityFrameworkCoreUnitOfWorkBase<AppDbContext>,
    IAppUnitOfWork
{
    // Inherits CommitAsync() and RollbackAsync()
}
```

### Handler Usage

```csharp
await _unitOfWork.CommitAsync(ct); // Commits all tracked changes
```

### Guarantees

- **Atomicity** — all changes succeed or none do  
- **Consistency** — domain invariants remain intact  
- **Isolation** — each request has its own transaction scope  

The Unit of Work ensures that write operations behave predictably across vertical slices.

---

## Infrastructure Responsibilities

The infrastructure layer is responsible for:

- implementing persistence abstractions  
- mapping domain aggregates to relational tables  
- projecting optimized DTOs for read operations  
- managing transactions and database connections  
- applying EF Core migrations  
- integrating with the broader platform (configuration, DI, logging)  

It does **not** contain domain logic, validation, or application orchestration.

---

## Composition Flow (API → Application → Infrastructure)

1. **API Layer**  
   Endpoint receives request → constructs command/query.

2. **Application Layer**  
   Handler executes → uses persistence abstractions.

3. **Infrastructure Layer**  
   Writers/readers use DbContext → EF Core executes SQL → Unit of Work commits.

4. **Database**  
   PostgreSQL stores and retrieves data.

This flow maintains strict separation of concerns and vertical‑slice clarity.

---

## Summary

The infrastructure layer provides:

- EF Core persistence for domain aggregates  
- optimized read‑side projections  
- transactional consistency via Unit of Work  
- clean separation from application and domain layers  
- schema evolution through migrations  

It is the foundation of reliable, predictable persistence behavior across the CampFitFurDogs platform.

