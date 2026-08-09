# Identity EntityFrameworkCore — Users

The **Users** folder contains the complete EF Core persistence layer for the
Identity subsystem’s user vertical slices.  
It provides writers, readers, and entity configuration for storing and retrieving
`User` aggregates using `FrankIdentityDbContext`.

This folder forms the infrastructure layer for user creation, lookup, and
identity‑provider linking.

---

## Purpose

The Users subsystem provides:

- Persistence for the `User` aggregate
- Writers for creating users
- Readers for retrieving users by internal or external ID
- EF Core configuration for value objects and constraints
- DI registration for all user components

It ensures that user data is stored consistently, securely, and in alignment with
your domain model.

---

## Files

### **CreateUserWriter**

Persists newly created `User` aggregates.

Responsibilities:

- Adds a new `User` to the EF Core change tracker
- Does **not** call `SaveChangesAsync` (unit of work handles commit)
- Implements `ICreateUserWriter`

Used during:

- Account creation
- External‑provider onboarding
- Identity initialization flows

---

### **GetUserByExternalIdReader**

Retrieves a user by external identity provider ID.

Responsibilities:

- Performs a read‑only lookup using `AsNoTracking`
- Matches `ExternalId.Value` against the provided external ID
- Returns a lightweight `GetUserByExternalIdResponse`
- Implements `IGetUserByExternalIdReader`

Used during:

- OIDC login flows
- External‑provider account linking
- Identity federation

---

### **GetUserByIdReader**

Retrieves a user by internal Identity ID.

Responsibilities:

- Performs a read‑only lookup using `AsNoTracking`
- Matches the `UserId` value object
- Returns basic profile information (`FirstName`, `LastName`)
- Implements `IGetUserByIdReader`

Used during:

- Profile retrieval
- Authorization checks
- Internal identity resolution

---

### **UserConfiguration**

Defines EF Core mapping for the `User` aggregate.

Responsibilities:

- Maps `UserId` using a value converter
- Maps `FirstName`, `LastName`, `Email` as owned value objects
- Maps optional `PhoneNumber` using a nullable converter
- Maps required `ExternalId` as an owned type with a unique index
- Defines column names using snake_case conventions

Used during:

- Database schema generation
- EF Core materialization
- Domain‑driven persistence

---

### **ServiceCollectionExtensions**

Registers all user persistence components.

Responsibilities:

- Adds:
  - `ICreateUserWriter`
  - `IGetUserByExternalIdReader`
  - `IGetUserByIdReader`
- Ensures scoped lifetime to match DbContext
- Provides a single registration point for the Users subsystem

Used during:

- Application startup
- Host configuration

---

## Design Principles

The Users subsystem follows these architectural principles:

- **Vertical slice isolation**  
  Each operation (create, lookup by external ID, lookup by internal ID) has its
  own interface and implementation.

- **Domain purity**  
  Value objects remain intact; EF Core handles persistence concerns.

- **Owned types for value objects**  
  First name, last name, email, and external ID are modeled as owned types.

- **Fail‑fast uniqueness constraints**  
  External IDs must be unique across all users.

- **Scoped lifetime alignment**  
  Writers and readers share the same DbContext lifetime.

---

## User Lifecycle Overview

1. **Create**  
   - Domain constructs a `User`  
   - Writer attaches it to the DbContext  
   - Unit of work commits  

2. **Lookup by External ID**  
   - Reader matches `ExternalId.Value`  
   - Used for OIDC login and account linking  

3. **Lookup by Internal ID**  
   - Reader matches `UserId`  
   - Used for profile retrieval and authorization  

---

## Summary

The **Users** folder provides the complete EF Core infrastructure for managing
Identity users:

- Creation  
- Lookup (internal + external)  
- Value object persistence  
- Configuration  
- Dependency injection  

It is a fully isolated vertical slice aligned with your Identity subsystem’s
architecture and domain‑driven design principles.

