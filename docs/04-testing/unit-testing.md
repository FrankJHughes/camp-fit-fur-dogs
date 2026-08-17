# Frank.Testing — Unit Testing

Unit tests verify individual components in isolation, without external dependencies, infrastructure, or the full HTTP pipeline. They ensure domain invariants, application logic, and small units of behavior remain correct, deterministic, and easy to reason about.

This document describes the unit testing subsystem under:

```
/docs/04-testing
```

and maps it back to its implementation under:

```
/src/Frank/Testing
```

---

## Test Organization

Unit tests mirror the structure of the source code:

```
src/CampFitFurDogs/
  Domain/Dogs/
  Application/Dogs/
  Api/Endpoints/Dogs/
  Infrastructure/Persistence/

tests/CampFitFurDogs/
  Domain/Dogs/
  Application/Dogs/
  Api/Endpoints/Dogs/
  Infrastructure/Persistence/
```

This parallel structure ensures:

- tests stay close to the code they validate  
- domain tests remain pure  
- application tests isolate handlers and orchestrators  
- API tests validate endpoint logic without full pipeline execution  
- infrastructure tests validate persistence and configuration behavior  

---

## Test Structure

Unit tests follow the **AAA pattern** (Arrange, Act, Assert):

```csharp
[Fact]
public void Should_prevent_empty_dog_name()
{
    // Arrange
    var emptyName = string.Empty;

    // Act
    var result = DogName.Create(emptyName);

    // Assert
    result.Should().BeOfType<Result<DogName>.Failure>();
}
```

AAA keeps tests readable, predictable, and easy to maintain.

---

## Domain Tests

Domain tests validate invariants and rules in complete isolation.

```csharp
[Theory]
[InlineData(null)]
[InlineData("")]
[InlineData("   ")]
public void Should_reject_invalid_names(string invalidName)
{
    var result = DogName.Create(invalidName);
    result.Should().BeOfType<Result<DogName>.Failure>();
}
```

Domain tests also validate aggregate creation:

```csharp
[Fact]
public void Should_create_dog_with_valid_data()
{
    var userId = new UserId(Guid.NewGuid());
    var name = DogName.Create("Buddy").GetOrThrow();
    var breed = Breed.Create("Labrador").GetOrThrow();
    var dateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-5));
    var sex = Sex.Male;

    var dog = Dog.Create(userId, name, breed, dateOfBirth, sex);

    dog.OwnerId.Should().Be(userId);
    dog.Name.Should().Be(name);
}
```

Domain tests must never use EF Core, HTTP clients, or external services.

---

## Handler Tests

Handler tests validate command/query handlers with mocked dependencies.

```csharp
[Fact]
public async Task Should_register_dog_for_authenticated_user()
{
    // Arrange
    var userId = new UserId(Guid.NewGuid());
    var command = new RegisterDogCommand(
        userId,
        "Buddy",
        "Labrador",
        DateOnly.FromDateTime(DateTime.Now.AddYears(-5)),
        Sex.Male);

    var writerMock = new Mock<IRegisterDogWriter>();
    var unitOfWorkMock = new Mock<IAppUnitOfWork>();

    var handler = new RegisterDogCommandHandler(writerMock.Object, unitOfWorkMock.Object);

    // Act
    var result = await handler.HandleAsync(command, CancellationToken.None);

    // Assert
    result.Should().BeOfType<CommandResult<Guid>.Success>();
    writerMock.Verify(w => w.AddAsync(It.IsAny<Dog>(), It.IsAny<CancellationToken>()));
    unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()));
}
```

Handler tests isolate application logic without invoking the full pipeline.

---

## Using Factories

Factories simplify object creation and reduce boilerplate:

```csharp
var dog = DogFactory
    .WithName("Buddy")
    .WithBreed("Labrador")
    .WithDateOfBirth(new DateOnly(2019, 1, 15))
    .Build();

var command = RegisterDogCommandFactory
    .WithOwnerId(userId)
    .WithName("Buddy")
    .Build();
```

Factories ensure:

- domain invariants are respected  
- test data remains realistic  
- tests stay readable  

---

## Assertions

Use FluentAssertions for expressive, readable tests:

```csharp
result.Should()
    .BeOfType<CommandResult<Guid>.Success>()
    .Which.Value.Should().NotBeEmpty();

dog.Name.Value.Should().Be("Buddy");

dogs.Should()
    .NotBeEmpty()
    .And.AllSatisfy(d => d.OwnerId.Should().Be(userId));
```

FluentAssertions improves clarity and reduces boilerplate.

---

## Running Tests

```bash
dotnet test
dotnet test --filter "Category=Unit"
dotnet test --configuration Release
```

Unit tests should run quickly and deterministically.

---

## Notes

Keep unit tests pure, isolated, and free from infrastructure concerns.  
Whenever domain rules or application logic evolve, update unit tests to reflect new invariants and behaviors.
