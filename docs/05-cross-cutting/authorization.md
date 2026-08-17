# Frank.CrossCutting — Authorization

Authorization determines what actions an authenticated user can perform. It is a **cross‑cutting concern** applied uniformly across all vertical slices. While authentication establishes *who* the user is, authorization determines *what* they are allowed to do.

This document describes the authorization subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Identity
/src/Frank/Core.Api
/src/Frank/Core.Infrastructure
```

Authorization is composed of:

- identity resolution  
- ownership checks  
- role‑based access rules  
- endpoint‑level authorization decisions  
- consistent error semantics  

---

## Authorization Pattern

Authorization decisions are made directly in endpoints using the authenticated identity and domain‑specific rules.

```csharp
if (currentUser?.IsAuthenticated != true)
    return Results.Unauthorized();

// Verify ownership: can only edit your own dogs
var dogOwner = await repository.GetDogOwnerAsync(dogId);
if (dogOwner != currentUser.Id)
    return Results.Forbid();

// Proceed with business logic
```

This pattern ensures:

- **authentication** is validated first  
- **ownership** is enforced consistently  
- **domain rules** remain in the product layer  
- **authorization** is explicit and readable  

See also:  
- [Authentication](ca://s?q=Explain_crosscutting_authentication)  
- [Current User](ca://s?q=Explain_identity_context_access)

---

## Ownership and Access Control

The product layer enforces domain‑specific access rules. These rules vary by vertical slice but follow consistent platform‑wide patterns.

### Common rules

- Users can only view/edit **their own dogs**  
- Admin users (if present) have elevated privileges  
- Public endpoints do not require authentication  
- Protected endpoints require a valid session  
- Ownership checks must be explicit and enforced at the endpoint boundary  

Ownership is a **domain rule**, not an identity rule. Identity only provides the authenticated user; the product layer decides what that user can do.

See: [Domain Authorization](ca://s?q=Explain_domain_authorization_rules)

---

## Forbidden vs. Not Found

Correct error semantics prevent information leakage and ensure consistent behavior across the platform.

### Best practice

- Return **403 Forbidden** when the user lacks permission  
- Return **404 Not Found** when the resource does not exist *or* the user cannot access it  

This prevents attackers from discovering whether a resource exists.

Example:

```csharp
var dog = await repository.GetDogAsync(dogId);
if (dog is null)
    return Results.NotFound();

if (dog.OwnerId != currentUser.Id)
    return Results.Forbid();
```

This pattern is used across all vertical slices.

See:  
- [Error Handling](ca://s?q=Explain_crosscutting_error_handling)  
- [Security](ca://s?q=Explain_crosscutting_security)

---

## Role‑Based Authorization (Optional)

If the platform introduces roles (e.g., Admin, Staff), authorization expands to include:

- role checks  
- elevated privileges  
- administrative endpoints  
- multi‑tenant access rules  

Role‑based authorization is implemented in the API layer and enforced in endpoints.

See: [Role Authorization](ca://s?q=Explain_role_based_authorization)

---

## Runtime Collaboration Points

Authorization interacts with:

- **Authentication** — session validation and identity resolution  
- **Identity Infrastructure** — current user context  
- **Product Layer** — domain‑specific access rules  
- **Cross‑Cutting Middleware** — consistent error semantics  
- **Testing** — mutated contexts and fake identities  

Authorization is a cross‑cutting concern that shapes endpoint behavior across the entire platform.

---

## Notes

Keep this document grounded in the actual authorization implementation.  
Whenever ownership rules, role models, or endpoint patterns evolve, update this section to reflect the current architecture.
