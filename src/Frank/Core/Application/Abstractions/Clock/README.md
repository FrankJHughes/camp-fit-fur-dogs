# Clock

The **Clock** folder contains abstractions for accessing time in a deterministic, testable, and environment‑agnostic way. Instead of calling `DateTimeOffset.UtcNow` directly, application code depends on the `IClock` abstraction, which allows time to be controlled, simulated, or replaced during testing.

This pattern ensures that time‑dependent logic behaves consistently across environments and can be validated without relying on the system clock.

---

## Why a Clock Abstraction?

Time is one of the most common sources of nondeterminism in application code. Directly accessing the system clock makes:

- unit tests brittle  
- integration tests unpredictable  
- simulations impossible  
- time‑based logic harder to reason about  

By introducing a clock abstraction, the application gains:

- **Deterministic tests** — fixed or virtual clocks  
- **Predictable behavior** — no hidden dependencies on system time  
- **Clear intent** — time access is explicit, not implicit  
- **Extensibility** — custom clocks for replay, fast‑forward, or freeze scenarios  

---

## Components

### IClock
The core abstraction representing a source of UTC time.

```csharp
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
```

Implementations may include:

- **SystemClock** — wraps the real system time  
- **FrozenClock** — always returns a fixed instant  
- **AdjustableClock** — allows manual advancement or rewinding  
- **TestClock** — controlled entirely by the test harness  

---

## Usage

Inject `IClock` wherever time is needed:

```csharp
public class TokenService
{
    private readonly IClock _clock;

    public TokenService(IClock clock)
    {
        _clock = clock;
    }

    public Token IssueToken()
    {
        return new Token
        {
            IssuedAt = _clock.UtcNow,
            ExpiresAt = _clock.UtcNow.AddMinutes(30)
        };
    }
}
```

In tests:

```csharp
var clock = new FrozenClock(new DateTimeOffset(2026, 8, 4, 12, 0, 0, TimeSpan.Zero));
var service = new TokenService(clock);

// Deterministic behavior
Assert.Equal(clock.UtcNow.AddMinutes(30), service.IssueToken().ExpiresAt);
```

---

## Design Principles

- **Minimal surface area** — only UTC time is exposed  
- **Testability first** — every time‑dependent component becomes deterministic  
- **Environment independence** — no reliance on system clock or timezone  
- **Single responsibility** — the clock abstraction does one thing well  
