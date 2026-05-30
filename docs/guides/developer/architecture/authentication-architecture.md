# Authentication Architecture Guide

This guide explains the **authentication architecture** as implemented in **US‑110 (Authentication: Owner Login)**.  
It documents how the system performs OIDC login, identity resolution, and session issuance using the dispatcher pipeline and the current backend architecture.

This guide does **not** define rules, boundaries, or decisions — those live in:

- Governance (process + enforcement)  
- Conventions (how we implement)  
- ADRs (why decisions were made)

This guide focuses solely on **how the authentication architecture works today**.

---

# High‑Level Architecture

Authentication in the system is composed of three cooperating components:

1. **OIDC Login Initiation**  
2. **Auth Callback Pipeline**  
3. **Session Issuance + Identity Mapping**

These components work together to produce a secure, server‑managed session cookie.

---

# Component 1 — OIDC Login Initiation

The login initiation endpoint:

- Generates the Auth0 authorization URL  
- Includes the correct redirect URI  
- Includes PKCE parameters  
- Redirects the browser to Auth0  

This endpoint is intentionally simple — it delegates all identity proof to Auth0.

---

# Component 2 — Auth Callback Pipeline

The callback endpoint is implemented using the **dispatcher pipeline**, which breaks the authentication flow into small, composable steps.

The pipeline looks like this:

1. **ExchangeAuthorizationCodeStep**  
2. **FetchUserProfileStep**  
3. **ResolveIdentityStep**  
4. **CreateSessionCookieStep**  
5. **ReturnCallbackResultStep**

Each step:

- Performs one job  
- Has no side effects outside its boundary  
- Receives a shared context object  
- Writes results back into the context  
- Passes control to the next step  

This keeps the authentication flow deterministic, testable, and composable.

---

# Pipeline Step Responsibilities

### 1. ExchangeAuthorizationCodeStep  
Exchanges the authorization code for:

- ID token  
- Access token  
- Token metadata  

### 2. FetchUserProfileStep  
Uses the access token to fetch the Auth0 user profile.

### 3. ResolveIdentityStep  
Maps the external identity (`sub`) to an internal `OwnerId`.

### 4. CreateSessionCookieStep  
Creates a new session record and issues the session cookie.

### 5. ReturnCallbackResultStep  
Returns the final redirect response to the frontend.

---

# Identity Mapping in the Architecture

Identity mapping is a core architectural boundary:

- External identity → internal domain identity  
- Implemented via the identity resolver  
- Pure, deterministic, and side‑effect‑free except for Owner creation  
- Never uses email for identity  
- Never exposes internal IDs to Auth0  

Identity mapping is documented in detail in the **Identity Mapping Guide**.

---

# Session Issuance in the Architecture

Session issuance is handled by the session service:

- Generates a 256‑bit random token  
- Hashes the token using SHA‑256  
- Stores the hash in the database  
- Issues the raw token as a secure cookie  
- Associates the session with the resolved `OwnerId`  

Session validation middleware will be added in **US‑111**.

---

# Architectural Boundaries

Authentication touches several architectural layers:

### **1. API Layer**  
- Defines endpoints  
- Orchestrates the dispatcher pipeline  
- Issues cookies  
- Returns redirect responses  

### **2. Application Layer**  
- Contains the pipeline steps  
- Contains identity resolution logic  
- Contains session creation logic  

### **3. Domain Layer**  
- Owns the Owner aggregate  
- Owns the session model  
- Owns domain invariants  

### **4. Infrastructure Layer**  
- Implements Auth0 client  
- Implements session repository  
- Implements Owner repository  

Each layer has strict purity and dependency rules (defined in conventions, not here).

---

# Data Flow Diagram (Textual)

````  
Browser → /api/auth/login  
    → Redirect to Auth0  
Auth0 → /api/auth/callback  
    → Exchange code  
    → Fetch profile  
    → Resolve identity  
    → Create session  
    → Issue cookie  
    → Redirect to frontend  
````

---

# Error Handling Architecture

Authentication errors fall into three categories:

### **1. External Errors**  
- Auth0 unreachable  
- Invalid authorization code  
- Missing tokens  

### **2. Identity Errors**  
- Missing `sub`  
- Invalid external ID  
- Owner creation failure  

### **3. Session Errors**  
- Database failure  
- Cookie issuance failure  

All errors are surfaced as:

- Logged exceptions  
- 500 responses  
- No cookies issued  

---

# Testing Architecture

Authentication is tested at three levels:

### **1. Unit Tests**  
- Pipeline step logic  
- Identity resolver  
- Session creation  

### **2. Integration Tests**  
- Full callback flow  
- Cookie issuance  
- Owner creation  

### **3. Guardrail Tests**  
- Cookie flags  
- No sensitive data in cookies  
- Identity mapping purity  

---

# Local Development Architecture Notes

### Auth0  
Local dev uses:

````  
http://localhost:5000/api/auth/callback
````

### Cookies  
Local dev uses:

- `Secure=false`  
- `SameSite=Lax`  

Preview/prod enforce `Secure=true`.

---

# Related Documents

- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
- **[Authentication Testing](ca://s?q=Generate_Authentication_Testing_Guide)**  
- **[Authentication Operations](ca://s?q=Generate_Authentication_Operations_Guide)**  
- **[Create Account Form](ca://s?q=Generate_Create_Account_Form_Guide)**  
- **[Create Account Feature Slice](ca://s?q=Generate_Create_Account_Slice_Guide)**  
