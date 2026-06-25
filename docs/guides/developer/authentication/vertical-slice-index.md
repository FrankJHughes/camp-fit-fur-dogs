# Camp Fit Fur Dogs — Guides — Developer — Architecture — Vertical Slice Index  
**Aligned With Exclusive OIDC Authentication, De‑featured Local Identity, and Actual Implemented Slices**

This index lists all **actual vertical slices** implemented in Camp Fit Fur Dogs.

Vertical slices follow the rule:

> **One verb, one noun. Must represent a domain use case. Must contain domain logic.  
> Not a feature area. Not a horizontal.**

Horizontals (cross‑cutting concerns) are **not** slices and are not listed here.

---

# Customer Slices

```
src/CampFitFurDogs.Application/Customers
    CreateCustomer
```

| Slice | Description | Guide |
|-------|-------------|--------|
| **CreateCustomer** | Owner onboarding — creates the customer aggregate | [Create Customer Slice Guide](ca://s?q=Show_create_customer_slice_guide) |

---

# Dog Slices

```
src/CampFitFurDogs.Application/Dogs
    EditDogProfile
    GetDogProfile
    ListDogsByOwner
    RegisterDog
    RemoveDog
```

| Slice | Description | Guide |
|-------|-------------|--------|
| **RegisterDog** | Register a dog under the authenticated owner | [Register Dog Slice Guide](ca://s?q=Show_register_dog_slice_guide) |
| **EditDogProfile** | Edit a dog’s profile | [Edit Dog Profile Slice Guide](ca://s?q=Show_edit_dog_profile_slice_guide) |
| **GetDogProfile** | Query a dog’s profile | [Dog Profile Slice Guide](ca://s?q=Show_dog_profile_slice_guide) |
| **ListDogsByOwner** | List all dogs for the authenticated owner | [List Dogs Slice Guide](ca://s?q=Show_list_dogs_slice_guide) |
| **RemoveDog** | Remove a dog | [Remove Dog Slice Guide](ca://s?q=Show_remove_dog_slice_guide) |

---

# Session (Domain Aggregate — No Slices Yet)

The `Session` aggregate exists, but **no vertical slices exist yet**.

Future slices (not implemented):

- **CreateSession**  
- **ValidateSession**  
- **RevokeSession**  
- **RotateSession**  

These will be added when US‑111+ stories are implemented.

---

# Horizontals (Cross‑Cutting Concerns — Not Slices)

These are **not** vertical slices and are intentionally excluded:

- Authentication (OIDC login + callback)  
- Session cookie issuance  
- SecurityHeaders  
- CORS  
- ErrorBoundary  
- ProblemDetails  
- Observability  
- Logging  
- DI registration  
- Hosting configuration  
- RateLimiting  
- AccountLockout  
- API throttling (future)  

Horizontals are governed by **Infrastructure Governance**, not slice conventions.

---

# Adding a New Slice

See: **[Adding a New Feature Slice Guide](ca://s?q=Show_add_new_feature_slice_guide)**

A new slice must:

- Represent a **single domain use case**  
- Contain **domain logic**  
- Follow **Architecture Purity Rules**  
- Live under the correct domain folder  
- Include command, handler, validator, and tests  
- Integrate with the dispatcher pipeline  
- Emit observability events at use‑case boundaries  

Slices must never:

- Contain infrastructure logic  
- Contain API logic  
- Contain cross‑cutting concerns  
- Represent multiple capabilities  
