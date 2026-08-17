# Dogs Application Layer (Vertical Slice)

The Dogs application layer contains all CQRS handlers, validators, and orchestration logic for dog‑related operations. It follows the vertical‑slice architecture pattern, keeping all dog‑specific behavior cohesive across commands, queries, and persistence abstractions.

## Vertical Slice Organization

The Dogs slice is organized into command and query workflows, each with dedicated handlers and validators:

```
CampFitFurDogs.Application.Dogs/
├── RegisterDog/
│   ├── RegisterDogCommand.cs
│   ├── RegisterDogCommandHandler.cs
│   └── RegisterDogCommandValidator.cs
├── EditDog/
│   ├── EditDogCommand.cs
│   ├── EditDogCommandHandler.cs
│   └── EditDogCommandValidator.cs
├── RemoveDog/
│   ├── RemoveDogCommand.cs
│   └── RemoveDogCommandHandler.cs
├── GetDog/
│   ├── GetDogQuery.cs
│   └── GetDogQueryHandler.cs
├── ListDogsByOwner/
│   ├── ListDogsByOwnerQuery.cs
│   └── ListDogsByOwnerQueryHandler.cs
└── ServiceCollectionExtensions.cs
```

This structure ensures that all dog‑related logic remains cohesive and easy to trace from API → Application → Domain → Infrastructure.

---

## Commands (Write Operations)

Commands represent state‑changing operations. Each command has a validator and a handler.

### RegisterDogCommand

Registers a new dog under the current authenticated owner.

**Request:**
```csharp
public record RegisterDogCommand(
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
```

**Validation Rules:**
- Name must not be empty  
- Breed must not be empty  
- DateOfBirth must be in the past  
- Sex must be `"Male"` or `"Female"`  

**Handler Workflow:**
1. Validate input via `RegisterDogCommandValidator`  
2. Construct domain value objects (`DogName`, `Breed`)  
3. Create `Dog` aggregate using factory method  
4. Persist via `IRegisterDogWriter`  
5. Commit transaction via `IAppUnitOfWork`  
6. Return the new dog ID  

---

### EditDogCommand

Updates an existing dog’s information.

**Request:**
```csharp
public record EditDogCommand(
    Guid DogId,
    string Name,
    string Breed,
    DateOnly DateOfBirth);
```

**Handler Responsibilities:**
- Load existing dog aggregate  
- Verify ownership (current user must own the dog)  
- Apply updates through aggregate methods  
- Persist changes atomically  

---

### RemoveDogCommand

Deletes a dog from the system.

**Request:**
```csharp
public record RemoveDogCommand(Guid DogId);
```

**Handler Responsibilities:**
- Load existing dog  
- Verify ownership  
- Delete via writer abstraction  
- Commit transaction  

---

## Queries (Read Operations)

Queries retrieve data without modifying state. They use optimized read‑side abstractions.

### GetDogQuery

Retrieves a single dog by ID.

**Request:**
```csharp
public record GetDogQuery(Guid DogId);
```

**Response:**
```csharp
public record DogDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Breed,
    DateOnly DateOfBirth,
    string Sex);
```

**Implementation Notes:**
- Uses `IGetDogReader`  
- Returns DTO without instantiating domain objects  
- Single optimized database query  

---

### ListDogsByOwnerQuery

Retrieves all dogs owned by a specific user.

**Request:**
```csharp
public record ListDogsByOwnerQuery(Guid OwnerId);
```

**Response:**
```csharp
public record DogListItemDto(
    Guid Id,
    string Name,
    string Breed,
    int AgeInYears);
```

**Implementation Notes:**
- Uses `IListDogsByOwnerReader`  
- Lightweight DTOs optimized for list views  
- Supports pagination in real‑world scenarios  

---

## Persistence Abstractions

The application layer depends on persistence contracts defined in abstractions. These interfaces decouple application logic from EF Core implementations.

### Write Contracts (Commands)
- `IRegisterDogWriter` — create new dogs  
- `IEditDogWriter` — update existing dogs  
- `IRemoveDogWriter` — delete dogs  

### Read Contracts (Queries)
- `IGetDogReader` — fetch dog details  
- `IGetDogByIdReader` — load aggregate for modification  
- `IListDogsByOwnerReader` — list dogs for an owner  

See **Dogs Persistence** for implementation details.

---

## Service Registration

All Dogs handlers and validators are registered via `AddApplicationDogs()`:

```csharp
public static IServiceCollection AddApplicationDogs(
    this IServiceCollection services)
{
    return services
        .AddFrankCoreApplicationCqrsCommands(
            [typeof(AssemblyMarker).Assembly],
            discoveryOptions => updateOptions(discoveryOptions))
        .AddFrankCoreApplicationCqrsQueries(
            [typeof(AssemblyMarker).Assembly],
            discoveryOptions => updateOptions(discoveryOptions));
}
```

This method:

- scans the assembly for `ICommandHandler<>` implementations  
- scans for `IQueryHandler<,>` implementations  
- registers all handlers as transient  
- enables dispatching via `ICommandDispatcher` and `IQueryDispatcher`  

---

## Error Handling

Exceptions thrown inside Dogs handlers flow into the global exception‑handling pipeline:

- **Domain Exceptions** — invariant violations  
- **Validation Exceptions** — FluentValidation failures  
- **Infrastructure Exceptions** — database or I/O errors  

All are converted into structured `ProblemDetails` responses.

---

## Testing Strategy

See **Testing Strategy** for full guidelines.

**Typical Handler Test Pattern (AAA):**

```csharp
[Fact]
public async Task RegisterDogCommandHandler_WithValidInput_CreatesNewDog()
{
    // Arrange
    var command = new RegisterDogCommand(
        ownerId: _userId,
        name: "Buddy",
        breed: "Golden Retriever",
        dateOfBirth: new DateOnly(2020, 1, 15),
        sex: "Male");

    // Act
    var dogId = await _handler.HandleAsync(command, CancellationToken.None);

    // Assert
    Assert.NotEqual(Guid.Empty, dogId);
    var savedDog = await _dogReader.GetDogByIdAsync(dogId, CancellationToken.None);
    Assert.Equal("Buddy", savedDog.Name);
}
```

---

## Source References

- `src/CampFitFurDogs/Application/Dogs/*` — Dogs CQRS handlers  
- `src/CampFitFurDogs/Application/Abstractions/Dogs/*` — persistence contracts  
- `src/Frank/Core/Application/Cqrs/*` — CQRS dispatcher implementations  
