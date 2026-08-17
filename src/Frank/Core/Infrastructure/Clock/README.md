# Clock

The **Clock** folder contains the infrastructure‑level implementation of the
application’s time abstraction. This subsystem provides the production clock
used throughout the system and the DI registration required to expose it to
vertical slices and application services.

Time is treated as an injectable dependency so that slices can remain
deterministic, testable, and decoupled from system time.

---

## Components

### SystemClock

`SystemClock` is the default infrastructure implementation of `IClock`.  
It provides the current UTC timestamp using `DateTimeOffset.UtcNow`.

#### Responsibilities

- Supplies the authoritative UTC time for the application.
- Implements the `IClock` abstraction from the Application layer.
- Serves as the production clock; test environments may override it.

This keeps time retrieval consistent and centralized across the system.

---

### ServiceCollectionExtensions

Provides DI registration for the infrastructure clock:

```csharp
services.AddScoped<IClock, SystemClock>();
```

#### Responsibilities

- Registers `SystemClock` as the scoped `IClock` implementation.
- Allows vertical slices and application services to depend on `IClock`
  instead of directly accessing system time.
- Supports testability by enabling replacement with deterministic clocks.

---

## Design Principles

- **Abstraction-first**  
  Time is accessed through `IClock`, not `DateTimeOffset.UtcNow`.

- **Deterministic testing**  
  Tests can replace the clock with fixed or simulated implementations.

- **Separation of concerns**  
  The Application layer defines the contract; Infrastructure provides the
  production implementation.

- **Simplicity**  
  The infrastructure clock is intentionally minimal and free of logic.

---

## How This Folder Fits Into the Architecture

The Clock subsystem supports:

- request timestamping  
- domain event creation  
- audit logging  
- scheduling and timeout logic  
- observability pipelines  

Any component that needs the current time depends on `IClock`, ensuring
consistent behavior across slices and environments.

---

## Typical Usage

```csharp
public sealed class OwnerCreatedEventHandler
{
    private readonly IClock _clock;

    public OwnerCreatedEventHandler(IClock clock)
    {
        _clock = clock;
    }

    public Task HandleAsync(OwnerCreatedEvent evt, CancellationToken ct)
    {
        var timestamp = _clock.UtcNow;
        // ...
    }
}
```

---

## Notes

- This folder contains **only** the production clock and its registration.
- Additional clock implementations (e.g., `FixedClock`, `OffsetClock`,
  `TestClock`) should live in test or specialized infrastructure modules.
- All time usage should flow through `IClock` for consistency and testability.

---
