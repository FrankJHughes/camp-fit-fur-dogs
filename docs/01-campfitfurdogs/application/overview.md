# Application Layer

The application layer coordinates the Dogs vertical slice using CQRS commands, queries, and validators. It acts as the orchestrator between the API layer, domain model, and infrastructure persistence, ensuring that each use case is expressed cleanly and consistently.

## Organization

Each use case in the Dogs slice has its own folder, keeping command, query, handler, and validator logic cohesive:

- `Dogs/RegisterDog/` — `RegisterDogCommand`, handler, validator  
- `Dogs/EditDog/` — `EditDogCommand`, handler, validator  
- `Dogs/RemoveDog/` — `RemoveDogCommand`, handler  
- `Dogs/GetDog/` — `GetDogQuery`, handler  
- `Dogs/ListDogsByOwner/` — `ListDogsByOwnerQuery`, handler  
- `Dogs/ServiceCollectionExtensions.cs` — DI registration for all Dogs handlers and validators  

This structure ensures vertical‑slice clarity: each feature’s behavior is easy to trace end‑to‑end.

## Registration Pattern

When `AddCampFitFurDogsApplication()` is invoked from `Program.cs`:

1. It calls `AddApplicationDogs()` to register all command and query handlers  
2. It discovers and registers all `IValidator<T>` implementations via FluentValidation  
3. All handlers and validators are added to the DI container with appropriate lifetimes  

This keeps the application layer declarative and avoids manual registration boilerplate.

## Command and Query Flow

The application layer follows a consistent flow for all operations:

1. **Endpoint** receives the HTTP request and constructs a command or query  
2. **FluentValidation** automatically validates structural constraints  
3. **Handler** executes business logic (reads, writes, calculations)  
4. **Unit of Work** commits transactions for commands  
5. **Response** is returned to the endpoint  

Handlers assume valid input because validation occurs before execution.

## Abstractions

The `Abstractions/` folder contains the contracts that decouple the application layer from infrastructure:

- `RegisterDogCommand` — command DTO  
- `IRegisterDogWriter` — write‑side persistence contract  
- Similar abstractions for edit, remove, get, and list operations  

These abstractions ensure that:

- the application layer depends only on interfaces  
- infrastructure can evolve independently  
- domain logic remains pure and unaffected by EF Core or transport concerns  

## Summary

The application layer provides:

- cohesive vertical slices  
- clear separation of commands and queries  
- automatic validation  
- decoupled persistence via abstractions  
- predictable CQRS flow across all dog‑related operations  

It is the central coordination point for business behavior in the CampFitFurDogs product.

