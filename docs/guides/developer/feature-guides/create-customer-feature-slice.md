# Create Customer Slice Guide  
**Aligned With Exclusive OIDC Authentication & De‑featured Local Identity**

This guide documents the **Create Customer** vertical slice — the domain‑level creation of an Owner record.

This slice was formerly known as **Create Account**, but after **US‑184 (De‑feature Local Identity)**:

- There is **no local registration form**  
- There is **no email/password flow**  
- There is **no `/api/account` endpoint**  
- Owner creation is triggered by the **OIDC callback**  
- The slice’s API surface is **`/api/auth/callback`**  
- The slice’s frontend surface is the **post‑callback redirect**  

This guide explains how the slice works today.

---

# Slice Overview

The Create Customer slice is responsible for:

- Mapping external Auth0 identity → internal Owner identity  
- Creating the Owner aggregate (first login only)  
- Persisting the Owner  
- Creating a session  
- Returning the redirect URL  

This slice follows the vertical slice pattern:

```
Auth0 → /api/auth/callback → Frank Pipeline → Application Pipeline → Domain → Persistence → Redirect
```

---

# Trigger

The slice is triggered when Auth0 redirects the user to:

```
GET /api/auth/callback?code=XYZ
```

The callback endpoint:

- Extracts the authorization code  
- Invokes the **Frank Auth Callback Pipeline**  
- Invokes the **Application Auth Callback Pipeline**  
- Issues the session cookie  
- Redirects the user  

The **Application pipeline** contains the Create Customer slice.

---

# Application Layer Behavior (The Slice Itself)

The Create Customer slice lives inside the **Application Auth Callback Pipeline**.

### Responsibilities

1. Validate required identity claims (`sub`, email, etc.)  
2. Map external identity → internal identity  
3. Check if an Owner already exists  
4. If not, **create a new Owner aggregate**  
5. Persist the Owner  
6. Create a session  
7. Compute the cookie value  
8. Compute the redirect URL  

### Files (conceptual)

````text
src/CampFitFurDogs.Application/Authentication/Callback/
    ApplicationAuthCallbackPipeline.cs
    IdentityMapping.cs
    CreateCustomerBehavior.cs
    SessionCreationBehavior.cs
````

### Domain Inputs

- External identity (normalized by Frank pipeline)  
- `sub` (stable external identifier)  
- Email (optional but common)  

### Domain Outputs

- `CustomerId`  
- `SessionId`  
- `RedirectUrl`  
- `CookieValue`  

---

# Domain Layer Behavior

The domain layer owns the **Owner aggregate**.

### Responsibilities

- Enforce invariants  
- Ensure valid Owner creation  
- Persist the Owner  
- Emit domain events (future)  

### Files

````text
src/CampFitFurDogs.Domain/Customers/
    Owner.cs
    OwnerId.cs
````

### Invariants

- Owner must have a stable external identity key  
- Owner must have a valid email (if provided)  
- Owner cannot be duplicated  

---

# Infrastructure Layer Behavior

Infrastructure persists:

- Owner  
- Session  

### Files

````text
src/CampFitFurDogs.Infrastructure/Customers/
    OwnerRepository.cs
    OwnerConfiguration.cs

src/CampFitFurDogs.Infrastructure/Sessions/
    SessionRepository.cs
    SessionConfiguration.cs
````

### Rules

- Use `_db.Set<T>()`  
- No domain logic  
- No identity logic  
- No protocol logic  

---

# API Layer Behavior

The API layer does **not** implement the slice logic.

It only:

- Extracts the authorization code  
- Invokes the pipelines  
- Issues the cookie  
- Redirects the user  

### File

````text
src/CampFitFurDogs.Api/Authentication/AuthCallbackEndpoint.cs
````

### Rules

- No domain logic  
- No identity logic  
- No session logic  
- No redirect logic  
- No persistence  

All of that lives in the pipelines.

---

# Frontend Behavior

There is **no Create Customer form**.

Auth0 handles:

- Registration  
- Login  
- MFA  
- Password resets  
- Email verification  

The frontend only receives the **post‑callback redirect**.

---

# Success Flow

1. User logs in via Auth0  
2. Auth0 redirects to `/api/auth/callback`  
3. Frank pipeline normalizes identity  
4. Application pipeline:
   - Creates Owner (if first login)  
   - Creates session  
   - Computes redirect  
5. API issues cookie  
6. Browser redirects to app  

---

# Error Handling

Handled by:

- Frank error boundary  
- Application pipeline validation  
- API boundary  

Typical errors:

- Missing OIDC configuration  
- Invalid authorization code  
- Missing `sub` claim  
- Owner creation failure  
- Session creation failure  

---

# Testing the Slice

### 1. Application Pipeline Tests  
- Identity mapping  
- Owner creation  
- Session creation  
- Redirect computation  

### 2. Domain Tests  
- Owner invariants  
- Owner creation rules  

### 3. Integration Tests  
- Full callback flow  
- Owner persistence  
- Session persistence  
- Cookie issuance  

---

# Troubleshooting

### Owner not created  
- Check identity mapping  
- Check `sub` claim  
- Check database connection  

### Callback failing  
- Check OIDC configuration  
- Check Auth0 application settings  

### Cookie not issued  
- Check SameSite + Secure flags  
- Check callback URL correctness  

---

# Related Documents

- **[Authentication Overview](ca://s?q=Show_authentication_overview)**  
- **[Callback Endpoint](ca://s?q=Show_callback_endpoint_doc)**  
- **[Application Callback Pipeline](ca://s?q=Show_application_callback_pipeline)**  
- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Session Management Guide](ca://s?q=Show_session_management_guide)**  
