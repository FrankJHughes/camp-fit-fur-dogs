# Application Exceptions

Application‑layer exceptions represent errors that occur during request handling, validation, or coordination across system layers. They wrap domain exceptions when necessary and provide application‑specific context for the API’s exception‑handling pipeline.

## Exception Categories

### 1. Validation Exceptions

Thrown when FluentValidation rules fail:

```csharp
throw new ValidationException([
    new ValidationFailure(nameof(RegisterDogCommand.Name), "Name is required")
]);
```

**Handler:** `ValidationExceptionHandler`  
**HTTP Status:** 400 Bad Request  
**Response:** Field‑level validation errors

Validation exceptions are raised *before* the handler executes, ensuring handlers receive structurally valid input.

---

### 2. Domain Exceptions

Domain exceptions propagate directly from the domain layer without modification:

```csharp
var name = DogName.Create(command.Name);   // Throws DomainException if invalid
var breed = Breed.Create(command.Breed);
```

**Handler:** `DomainExceptionHandler`  
**HTTP Status:** 400 Bad Request  
**Response:** Domain error message

Domain exceptions represent invariant violations and should not be caught unless the handler can meaningfully recover.

---

### 3. Resource Not Found

Thrown when a required resource does not exist:

```csharp
public sealed class DogNotFoundException : ApplicationException
{
    public DogNotFoundException(DogId dogId)
        : base($"Dog {dogId} was not found.")
    {
        DogId = dogId;
    }

    public DogId DogId { get; }
}
```

**Handler:** `NotFoundExceptionHandler`  
**HTTP Status:** 404 Not Found  

Resource‑not‑found exceptions are part of normal application flow and should be thrown when a handler cannot locate a required aggregate or DTO.

---

## Exception vs. Result Pattern

The application layer uses **exceptions for exceptional conditions**, not for normal business outcomes.

```csharp
// ✅ Good: Exception for truly exceptional condition
public async Task<Guid> HandleAsync(RegisterDogCommand command, CancellationToken ct)
{
    var name = DogName.Create(command.Name);  // Throws if invalid
    var dog = Dog.Create(ownerId, name, breed, dob, sex);

    await _dogWriter.WriteAsync(dog, ct);
    await _unitOfWork.CommitAsync(ct);

    return dog.Id.Value;
}
```

Avoid using “Result” types for validation or domain invariants:

```csharp
// ❌ Bad: Using Result pattern for validation
public async Task<Result<Guid>> HandleAsync(RegisterDogCommand command, CancellationToken ct)
{
    if (string.IsNullOrEmpty(command.Name))
        return Result.Failure("Name required");
}
```

FluentValidation ensures handlers receive valid input, so handlers should rely on domain exceptions for invariant enforcement.

---

## Exception Handling Pipeline

1. **Request arrives** at endpoint  
2. **FluentValidation runs** (throws `ValidationException` if invalid)  
3. **Handler executes** (may throw domain or application exceptions)  
4. **Exception middleware intercepts**  
5. **Exception handler** converts exception to `ProblemDetails`  
6. **Response returned** to client  

This pipeline ensures consistent error semantics across vertical slices.

---

## Best Practices

### 1. Don’t Catch Exceptions You Can’t Handle

```csharp
// ❌ Bad: Catching without meaningful recovery
try
{
    var dog = Dog.Create(ownerId, name, breed, dob, sex);
}
catch (DomainException)
{
    throw;
}

// ✅ Good: Let domain exception propagate
var dog = Dog.Create(ownerId, name, breed, dob, sex);
```

### 2. Log Contextual Information

```csharp
public async Task<Guid> HandleAsync(RegisterDogCommand command, CancellationToken ct)
{
    try
    {
        _logger.Information("Registering dog for user {UserId}", command.OwnerId);
        // ... handle command
    }
    catch (Exception ex)
    {
        _logger.Error(ex, "Failed to register dog for user {UserId}", command.OwnerId);
        throw;
    }
}
```

### 3. Add Observability Data to Exceptions

```csharp
public sealed class DogNotFoundException : ApplicationException
{
    public DogNotFoundException(DogId dogId)
        : base($"Dog {dogId} was not found.")
    {
        DogId = dogId;
        Data["dog_id"] = dogId.Value; // Observability metadata
    }

    public DogId DogId { get; }
}
```

---

## Creating Custom Application Exceptions

Follow this pattern for new exception types:

```csharp
public sealed class MyApplicationException : ApplicationException
{
    public MyApplicationException(string message) : base(message) { }

    public MyApplicationException(string message, Exception inner)
        : base(message, inner) { }
}
```

Then create a handler:

```csharp
[ExceptionHandler(404)]
public sealed class MyApplicationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => ex is MyApplicationException;

    public IErrorCode GetErrorCode(Exception ex) => ErrorCode.NotFound;

    public ProblemDetails CreateProblemDetails(Exception ex) => new()
    {
        Title = "Not Found",
        Detail = ex.Message,
        Status = StatusCodes.Status404NotFound,
        Type = "https://httpstatuses.com/404"
    };
}
```

---

## Testing Exception Handling

```csharp
[Fact]
public async Task RegisterDogCommandHandler_WithInvalidName_ThrowsDomainException()
{
    // Arrange
    var command = new RegisterDogCommand(
        _userId,
        "",  // Invalid
        "Labrador",
        new DateOnly(2020, 1, 1),
        "Male");

    // Act & Assert
    var ex = await Assert.ThrowsAsync<InvalidDogNameException>(
        () => _handler.HandleAsync(command, CancellationToken.None));

    Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
}
```

---

## Source References

- `src/CampFitFurDogs/Application/Exceptions/` — application exception definitions  
- `src/CampFitFurDogs/Api/ExceptionHandlers/` — exception handlers  
- `src/Frank/Core/Application/Abstractions/Exceptions/` — platform exception abstractions  
