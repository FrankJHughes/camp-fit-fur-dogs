# Session Management Guide (Aligned With Auth Callback Refactor)

This guide explains how session management works **today** based on the implementation completed for **US‑110 (Authentication: Owner Login)** and **US‑111 (Session Management)**.  
It documents the *runtime behavior* and *developer workflow* for issuing and persisting session cookies after a successful OIDC login.

This guide does **not** define rules, boundaries, or architectural requirements — those live in:

- Architecture Governance  
- Security Governance  
- Operations Governance  
- Conventions  
- ADRs  

This guide focuses solely on **how the current session implementation behaves**, aligned with the **new authentication callback architecture**.

---

# What Session Management Covers Today

The current system implements the **first half** of the session lifecycle:

- Session token **generation**  
- Token **hashing**  
- Cookie **value computation** (Application pipeline)  
- Cookie **issuance** (API layer)  
- Session **persistence** in the database  
- Session **association** with an Owner  
- Session **lookup** (internal only — middleware not yet implemented)  

Not yet implemented (future stories):

- Session **validation middleware**  
- Session **rotation**  
- Session **revocation**  
- Idle timeout enforcement  
- Absolute expiration enforcement  

This guide documents only what exists today.

---

# Session Flow Overview (Post‑Refactor)

1. User completes OIDC login with Auth0  
2. Auth0 redirects to `/api/auth/callback?code=XYZ`  
3. API extracts the authorization code  
4. **Frank Auth Callback Pipeline** runs (protocol):  
   - Validates configuration  
   - Exchanges the authorization code  
   - Fetches and normalizes userinfo  
   - Produces a stable external identity payload  

5. **Application Auth Callback Pipeline** runs (business):  
   - Validates identity claims  
   - Resolves or creates the Owner  
   - Logs the login event  
   - **Creates the session**  
   - **Computes the session token + hash**  
   - **Computes the cookie value**  
   - Computes the final redirect URL  

6. API layer:  
   - Issues the session cookie using the `CookieValue`  
   - Returns **302 Redirect** to the frontend  

7. Browser stores the cookie  
8. Future API requests will include the cookie  
9. Session validation middleware (future) will consume it  

All of this behavior follows **Session Management Governance** and **API Endpoint Purity**.

---

# Session Cookie Format

The session cookie is an **opaque, server‑managed token**.  
It contains **no user data**, is **not a JWT**, and is **not readable** by the client.

Example (conceptual):

```
Set-Cookie: cfd.session=2f9c3e2a...; HttpOnly; Secure; SameSite=Lax; Path=/; Max-Age=86400
```

### Cookie Properties

| Property | Value | Purpose |
|---------|--------|---------|
| Name | `cfd.session` | Standardized across API |
| Value | 256‑bit random token | Prevents guessing/bruteforce |
| HttpOnly | `true` | Prevents JS access |
| Secure | `true` in preview/prod | Required for HTTPS |
| SameSite | `Lax` | Allows OIDC redirects |
| Path | `/` | Sent to all API routes |
| Max‑Age | Configured | Temporary until US‑111 |

The cookie value is computed in the **Application pipeline** and written by the **API layer**.

Cookie issuance follows **Security Governance**.

---

# Session Token Generation

Session tokens are generated inside the **Application Auth Callback Pipeline** using a cryptographically secure token generator.

Each session token consists of:

- A **256‑bit random plaintext token**  
- A **SHA‑256 hash** of that token  

The pipeline returns:

```
CookieValue  (plaintext token)
TokenHash    (SHA‑256 hash)
```

The **plaintext token** is sent to the browser as a cookie.  
The **hash** is stored in the database.

The plaintext token is **never persisted**.

This aligns with **Security Governance** and **Session Token Governance**.

---

# Session Database Record

Sessions are stored in the database using the domain `Session` entity.

| Column | Purpose |
|--------|---------|
| `Id` | Primary key |
| `TokenHash` | SHA‑256 hash of the session token |
| `CustomerId` | Internal owner identifier |
| `CreatedAt` | Audit timestamp |
| `ExpiresAt` | Absolute expiration |
| `LastUsedAt` | For idle timeout (future) |

Only the **hash** of the token is stored — never the raw token.

---

# Where Session Creation Happens (Post‑Refactor)

Session creation occurs inside the **Application Auth Callback Pipeline**, not in a dispatcher step.

### Inputs:

- Normalized identity from Frank  
- `CustomerId`  
- `Now`  
- `TokenHash`  

### Behavior:

1. Creates a new domain `Session`  
2. Persists it via `ISessionRepository`  
3. Commits via `IUnitOfWork`  
4. Returns a result containing:
   - `SessionId`
   - `TokenHash`
   - `CookieValue`
   - `RedirectUrl`

The Application pipeline does **not** issue cookies — that is the API layer’s responsibility.

---

# Where Cookie Issuance Happens

Cookie issuance happens in the **API callback endpoint**, not in the pipeline.

The API endpoint:

1. Receives `CookieValue` from the Application pipeline  
2. Applies cookie security flags  
3. Writes the cookie to the HTTP response  
4. Issues the redirect  

This separation enforces:

- **API Endpoint Purity**  
- **Session Management Governance**  
- **Security Governance**  

---

# Session Validation (Future Work)

Session validation middleware is **not yet implemented**.  
It will be added in **US‑111**.

The middleware will:

- Read the `cfd.session` cookie  
- Hash the token  
- Look up the session by hash  
- Validate expiration  
- Attach the authenticated Owner to the request  

Until then:

- Sessions are created  
- Cookies are issued  
- No middleware consumes them  

---

# Local Development Notes

### Cookies in HTTP vs HTTPS

- Local dev uses `http://localhost:3000` (frontend) and `http://localhost:5000` (API)  
- Cookie is issued with `Secure=false` in local dev  
- Preview/prod enforce `Secure=true`  

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

---

## 1. Unit Tests  
- Token generation  
- Token hashing  
- Cookie value computation  
- Session creation  

---

## 2. Application Pipeline Tests  
- Identity resolution → session creation  
- Cookie value computation  
- Redirect computation  

---

## 3. Integration Tests  
- Full callback flow  
- Cookie issuance  
- Session persistence  
- Redirect behavior  

---

## 4. Guardrail Tests  
- Cookie flags  
- No sensitive data in cookies  
- Token opacity  
- No JWTs  
- No Infrastructure leakage into Application  

Tests live in:

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
- Ensure identity resolution succeeded  
- Check database connection  
- Check session repository DI registration  

### Callback returning 500  
- Usually caused by missing Auth0 secrets  
- Check `Authentication__Callback__Oidc__ClientId`  
- Check `Authentication__Callback__Oidc__ClientSecret`  
- Check `Authentication__Callback__Oidc__Authority`  

---

# Related Documents

- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Authentication Testing Guide](ca://s?q=Show_authentication_testing_guide)**  
- **[Authentication Operations Guide](ca://s?q=Show_authentication_operations_guide)**  
- **[Create Account Form Guide](ca://s?q=Show_create_account_form_guide)**  
- **[Create Account Feature Slice Guide](ca://s?q=Show_create_account_feature_slice_guide)**
