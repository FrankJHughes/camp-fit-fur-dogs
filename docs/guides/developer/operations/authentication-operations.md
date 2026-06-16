# Authentication Operations Guide (Aligned With Auth Callback Refactor)

This guide explains how to operate and configure the authentication system implemented in **US‑110 (Authentication: Owner Login)** and **US‑111 (Session Management)**.  
It documents the *operational setup*, *environment configuration*, *Auth0 integration*, *local development workflow*, and *preview/production hosting requirements*.

This guide does **not** define rules, boundaries, or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on **how to operate the authentication system that exists today**, aligned with the **new authentication callback architecture**.

---

# Overview

Authentication operations involve:

- Auth0 tenant configuration  
- Application configuration (`Authentication:Callback:*`)  
- Callback URL configuration  
- Logout URL configuration  
- Web origin + CORS configuration  
- Local development setup  
- Preview environment setup  
- Production environment setup  
- Troubleshooting common issues  

All behavior follows:

- Security Governance  
- Operations Governance  
- Hosting Provider Abstraction Rules  
- Frank Hosting Abstractions  

---

# Auth0 Configuration

Authentication relies on a correctly configured Auth0 application.

## Required Auth0 Settings

| Setting | Value |
|--------|--------|
| Application Type | **Single Page Application** |
| Token Endpoint Auth Method | **None** |
| Allowed Callback URLs | Must include API callback URLs |
| Allowed Logout URLs | Must include frontend URLs |
| Allowed Web Origins | Must include frontend URLs |
| Allowed CORS Origins | Must include frontend URLs |

These settings ensure the OIDC login flow can complete successfully across local, preview, and production environments.

Auth0 must always be configured with **HTTPS** URLs for preview and production.

---

# Callback, Logout, and Web Origin Configuration

These values must stay synchronized with Render preview URLs and production hosting.

## Required Callback URLs

```
http://localhost:5000/api/auth/callback
https://campfitfurdogsapi-pr-<number>.onrender.com/api/auth/callback
https://campfitfurdogsapi.onrender.com/api/auth/callback
```

## Required Logout URLs

```
http://localhost:3000
https://campfitfurdogsapi-pr-<number>.onrender.com
https://campfitfurdogs.com
```

## Required Web Origins

```
http://localhost:3000
https://campfitfurdogsapi-pr-<number>.onrender.com
https://campfitfurdogs.com
```

These must be updated whenever Render preview URLs change.

---

# Application Configuration (Aligned With Refactor)

The system no longer uses `AUTH0_*` environment variables directly.  
All configuration is now provided under:

```
Authentication:Callback:Oidc:*
Authentication:Callback:PostLoginRedirectUrl
```

### Canonical configuration shape:

```json
{
  "Authentication": {
    "Callback": {
      "PostLoginRedirectUrl": "https://yourapp.com/dashboard",
      "Oidc": {
        "Authority": "https://YOUR_DOMAIN",
        "ClientId": "YOUR_CLIENT_ID",
        "ClientSecret": "YOUR_CLIENT_SECRET",
        "CallbackUrl": "https://yourapp.com/auth/callback",
        "Disabled": false
      }
    }
  }
}
```

### Environment variable equivalents:

- `Authentication__Callback__Oidc__Authority`  
- `Authentication__Callback__Oidc__ClientId`  
- `Authentication__Callback__Oidc__ClientSecret`  
- `Authentication__Callback__Oidc__CallbackUrl`  
- `Authentication__Callback__Oidc__Disabled`  
- `Authentication__Callback__PostLoginRedirectUrl`  

These values are consumed **only** by the **Frank Auth Callback Pipeline**.

The Application pipeline consumes only the normalized protocol result and the `PostLoginRedirectUrl`.

---

# The `Oidc:Disabled` Flag

```
Authentication:Callback:Oidc:Disabled = true
```

This flag:

- Completely disables OIDC authentication  
- Causes the Frank pipeline to short‑circuit  
- Prevents token exchange  
- Prevents userinfo calls  
- Prevents identity resolution  
- Prevents session creation  
- Causes the callback endpoint to return a shaped **501 Not Implemented**  

Used for:

- Local offline development  
- CI environments without secrets  
- Automated tests  

