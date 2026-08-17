# EnvironmentVariables

The **EnvironmentVariables** folder contains the infrastructure‑level
implementation of the application’s environment variable abstraction.  
This subsystem provides the production mechanism for retrieving environment
variables and the DI registration required to expose it to vertical slices and
application services.

Environment variables are treated as an injectable dependency to ensure
testability, configurability, and separation of concerns.

---

## Components

### SystemEnvironmentVariables

`SystemEnvironmentVariables` is the default infrastructure implementation of
`IEnvironmentVariables`.  
It retrieves environment variable values directly from the host operating
system using `System.Environment.GetEnvironmentVariable`.

#### Responsibilities

- Provides access to OS‑level environment variables.
- Implements the `IEnvironmentVariables` abstraction from the Application layer.
- Serves as the production provider; test environments may override it.

This keeps configuration retrieval centralized and consistent across the system.

---

### ServiceCollectionExtensions

Provides DI registration for the infrastructure environment variable provider:

```csharp
services.AddScoped<IEnvironmentVariables, SystemEnvironmentVariables>();
```

#### Responsibilities

- Registers `SystemEnvironmentVariables` as the scoped `IEnvironmentVariables`
  implementation.
- Allows vertical slices and application services to depend on
  `IEnvironmentVariables` instead of directly accessing `System.Environment`.
- Supports testability by enabling replacement with deterministic or in‑memory
  implementations.

---

## Design Principles

- **Abstraction-first**  
  Code retrieves environment variables through `IEnvironmentVariables`, not
  `System.Environment`.

- **Deterministic testing**  
  Tests can replace the provider with fixed or simulated implementations.

- **Separation of concerns**  
  The Application layer defines the contract; Infrastructure provides the
  production implementation.

- **Minimalism**  
  The infrastructure provider is intentionally simple and free of logic.

---

## How This Folder Fits Into the Architecture

The EnvironmentVariables subsystem supports:

- configuration loading  
- feature toggles  
- secrets management (when stored in environment variables)  
- infrastructure bootstrapping  
- hosting and deployment configuration  

Any component that needs environment variables depends on `IEnvironmentVariables`,
ensuring consistent behavior across slices and environments.

---

## Typical Usage

```csharp
public sealed class StartupConfigurationLoader
{
    private readonly IEnvironmentVariables _env;

    public StartupConfigurationLoader(IEnvironmentVariables env)
    {
        _env = env;
    }

    public string? GetDatabaseConnectionString()
    {
        return _env.Get("DB_CONNECTION_STRING");
    }
}
```

---

## Notes

- This folder contains **only** the production provider and its registration.
- The abstraction (`IEnvironmentVariables`) lives in
  `Application/Abstractions/EnvironmentVariables`.
- Additional providers (e.g., in‑memory, encrypted, test‑only) should live in
  test or specialized infrastructure modules.
- All environment variable access should flow through `IEnvironmentVariables`
  for consistency and testability.

---
