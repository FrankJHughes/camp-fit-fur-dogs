# Authentication Operations Guide

This guide explains how to operate and configure the authentication system implemented in **US‑110 (Authentication: Owner Login)**.  
It documents the *operational setup*, *environment configuration*, *Auth0 integration*, *local development workflow*, and *preview/production hosting requirements*.

This guide does **not** define rules, boundaries, or architectural decisions — those live in governance, conventions, and ADRs.  
This guide focuses solely on **how to operate the authentication system that exists today**.

---

# Overview

Authentication operations involve:

- Auth0 tenant configuration  
- Environment variable setup  
- Callback URL configuration  
- Logout URL configuration  
- Web origin + CORS configuration  
- Local development setup  
- Preview environment setup  
- Production environment setup  
- Troubleshooting common issues  

Each environment has different requirements based on hosting provider behavior, cookie security rules, and redirect constraints.

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

# Environment Variables

Authentication requires the following environment variables:

| Variable | Purpose |
|----------|----------|
| `AUTH0_DOMAIN` | Auth0 tenant domain |
| `AUTH0_CLIENT_ID` | Auth0 application client ID |
| `AUTH0_CLIENT_SECRET` | Auth0 application client secret |
| `AUTH0_AUDIENCE` | API audience (optional) |
| `AUTH0_REDIRECT_URI` | Callback URL for the API |
| `AUTH0_LOGOUT_REDIRECT_URI` | Logout redirect URL |

These values are consumed through **Frank hosting abstractions**, not `Environment.GetEnvironmentVariable`.

---

# Local Development Values

```
AUTH0_DOMAIN=dev-xxxxx.us.auth0.com
AUTH0_CLIENT_ID=xxxxxxxxxxxxxxxxxxxx
AUTH0_CLIENT_SECRET=xxxxxxxxxxxxxxxxxxxx
AUTH0_REDIRECT_URI=http://localhost:5000/api/auth/callback
AUTH0_LOGOUT_REDIRECT_URI=http://localhost:3000
```

Local development uses **HTTP**, so cookies are issued with:

- `Secure=false`  
- `SameSite=Lax`  
- `HttpOnly=true`  

This is the only environment where insecure cookies are allowed.

---

# Preview Environment Values

```
AUTH0_REDIRECT_URI=https://campfitfurdogsapi-pr-<number>.onrender.com/api/auth/callback
AUTH0_LOGOUT_REDIRECT_URI=https://campfitfurdogsapi-pr-<number>.onrender.com
```

Preview environments require:

- HTTPS  
- `Secure=true` cookie flag  
- Correct callback URLs  
- Correct logout URLs  
- Correct CORS origins  
- Correct web origins  

---

# Production Values

```
AUTH0_REDIRECT_URI=https://campfitfurdogsapi.onrender.com/api/auth/callback
AUTH0_LOGOUT_REDIRECT_URI=https://campfitfurdogs.com
```

Production requires:

- HTTPS  
- `Secure=true`  
- Domain-scoped cookies  
- Correct callback + logout URLs  

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
- Callback pipeline resolves identity + creates session  
- API issues session cookie  
- Browser stores cookie  
- Frontend redirects to dashboard  

## 4. Cookie Behavior in Local Dev

- `Secure=false`  
- `SameSite=Lax`  
- `HttpOnly=true`  
- Works over HTTP  

This matches **Security Governance** for local development.

---

# Preview Environment Workflow

Preview environments run on Render.

## Requirements

- HTTPS enforced  
- `Secure=true` cookie flag  
- Correct callback URLs  
- Correct logout URLs  
- Correct CORS origins  
- Correct web origins  

## Preview Callback URL

```
https://campfitfurdogsapi-pr-<number>.onrender.com/api/auth/callback
```

## Preview Frontend URL

```
https://campfitfurdogsapi-pr-<number>.onrender.com
```

## Common Preview Issues

### 1. Cookie not set  
- Missing HTTPS  
- Wrong domain  
- Wrong callback URL  

### 2. 500 on callback  
- Missing Auth0 secrets  
- Wrong redirect URI  

### 3. Infinite redirect loop  
- Wrong logout redirect URL  

---

# Production Environment Workflow

Production uses:

- Render for API  
- Vercel or static hosting for frontend  
- Auth0 for identity  

## Production Callback URL

```
https://campfitfurdogsapi.onrender.com/api/auth/callback
```

## Production Frontend URL

```
https://campfitfurdogs.com
```

## Production Cookie Behavior

- `Secure=true`  
- `SameSite=Lax`  
- `HttpOnly=true`  
- Domain-scoped cookie  

## Production Troubleshooting

### 1. Cookie not appearing  
- Check HTTPS  
- Check domain mismatch  
- Check SameSite rules  

### 2. Callback failing  
- Check Auth0 logs  
- Check redirect URI mismatch  

### 3. Owner not created  
- Check identity resolver  
- Check database connectivity  

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

## 1. Missing Auth0 Secrets  
**Symptoms:**  
- 500 on callback  
- No tokens returned  

**Fix:**  
- Set `AUTH0_CLIENT_SECRET`  
- Redeploy  

## 2. Wrong Callback URL  
**Symptoms:**  
- Auth0 error page  
- “Callback URL mismatch”  

**Fix:**  
- Update Auth0 Allowed Callback URLs  

## 3. Cookie Not Set  
**Symptoms:**  
- User appears logged out  
- No session cookie  

**Fix:**  
- Ensure HTTPS  
- Ensure correct domain  
- Ensure `Secure=true` in preview/prod  

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
