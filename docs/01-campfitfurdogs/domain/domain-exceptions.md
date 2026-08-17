# Domain Exceptions

Domain exceptions represent violations of business rules and invariants within the CampFitFurDogs domain. They are thrown exclusively by domain objects and value objects when operations would result in an invalid or inconsistent state. These exceptions form the backbone of domain integrity.

## Exception Hierarchy

All domain exceptions inherit from `DomainException`, which itself inherits from `Exception`:

```
DomainException
├── InvalidDogNameException
├── InvalidBreedException
├── OwnershipViolationException
└── ... (other domain-specific exceptions)
```

This hierarchy ensures consistent handling and mapping across the application and API layers.

---

## Common Domain Exceptions

### InvalidDogNameException

Thrown when dog name validation fails:

```csharp
public sealed class InvalidDogNameException : DomainException
{
    public InvalidDogNameException(string message) : base(message) { }
}
```

**Thrown by:** `DogName.Create(string value)`  
**Conditions:**
- Name is null, empty, or whitespace  
- Name exceeds maximum length constraints  

**HTTP Response:** `400 Bad Request` (via `DomainExceptionHandler`)

---

### InvalidBreedException

Thrown when breed validation fails:

```csharp
public sealed class InvalidBreedException : DomainException
{
    public InvalidBreedException(string message) : base(message) { }
}
```

**Thrown by:** `Breed.Create(string value)`  
**Conditions:**
- Breed is null, empty, or whitespace  
- Breed exceeds maximum length constraints  

**HTTP Response:** `400 Bad Request`

---

### OwnershipViolationException

Thrown when a user attempts to modify a dog they do not own:

```csharp
public sealed class OwnershipViolationException : DomainException
{
    public OwnershipViolationException(UserId ownerId, DogId dogId)
        : base($"User {ownerId} does not own dog {dogId}.")
    {
        OwnerId = ownerId;
        DogId = dogId;
    }

    public UserId OwnerId { get; }
    public DogId DogId { get; }
}
```

**Thrown by:** Domain methods enforcing ownership invariants  
**HTTP Response:** `403 Forbidden` (via `AuthorizationExceptionHandler`)

---

## Exception Handling Flow

1. **Domain layer** throws exception when invariant is violated  
2. **Application layer** allows exception to propagate (no recovery attempted)  
3. **Exception middleware** intercepts the exception  
4. **Exception handler** converts it to a `ProblemDetails` response  
5. **Client receives** structured error with appropriate HTTP status  

This flow ensures domain integrity while providing clear feedback to API consumers.

---

## Best Practices

### 1. Throw Early and Specifically

```csharp
// ✅ Good: Specific domain exception
if (string.IsNullOrWhiteSpace(value))
    throw new InvalidDogNameException("Dog name cannot be empty.");

// ❌ Bad: Generic exception
throw new Exception("Invalid input");
```

### 2. Provide Contextual Information

```csharp
// ✅ Good: Includes context
throw new InvalidDogNameException(
    $"Dog name '{value}' is invalid. Names must be 2–100 characters.");
```

### 3. Never Catch and Ignore Domain Exceptions

```csharp
// ❌ Bad: Suppressing domain errors
try
{
    var name = DogName.Create(input);
}
catch (InvalidDogNameException) { }

// ✅ Good: Let middleware handle it
var name = DogName.Create(input);
```

### 4. Use Specific Exception Types

```csharp
public class InvalidDogNameException : DomainException { }
public class InvalidBreedException : DomainException { }
public class OwnershipViolationException : DomainException { }
```

Specific exceptions improve clarity, observability, and error mapping.

---

## Testing Domain Exceptions

Unit tests verify that domain invariants are enforced correctly:

```csharp
[Fact]
public void DogName_CreateWithEmptyValue_ThrowsInvalidDogNameException()
{
    Assert.Throws<InvalidDogNameException>(() => DogName.Create(string.Empty));
}

[Fact]
public void DogName_CreateWithValidValue_Succeeds()
{
    var name = DogName.Create("Buddy");
    Assert.Equal("Buddy", name.Value);
}
```

---

## Exception Mapping to HTTP

Domain exceptions are mapped to HTTP status codes via API exception handlers:

| Domain Exception            | Handler                    | HTTP Status | Error Code        |
|-----------------------------|----------------------------|-------------|-------------------|
| `DomainException`           | DomainExceptionHandler     | 400         | DomainError       |
| `InvalidDogNameException`   | DomainExceptionHandler     | 400         | DomainError       |
| `InvalidBreedException`     | DomainExceptionHandler     | 400         | DomainError       |
| `OwnershipViolationException` | AuthorizationExceptionHandler | 403     | Forbidden         |
| `ValidationException`       | ValidationExceptionHandler | 400         | ValidationFailed  |

---

## Source References

- `src/CampFitFurDogs/Application/Exceptions/` — domain exception definitions  
- `src/CampFitFurDogs/Api/ExceptionHandlers/` — exception handlers  
- `src/Frank/Core/Domain/DomainException.cs` — base domain exception class  

