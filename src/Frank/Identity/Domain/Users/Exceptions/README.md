# Users Domain — Exceptions

The **Exceptions** folder contains all domain‑level errors related to user identity, validation, and invariant enforcement within the **Identity** subsystem.  
These exceptions ensure that invalid or contradictory user data cannot enter or remain inside the domain model.

Each exception represents a **business rule violation**, not an infrastructure or application error.

---

## Purpose

User identity is one of the most constrained areas of the domain.  
These exceptions enforce invariants around:

- Identity source consistency  
- Email uniqueness and formatting  
- Name formatting  
- Phone number normalization  
- Password hashing rules  
- User identifier validity  
- External authentication provider identifiers  

By centralizing these exceptions, the domain model remains expressive, predictable, and self‑validating.

---

## Exception List

Below is a summary of all exceptions contained in this folder.

### 1. **ConflictingIdentitySourcesException**  
Thrown when multiple identity sources provide contradictory or incompatible data for the same user.

### 2. **EmailAlreadyExistsException**  
Thrown when attempting to create or register a user with an email already present in the system.

### 3. **InvalidEmailException**  
Thrown when an email fails domain validation (empty, malformed, incorrect format).

### 4. **InvalidExternalAuthProviderIdException**  
Thrown when an external authentication provider supplies an invalid or malformed provider ID.

### 5. **InvalidFirstNameException**  
Thrown when a first name fails validation (empty, whitespace, too short, too long, invalid characters).

### 6. **InvalidLastNameException**  
Thrown when a last name fails validation under the same rules as first names.

### 7. **InvalidPasswordHashException**  
Thrown when a password hash is empty, malformed, or violates domain hashing rules.

### 8. **InvalidPhoneNumberException**  
Thrown when a phone number fails validation or normalization requirements.

### 9. **InvalidUserIdException**  
Thrown when a user identifier is empty, malformed, or violates domain invariants.

### 10. **MissingIdentitySourceException**  
Thrown when a required identity source is missing during user creation or resolution.

---

## Design Principles

All exceptions in this folder follow these principles:

- **Invariant enforcement**  
  Each exception protects a rule that must always hold true.

- **Domain‑specific messaging**  
  Error messages describe *why* the data violates domain rules, not technical details.

- **No infrastructure leakage**  
  These exceptions do not reference persistence, HTTP, or external systems.

- **Consistency with Value Objects and Aggregates**  
  Exceptions are thrown exclusively from domain constructors, factories, and invariants.

---

## When These Exceptions Are Used

These exceptions appear in:

- `User` aggregate construction  
- `UserId`, `Email`, `PhoneNumber`, and other value objects  
- Identity resolution flows (OIDC, external providers, merging identities)  
- Registration and onboarding flows  
- Password reset and authentication flows  

They ensure that invalid user data is rejected **before** it can corrupt the domain model.

---

## Summary

The **Users/Exceptions** folder provides a complete set of domain‑level validation errors for the Identity subsystem.  
Together, these exceptions enforce:

- Identity consistency  
- Data correctness  
- Security constraints  
- Domain invariants  

This keeps the user model robust, predictable, and safe from malformed or contradictory identity data.

---