---

# Local Development Configuration

Example:

```
Authentication__Callback__Oidc__Authority=https://dev-tenant.us.auth0.com
Authentication__Callback__Oidc__ClientId=abc123
Authentication__Callback__Oidc__ClientSecret=xyz789
Authentication__Callback__Oidc__CallbackUrl=http://localhost:5000/api/auth/callback
Authentication__Callback__PostLoginRedirectUrl=http://localhost:3000/
Authentication__Callback__Oidc__Disabled=false
```

Local development uses **HTTP**, so cookies are issued with:

- `Secure=false`  
- `SameSite=Lax`  
- `HttpOnly=true`  

This is the only environment where insecure cookies are allowed.

---

# Preview Environment Configuration (Render PR Previews)

Preview environments require:

- HTTPS  
- `Secure=true` cookie flag  
- Correct callback URLs  
- Correct logout URLs  
- Correct CORS origins  
- Correct web origins  

### Preview Callback URL

```
https://campfitfurdogsapi-pr-<number>.onrender.com/api/auth/callback
```

### Preview Frontend URL

```
https://campfitfurdogsapi-pr-<number>.onrender.com
```

### Common Preview Issues

#### 1. Cookie not set  
- Missing HTTPS  
- Wrong domain  
- Wrong callback URL  

#### 2. 500 on callback  
- Missing OIDC secrets  
- Wrong redirect URI  

#### 3. Infinite redirect loop  
- Wrong logout redirect URL  

---

# Production Environment Configuration

### Production Callback URL

```
https://campfitfurdogsapi.onrender.com/api/auth/callback
```

### Production Frontend URL

```
https://campfitfurdogs.com
```

### Production Cookie Behavior

- `Secure=true`  
- `SameSite=Lax`  
- `HttpOnly=true`  
- Domain‑scoped cookie  

### Production Troubleshooting

#### 1. Cookie not appearing  
- Check HTTPS  
- Check domain mismatch  
- Check SameSite rules  

#### 2. Callback failing  
- Check Auth0 logs  
- Check redirect URI mismatch  

#### 3. Owner not created  
- Check identity resolver  
- Check database connectivity  

---

# Local Development Workflow

## 1. Start the API

```
dotnet run --project src/CampFitFurDogs.Api
```

## 2. Start the frontend

```
npm run dev
```

## 3. Login Flow

- Frontend calls `/api/auth/login`  
- Browser redirects to Auth0  
- Auth0 redirects back to `/api/auth/callback`  
- Frank pipeline performs protocol logic  
- Application pipeline resolves identity + creates session  
- API issues session cookie  
- Browser stores cookie  
- Frontend redirects to dashboard  

## 4. Cookie Behavior in Local Dev

- `Secure=false`  
- `SameSite=Lax`  
- `HttpOnly=true`  
- Works over HTTP  

---

# Operational Logs

Authentication logs include:

- Callback start/end  
- Token exchange failures  
- Userinfo failures  
- Identity mapping failures  
- Session creation failures  

Logs are emitted through the standard logging pipeline.

---

# Common Operational Failures

## 1. Missing OIDC Secrets  
**Symptoms:**  
- 500 on callback  
- No tokens returned  

**Fix:**  
- Set `Authentication__Callback__Oidc__ClientSecret`  
- Redeploy  

---

## 2. Wrong Callback URL  
**Symptoms:**  
- Auth0 error page  
- “Callback URL mismatch”  

**Fix:**  
- Update Auth0 Allowed Callback URLs  

---

## 3. Cookie Not Set  
**Symptoms:**  
- User appears logged out  
- No session cookie  

**Fix:**  
- Ensure HTTPS  
- Ensure correct domain  
- Ensure `Secure=true` in preview/prod  

---

## 4. Owner Not Created  
**Symptoms:**  
- Callback returns 500  
- Identity mapping fails  

**Fix:**  
- Check `sub` in userinfo  
- Check Owner repository  

---

# Related Documents

- **Session Management Guide**  
- **Identity Mapping Guide**  
- **Authentication Architecture Guide**  
- **Authentication Testing Guide**  
- **Create Account Form Guide**  
- **Create Account Feature Slice Guide**
