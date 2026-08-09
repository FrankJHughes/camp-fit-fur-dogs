# Identity — EntityFrameworkCore Layer

The **EntityFrameworkCore** folder contains the complete persistence
infrastructure for the Identity subsystem.  
It provides the DbContext, unit of work, entity configurations, readers, writers,
and DI registration required to store and retrieve Identity domain aggregates
using Entity Framework Core.

This layer is intentionally isolated from the Domain and Application layers,
ensuring clean separation of concerns and strict adherence to vertical‑slice
architecture.

---

## Purpose

The EntityFrameworkCore layer provides:

- A dedicated Identity DbContext
- EF Core configurations for all aggregates
- Readers and writers for vertical slices
- A unit of work implementation
- Centralized DI registration
- Assembly scanning support

It ensures that Identity persistence is consistent, testable, and aligned with
your domain model.

---

## Folder Structure

### **DbContexts/**
Contains the EF Core DbContext for the Identity subsystem.

- Defines DbSets for aggregates
- Applies configurations from this assembly
- Binds settings from configuration

### **UnitOfWork/**
Contains the EF Core–backed unit of work implementation.

- Coordinates transactional boundaries
- Ensures atomic commits across vertical slices
- Provides DI registration

### **Sessions/**
Contains persistence logic for the `Session` aggregate.

Includes:

- Session configuration  
- Session writers (create, revoke)  
- Session readers (lookup by token hash, lookup by ID)  
- DI registration

### **Users/**
Contains persistence logic for the `User` aggregate.

Includes:

- User configuration  
- Writers for creating users  
- Readers for lookup by ID and external ID  
- DI registration

### **AssemblyMarker.cs**
A marker class used for assembly scanning.

- Enables configuration discovery  
- Supports DI scanning  
- Used by migration tooling

### **ServiceCollectionExtensions.cs**
Root registration entry point for all EF Core Identity services.

Registers:

- DbContext  
- Unit of Work  
- Session persistence  
- User persistence  

---

## Design Principles

The EntityFrameworkCore layer follows these architectural principles:

- **Vertical slice isolation**  
  Each slice (Users, Sessions) has its own readers, writers, and configuration.

- **Domain purity**  
  Value objects remain untouched; EF Core handles persistence concerns.

- **Owned types for value objects**  
  First name, last name, email, external ID, and token hash are modeled as owned
  types or value converters.

- **Scoped lifetime alignment**  
  All persistence services share the same DbContext lifetime.

- **Assembly scanning**  
  Configurations are applied via `ApplyConfigurationsFromAssembly`.

- **Fail‑fast validation**  
  Misconfiguration is detected at startup.

---

## How This Layer Is Used

1. **Startup registration**  
   The application calls  
   `services.AddFrankIdentityEntityFrameworkCore(configuration)`.

2. **DbContext initialization**  
   The Identity DbContext is created per request.

3. **Vertical slice execution**  
   - Writers attach aggregates  
   - Readers query aggregates  
   - Unit of work commits changes

4. **EF Core configuration**  
   Value objects are mapped via owned types or converters.

---

## Summary

The **EntityFrameworkCore** folder provides the complete persistence backbone for
the Identity subsystem:

- DbContext  
- Unit of Work  
- Sessions vertical slice  
- Users vertical slice  
- Assembly scanning  
- DI registration  

It ensures that Identity persistence is clean, consistent, and aligned with your
domain‑driven architecture.

