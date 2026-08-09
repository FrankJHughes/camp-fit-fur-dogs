# Identity EntityFrameworkCore — DbContexts

The **DbContexts** folder contains all Entity Framework Core database context types and supporting infrastructure required to persist the Identity domain.  
This includes the runtime DbContext, the design‑time factory used by EF Core tooling, and DI registration helpers.

These components form the bridge between the pure Identity domain model and the underlying PostgreSQL database.

---

## Purpose

This folder provides:

- A fully configured EF Core DbContext for the Identity subsystem  
- A design‑time factory for migrations and CLI tooling  
- A DI extension method for consistent registration across hosting environments  

All persistence configuration is centralized here to keep the Application and Domain layers clean and infrastructure‑free.

---

## Files

### **FrankIdentityDbContext**
The primary EF Core DbContext for the Identity subsystem.

Responsibilities:

- Materializes Identity aggregates and value objects
- Applies all `IEntityTypeConfiguration<>` classes from the assembly
- Serves as the root EF Core context for migrations and runtime operations

Key behaviors:

- Uses `ApplyConfigurationsFromAssembly` to automatically load mapping classes
- Contains no domain logic — only persistence configuration

---

### **FrankIdentityDesignTimeDbContextFactory**
A design‑time factory required by EF Core tooling.

Used by:

- `dotnet ef migrations add`
- `dotnet ef database update`
- CI/CD pipelines (GitHub Actions)

Responsibilities:

- Loads configuration from:
  - `appsettings.json`
  - `appsettings.Development.json`
  - Environment variables (required for CI)
- Constructs a `FrankIdentityDbContext` with the correct PostgreSQL connection string

This ensures migrations can be generated even when the application’s normal startup path is unavailable.

---

### **ServiceCollectionExtensions**
Dependency injection registration for the Identity DbContext.

Responsibilities:

- Adds `FrankIdentityDbContext` to the DI container
- Configures EF Core to use PostgreSQL
- Resolves the `DefaultConnection` connection string from `IConfiguration`

This extension is intended to be called from application startup (`Program.cs`), ensuring consistent configuration across all environments.

---

## Design Principles

The DbContexts folder follows these architectural principles:

- **Separation of concerns**  
  Domain logic stays in the Domain layer; persistence logic stays here.

- **Convention‑based configuration**  
  All EF Core mappings are discovered automatically via assembly scanning.

- **Environment‑aware configuration**  
  Design‑time factory loads settings from multiple sources, including CI/CD.

- **Single registration point**  
  DI extension ensures consistent DbContext setup across all hosts.

- **PostgreSQL‑first design**  
  All configuration assumes Npgsql as the provider.

---

## Summary

The **DbContexts** folder provides the complete EF Core infrastructure for the Identity subsystem:

- A runtime DbContext  
- A design‑time factory for migrations  
- A DI extension for consistent registration  

Together, these components ensure that the Identity domain is persisted reliably, consistently, and cleanly across all environments.

