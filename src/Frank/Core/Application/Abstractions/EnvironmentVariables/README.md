# Environment Variables

The **EnvironmentVariables** folder contains abstractions for accessing environment‑level configuration values in a clean, testable, and centralized way. Environment variables often provide deployment‑specific settings such as connection strings, secrets, feature flags, or infrastructure configuration.

This folder provides the foundational interface used to retrieve environment variables in a controlled and consistent manner.

---

## Purpose

Environment variables model *external configuration*. They are:

- **deployment‑specific** — values differ between environments  
- **externalized** — configuration lives outside the application  
- **immutable at runtime** — values are read, not changed  
- **testable** — abstractions allow mocking and isolation  

By centralizing environment variable access, the application gains:

- predictable configuration behavior  
- improved testability  
- reduced reliance on static system APIs  
- consistent naming and retrieval patterns  

---

## Components

### IEnvironmentVariables
Represents an abstraction for retrieving environment variables.

```csharp
public interface IEnvironmentVariables
{
    string? Get(string key);
}
```

This interface allows implementations to:

- apply naming conventions  
- enforce required/optional variable rules  
- provide defaults or fallbacks  
- validate or normalize values  
- wrap system APIs for testability  

---

## Design Principles

- **Abstraction over system APIs**  
  Avoids direct calls to `Environment.GetEnvironmentVariable`.

- **Centralized configuration access**  
  All environment variable retrieval flows through one interface.

- **Testability**  
  Enables mocking environment values in unit tests.

- **Consistency**  
  Enforces uniform naming, validation, and fallback behavior.

- **Separation of concerns**  
  Keeps configuration logic out of business logic.

---

## How Environment Variables Fit Into the Application

Environment variables typically supply:

- connection strings  
- API keys and secrets  
- feature flags  
- environment identifiers  
- infrastructure configuration  
- external service endpoints  

The `IEnvironmentVariables` abstraction ensures these values are accessed safely and consistently across the application, supporting clean architecture and deployment flexibility.

---
