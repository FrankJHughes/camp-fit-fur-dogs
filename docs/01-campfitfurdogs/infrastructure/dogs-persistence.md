# Dogs Persistence

The Dogs persistence layer provides the concrete EF Core implementations behind the application layer’s read and write abstractions. It ensures that all database interactions remain isolated from domain logic and application orchestration, preserving testability and vertical‑slice purity.

This subsystem is responsible for translating domain aggregates into relational storage and projecting optimized DTOs for query operations.

---

## Persistence Contracts

The application layer depends on a set of persistence abstractions. These interfaces define *what* persistence operations exist, not *how* they are implemented.

### Write‑Side Contracts

#### IRegisterDogWriter

Persists newly created dogs:

```csharp
public interface IRegisterDogWriter
{
    Task WriteAsync(Dog dog, CancellationToken cancellationToken);
}
```

**Responsibility:** Insert a new `Dog` aggregate into the database.

---

#### IEditDogWriter

Persists updates to existing dogs:

```csharp
public interface IEditDogWriter
{
    Task UpdateAsync(Dog dog, CancellationToken cancellationToken);
}
```

**Responsibility:** Update an existing `Dog` aggregate.

---

#### IRemoveDogWriter

Deletes dogs:

```csharp
public interface IRemoveDogWriter
{
    Task DeleteAsync(DogId dogId, CancellationToken cancellationToken);
}
```

**Responsibility:** Remove a `Dog` aggregate by ID.

---

### Read‑Side Contracts

#### IGetDogReader

Fetches a single dog as a DTO:

```csharp
public interface IGetDogReader
{
    Task<DogDto?> GetDogAsync(DogId dogId, CancellationToken cancellationToken);
}
```

**Returns:** A lightweight DTO optimized for API responses.

---

#### IListDogsByOwnerReader

Lists dogs for a specific owner:

```csharp
public interface IListDogsByOwnerReader
{
    Task<IEnumerable<DogListItemDto>> ListByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken);
}
```

**Returns:** Lightweight DTOs suitable for list views.

---

## EF Core Implementations

### RegisterDogWriter

```csharp
public class RegisterDogWriter : IRegisterDogWriter
{
    private readonly AppDbContext _dbContext;

    public async Task WriteAsync(Dog dog, CancellationToken cancellationToken)
    {
        _dbContext.Dogs.Add(dog);
        // Commit handled by IAppUnitOfWork
    }
}
```

**Flow:**

1. Add aggregate to DbContext  
2. EF Core tracks changes  
3. `IAppUnitOfWork.CommitAsync()` calls `SaveChangesAsync()`  
4. Transaction commits atomically  

---

### GetDogReader

```csharp
public class GetDogReader : IGetDogReader
{
    private readonly AppDbContext _dbContext;

    public async Task<DogDto?> GetDogAsync(DogId dogId, CancellationToken cancellationToken)
    {
        return await _dbContext.Dogs
            .Where(d => d.Id == dogId)
            .Select(d => new DogDto(
                d.Id.Value,
                d.OwnerId.Value,
                d.Name.Value,
                d.Breed.Value,
                d.DateOfBirth,
                d.Sex.ToString()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
```

**Characteristics:**

- Query‑only projection  
- No aggregate instantiation  
- Single SQL query  
- Returns `null` if not found  

---

### ListDogsByOwnerReader

```csharp
public class ListDogsByOwnerReader : IListDogsByOwnerReader
{
    private readonly AppDbContext _dbContext;

    public async Task<IEnumerable<DogListItemDto>> ListByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Dogs
            .Where(d => d.OwnerId == ownerId)
            .Select(d => new DogListItemDto(
                d.Id.Value,
                d.Name.Value,
                d.Breed.Value,
                CalculateAge(d.DateOfBirth)))
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return today.Year - birthDate.Year -
            (today < birthDate.AddYears(today.Year - birthDate.Year) ? 1 : 0);
    }
}
```

---

## Database Context Integration

### AppDbContext Mapping

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Dog> Dogs { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dog>(builder =>
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasConversion(d => d.Value, v => DogId.From(v));

            builder.Property(d => d.OwnerId)
                .HasConversion(u => u.Value, v => UserId.From(v));

            builder.Property(d => d.Name)
                .HasConversion(n => n.Value, v => DogName.Create(v));

            builder.Property(d => d.Breed)
                .HasConversion(b => b.Value, v => Breed.Create(v));

            builder.Property(d => d.DateOfBirth);
            builder.Property(d => d.Sex);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(d => d.OwnerId);
        });
    }
}
```

**Key Points:**

- Strongly typed IDs converted to/from GUIDs  
- Value objects converted to/from primitive types  
- Ownership enforced via foreign key  
- Aggregate mapped cleanly without leaking EF Core concerns into domain  

---

## Service Registration

All persistence services are registered via `AddInfrastructureDogs()`:

```csharp
public static IServiceCollection AddInfrastructureDogs(
    this IServiceCollection services)
{
    return services
        .AddScoped<IEditDogWriter, EditDogWriter>()
        .AddScoped<IRegisterDogWriter, RegisterDogWriter>()
        .AddScoped<IRemoveDogWriter, RemoveDogWriter>()
        .AddScoped<IGetDogByIdReader, GetDogByIdReader>()
        .AddScoped<IGetDogReader, GetDogReader>()
        .AddScoped<IListDogsByOwnerReader, ListDogsByOwnerReader>();
}
```

**Lifetime:** Scoped — all writers/readers share the same DbContext per request.

---

## Transactional Boundaries

All write operations rely on the Unit of Work:

```csharp
await _dogWriter.WriteAsync(dog, ct);
await _unitOfWork.CommitAsync(ct);
```

If any step fails:

- no changes are persisted  
- the transaction is rolled back  
- domain consistency is preserved  

---

## DTOs vs Domain Objects

**Use DTOs for:**

- queries  
- API responses  
- lightweight projections  

**Use domain objects for:**

- commands  
- invariant enforcement  
- aggregate mutation  

This separation keeps the domain pure and queries efficient.

---

## Best Practices

1. **Never expose DbContext to the application layer**  
2. **Use DTOs for read operations**  
3. **Keep writers focused and minimal**  
4. **Test persistence with a real or test database**  
5. **Use EF Core migrations for schema evolution**  

---

## Testing Persistence

```csharp
[Fact]
public async Task WriteAsync_WithValidDog_PersistsToDatabaseAsync()
{
    var dog = Dog.Create(
        UserId.From(Guid.NewGuid()),
        DogName.Create("Buddy"),
        Breed.Create("Labrador"),
        new DateOnly(2020, 1, 15),
        Sex.Male);

    await _writer.WriteAsync(dog, CancellationToken.None);
    await _unitOfWork.CommitAsync(CancellationToken.None);

    var persisted = await _dbContext.Dogs.FindAsync(dog.Id);
    Assert.NotNull(persisted);
    Assert.Equal("Buddy", persisted.Name.Value);
}
```

---

## Source References

- `src/CampFitFurDogs/Application/Abstractions/Dogs/` — persistence contracts  
- `src/CampFitFurDogs/Infrastructure/Dogs/` — EF Core implementations  
- `src/CampFitFurDogs/Infrastructure/DbContexts/AppDbContext.cs` — DbContext  
- `src/CampFitFurDogs/Infrastructure/Migrations/` — schema evolution  

