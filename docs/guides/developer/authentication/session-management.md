# Session Management Guide

This guide explains how session management works **today** based on the implementation completed for **US‑110 (Authentication: Owner Login)**.  
It documents the *runtime behavior* and *developer workflow* for issuing and validating session cookies after a successful OIDC login.

This guide does **not** define rules, boundaries, or architectural requirements — those live in:

- Governance (process + enforcement)  
- Conventions (how we implement)  
- ADRs (why decisions were made)

This guide focuses solely on **how to work with the existing session implementation**.

---

# What Session Management Covers Today

US‑110 implements the **first half** of the session lifecycle:

- Session cookie **creation**  
- Cookie **format**  
- Cookie **security flags**  
- Cookie **issuance** during the Auth Callback  
- Session **persistence** in the database  
- Session **lookup** during authentication flows  
- Session **rotation** (not yet implemented — US‑111)  
- Session **revocation** (not yet implemented — US‑111)

This guide documents only what exists today.

---

# Session Flow Overview

1. User completes OIDC login with Auth0  
2. Auth0 redirects to `/api/auth/callback`  
3. The callback endpoint:
   - Validates the authorization code  
   - Fetches the user profile  
   - Resolves the internal Owner ID  
   - Creates a new session record  
   - Issues a secure session cookie  
4. The browser stores the cookie automatically  
5. Subsequent API requests include the cookie  
6. Middleware (coming in US‑111) will validate the session  

---

# Session Cookie Format

The session cookie is an **opaque, server‑managed token**.  
It contains **no user data** and is not a JWT.

Example (conceptual):

```
Set-Cookie: session=2f9c3e2a...; HttpOnly; Secure; SameSite=Lax; Path=/; Max-Age=86400
```

### Cookie Properties

| Property | Value | Purpose |
|---------|--------|---------|
| Name | `session` | Standardized across API |
| Value | 256‑bit random token | Prevents guessing/bruteforce |
| HttpOnly | `true` | Prevents JS access |
| Secure | `true` in preview/prod | Required for HTTPS |
| SameSite | `Lax` | Allows OIDC redirects |
| Path | `/` | Sent to all API routes |
| Max‑Age | 24 hours | Temporary until US‑111 |

The cookie is issued by the **Auth Callback Endpoint**.

---

# Session Database Record

Sessions are stored in the database using a simple schema:

| Column | Purpose |
|--------|---------|
| `Id` | Primary key |
| `TokenHash` | SHA‑256 hash of the session token |
| `OwnerId` | Internal owner identifier |
| `CreatedAt` | Audit timestamp |
| `ExpiresAt` | Absolute expiration |
| `LastUsedAt` | For idle timeout (future) |

Only the **hash** of the token is stored — never the raw token.

---

# Where Session Creation Happens

Session creation occurs inside the **Auth Callback pipeline**, specifically in the step:

```
CreateSessionCookieStep
```

This step:

1. Receives the resolved `OwnerId`  
2. Creates a new session record  
3. Persists it via the session repository  
4. Issues the session cookie  
5. Sets the callback result to `CreateSuccess`  

This step does **not** validate or rotate sessions — that belongs to US‑111.

---

# Session Validation (Future Work)

Session validation middleware is **not yet implemented**.  
It will be added in **US‑111**.

For now:

- The session cookie is issued  
- The session record is created  
- No middleware consumes it yet  

This guide will be updated once US‑111 is implemented.

---

# Local Development Notes

### Cookies in HTTP vs HTTPS

- Local dev uses `http://localhost:3000` (frontend) and `http://localhost:5000` (API)  
- The cookie is still issued with `Secure=false` in local dev  
- In preview/prod, `Secure=true` is enforced automatically  

### Auth0 Callback URLs

Ensure the following callback URLs are configured:

```
http://localhost:5000/api/auth/callback
https://<preview>.onrender.com/api/auth/callback
https://campfitfurdogsapi.onrender.com/api/auth/callback
```

---

# Testing Session Behavior

Session behavior is tested in three layers:

### 1. Unit Tests  
- Validate session creation logic  
- Validate cookie issuance  
- Validate token hashing  

### 2. Integration Tests  
- Exercise the full callback flow  
- Assert cookie presence  
- Assert session persistence  

### 3. Guardrail Tests  
- Ensure session cookie uses correct flags  
- Ensure no sensitive data leaks into cookies  
- Ensure session token is opaque  

These tests live in:

```
tests/Api.Tests/Authentication
tests/Api.Tests/Guardrails
```

---

# Troubleshooting

### Cookie not appearing in browser  
- Check SameSite=Lax  
- Check domain mismatch  
- Check HTTPS requirement in preview/prod  

### Session not created  
- Ensure Owner ID resolution succeeded  
- Check database connection  
- Check session repository DI registration  

### Callback returning 500  
- Usually caused by missing Auth0 secrets  
- Check `AUTH0_CLIENT_ID`, `AUTH0_CLIENT_SECRET`, `AUTH0_DOMAIN`  

---

# Related Documents

- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Authentication Testing](ca://s?q=Generate_Authentication_Testing_Guide)**  
- **[Authentication Operations](ca://s?q=Generate_Authentication_Operations_Guide)**  
- **[Create Account Form](ca://s?q=Generate_Create_Account_Form_Guide)**  
- **[Create Account Feature Slice](ca://s?q=Generate_Create_Account_Slice_Guide)**  
