# Application Auth Callback Pipeline Guide

The Application Auth Callback Pipeline processes a normalized external identity and produces a fully authenticated internal identity. It performs all business‑level authentication work:

- Identity mapping  
- Owner resolution  
- Owner creation  
- Session creation  
- Redirect computation  
- Cookie value computation  

This pipeline contains **no protocol logic** and **no HTTP concerns**. It is the business core of the authentication flow.

---

# Purpose

The pipeline transforms a normalized external identity (from the Frank pipeline) into:

- An internal `CustomerId`
- A persisted Owner record (if first login)
- A persisted session
- A computed redirect URL
- A cookie value for the API to issue

It ensures that identity mapping, Owner creation, and session creation follow consistent business rules.

---

# Inputs and Outputs

## Input

A normalized external identity containing:

- `sub` (stable external identifier)
- Email (optional)
- Name fields (optional)
- Provider metadata

This identity is already:

- Retrieved via OIDC
- Normalized by the Frank pipeline
- Free of protocol‑level concerns

## Output

A result object containing:

- `CustomerId`
- `SessionId`
- `RedirectUrl`
- `CookieValue` (opaque session token)
- Any additional metadata needed by the API boundary

---

# Responsibilities

The pipeline performs the following steps:

## 1. Validate external identity
- Ensure required claims (such as `sub`) are present  
- Enforce minimal business rules on identity data  

## 2. Identity mapping
- Map external identity → internal identity model  
- Determine the stable lookup key for the Owner  

## 3. Owner resolution
- Look up an existing Owner by mapped identity  
- If found, load the Owner  
- If not found, proceed to Owner creation  

## 4. Owner creation (Create Customer slice)
- Create a new Owner aggregate  
- Enforce domain invariants  
- Persist the Owner  
- Produce a new `CustomerId`  

## 5. Session creation
- Create a new session for the Owner  
- Generate a session token  
- Hash and store the token  
- Persist the session  
- Produce `SessionId` and raw token  

## 6. Redirect computation
- Determine the post‑login redirect URL  
- Apply any business rules for first‑time vs returning users  

## 7. Cookie value computation
- Convert the raw session token into an opaque cookie value  
- Ensure it is suitable for secure cookie issuance by the API  

The pipeline returns a single cohesive result object.

---

# Architecture and Structure

The pipeline is implemented using the **ImmutableContextBuilder** pattern.

## Conceptual structure

```
Application/Authentication/Callback/
    ApplicationAuthCallbackPipeline.cs
    IdentityMappingBehavior.cs
    OwnerResolutionBehavior.cs
    CreateCustomerBehavior.cs
    SessionCreationBehavior.cs
    RedirectComputationBehavior.cs
    CookieComputationBehavior.cs
```

Each behavior:

- Accepts an immutable context  
- Produces a new context with additional data  
- Contains no side effects outside its responsibility  
- Is composable and testable in isolation  

The pipeline composes these behaviors into a single flow.

---

# Separation of Concerns

The Application pipeline is strictly separated from:

## Frank pipeline
- No HTTP  
- No token exchange  
- No userinfo retrieval  
- No protocol error handling  

## API endpoint
- No cookie issuance  
- No redirect execution  
- No HTTP response construction  

## Frontend
- No UI logic  
- No routing logic  

The pipeline focuses solely on business decisions:

- Who is this user?  
- Do they already exist?  
- Should a new Owner be created?  
- What session should be created?  
- Where should they be redirected?  

---

# Identity Mapping

Identity mapping converts the normalized external identity into the internal identity model.

## Responsibilities

- Determine the stable identity key  
- Map external → internal identity  
- Handle optional fields  
- Provide consistent lookup rules  

## Typical rules

- Use `sub` as the primary stable identifier  
- Use email as a secondary attribute
