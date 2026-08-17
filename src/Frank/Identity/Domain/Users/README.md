# Identity Domain — Users

The **Users** folder contains the core domain model for authenticated owners in the Identity subsystem.  
It defines the `User` aggregate, its strongly‑typed identifiers, and all value objects and exceptions required to enforce identity‑related invariants.

This folder represents the canonical definition of a user inside the system.

---

## Purpose

The Users domain models the identity of an authenticated owner.  
It enforces strict invariants around:

- Personal information (first name, last name, email)
- Contact information (phone number)
- External authentication identity (provider + subject)
- Strongly‑typed aggregate identity (`UserId`)
- Domain‑level validation and error handling

Local identity has been fully de‑featured (US‑184).  
All users must originate from an external identity provider (US‑110).

---

## Folder Structure

### **Aggregate**
- **User**  
  The root entity representing an authenticated owner.  
  Composed exclusively of validated value objects.  
  Requires an `ExternalId` and internally generates a `UserId`.

### **Value Objects**
- **FirstName**  
  Normalized and validated personal first name.
- **LastName**  
  Normalized and validated personal last name.
- **Email**  
  Normalized, lower‑cased, regex‑validated email address.
- **PhoneNumber**  
  User‑entered formats normalized to E.164.
- **ExternalId**  
  External identity provider reference in the form `provider|identifier`.
- **PasswordHash**  
  BCrypt‑validated password hash (plaintext only allowed inside `Create` and `Verify`).
- **UserId**  
  Strongly‑typed aggregate identifier; never `Guid.Empty`.

### **Exceptions**
Domain‑level validation errors thrown when invariants are violated:

- ConflictingIdentitySourcesException  
- EmailAlreadyExistsException  
- InvalidEmailException  
- InvalidExternalAuthProviderIdException  
- InvalidFirstNameException  
- InvalidLastNameException  
- InvalidPasswordHashException  
- InvalidPhoneNumberException  
- InvalidUserIdException  
- MissingIdentitySourceException

---

## Design Principles

The Users domain follows these architectural principles:

- **Strong typing everywhere**  
  No primitive strings for identity data; all values are validated objects.

- **Invariant enforcement at construction time**  
  Invalid data cannot enter the aggregate.

- **Normalization before storage**  
  Emails, names, phone numbers, and external IDs are normalized consistently.

- **External identity required**  
  All users must originate from an external provider; local identity is removed.

- **Aggregate purity**  
  The `User` aggregate contains no behavior unrelated to identity.

- **EF Core compatibility**  
  Parameterless constructors exist only for persistence.

---

## When This Model Is Used

The Users domain is used in:

- Authentication flows (OIDC)
- Registration and onboarding
- Profile management
- Email verification (US‑148)
- Password reset flows (US‑146)
- Welcome email flows (US‑145)
- Observability and audit logging (US‑183)

It provides the canonical representation of an authenticated owner across the entire system.

---

## Summary

The **Users** folder defines the complete identity model for authenticated owners.  
It provides:

- A pure, invariant‑driven `User` aggregate  
- Strongly‑typed value objects  
- Domain‑specific exceptions  
- A consistent normalization and validation strategy  
- Full alignment with Identity subsystem stories (US‑110, US‑184, US‑148, US‑146, etc.)

This folder is the foundation of the Identity domain.

