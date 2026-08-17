# Frank.Core.Infrastructure — Clock

The `Clock` service in the infrastructure layer provides a simple, testable abstraction over system time. It allows domain and application code to depend on a stable, injectable time source rather than directly calling `DateTime.UtcNow` or `DateTime.Today`, which makes time‑dependent logic deterministic and fully testable.

This document maps the Clock subsystem under:

```
docs/02-frank-core/infrastructure
```

back to its implementation in:

```
src/Frank/Core/Infrastructure/Time
```

---

## Purpose

The Clock subsystem exists to:

- provide a consistent abstraction for current time  
- eliminate direct dependencies on `DateTime.UtcNow`  
- enable deterministic, time‑based testing  
- support manual time advancement in test scenarios  
- keep domain and application logic free from system‑time concerns  

Time becomes an injectable dependency rather than a global static.

---

## Service Interface

```csharp
public interface IClock
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
```

Key characteristics:

- **UtcNow** — the canonical source of current time  
- **Today** — derived from the clock’s current time  
- no setters, ensuring immutability from the consumer’s perspective  

---

## Production Implementation

```csharp
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
```

The production clock simply delegates to system time.

---

## Test Implementation

```csharp
public sealed class TestClock : IClock
{
    private DateTime _utcNow = DateTime.UtcNow;
    public DateTime UtcNow => _utcNow;
    public DateOnly Today => DateOnly.FromDateTime(_utcNow);
    public void Advance(TimeSpan duration) => _utcNow = _utcNow.Add(duration);
}
```

TestClock allows:

- manual time advancement  
- deterministic time‑dependent behavior  
- simulation of expiration, delays, and scheduled events  

This is essential for reliable domain and application tests.

---

## Usage in Handlers

Inject the clock into handlers:

```csharp
public sealed class RegisterDogCommandHandler : ICommandHandler<RegisterDogCommand, Guid>
{
    private readonly IClock _clock;
    public RegisterDogCommandHandler(IClock clock) => _clock = clock;

    public async Task<Result<Guid>> HandleAsync(RegisterDogCommand command, CancellationToken ct)
    {
        var dog = Dog.Create(
            command.OwnerId,
            command.Name,
            command.Breed,
            command.DateOfBirth,
            command.Sex,
            _clock.UtcNow);  // Use injected clock

        // ...
    }
}
```

This ensures domain creation timestamps are testable and consistent.

---

## Testing Time‑Dependent Logic

```csharp
[Fact]
public async Task Should_expire_old_registrations()
{
    var clock = new TestClock();
    var handler = new RegisterDogCommandHandler(clock);

    // Register dog
    var result = await handler.HandleAsync(command, CancellationToken.None);

    // Advance time
    clock.Advance(TimeSpan.FromDays(365));

    // Verify expiration logic
    // ...
}
```

Benefits:

- no mocking `DateTime.Now`  
- deterministic tests  
- full control over time progression  
- predictable expiration and scheduling behavior  

---

## How the Clock Connects to the Broader Platform

The Clock service collaborates with:

- **Frank.Core.Application**  
  Handlers use the clock for timestamps, scheduling, and expiration logic.

- **Frank.Core.Domain**  
  Aggregates receive timestamps from the clock rather than system time.

- **Frank.Core.Infrastructure**  
  TestClock lives in infrastructure and supports test harnesses.

- **Frank.Core.EntityFrameworkCore**  
  Persistence stores timestamps generated via the clock.

This ensures time flows consistently through the vertical slice.

---

## Runtime Collaboration Points

Clock interacts with runtime by:

- providing consistent timestamps for domain events  
- enabling time‑based validation and expiration  
- supporting scheduled or delayed logic  
- ensuring logs and observability correlate with domain timestamps  

---

## Composition Flow (API → Application → Domain → Persistence)

```
API Endpoint
    ↓
Application Handler (uses IClock)
    ↓
Domain Aggregate (receives timestamp)
    ↓
Unit of Work Commit
    ↓
Database
```

Time becomes part of the slice’s deterministic behavior.

---

## Notes

Keep this document grounded in the actual Frank.Core.Infrastructure Clock implementation.  
Whenever time abstractions, scheduling logic, or test harness patterns evolve, update this section to reflect the current platform architecture.
