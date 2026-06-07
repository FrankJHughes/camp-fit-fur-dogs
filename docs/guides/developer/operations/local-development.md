# Running the API Locally vs Hosted (US‑140)

This guide explains how the API behaves in **local development** versus when **hosted on Render**, including configuration sources, runtime differences, and operational expectations.  
It reflects the current hosting architecture, Frank hosting abstractions, and all updated governance rules.

---

# Local Development

The API can be run locally using:

```
dotnet run --project src/CampFitFurDogs.Api
```

Local configuration is loaded from:

- `appsettings.json`
- `appsettings.Development.json`
- User secrets (optional)
- Frank hosting abstractions (never direct environment access)

## Local Environment Characteristics

- Runs on **HTTP** (no TLS)  
- Uses **local PostgreSQL** or a Neon preview branch  
- Cookies use:  
  - `Secure = false`  
  - `SameSite = Lax`  
  - `HttpOnly = true`  
- Hot reload and fast iteration  
- No cold starts  
- CORS allows:  
  ```
  http://localhost:3000
  ```
- Auth0 callback URL uses:  
  ```
  http://localhost:5000/api/auth/callback
  ```
- Environment variables may be set via user secrets or shell  

Local development mirrors production **code paths**, but not production **hosting constraints**.

All behavior follows:

- API Endpoint Purity  
- Security Governance  
- Hosting Abstraction Rules  

---

# Hosted Environment (Render)

The hosted API runs inside a **Dockerized .NET 10 container** built from:

```
src/CampFitFurDogs.Api/Dockerfile
```

## Hosted Environment Characteristics

- Environment variables injected by Render  
- Neon PostgreSQL as the backing database  
- HTTPS termination handled by Render  
- Health check endpoint exposed at `/health`  
- Session cookies use:  
  - `Secure = true`  
  - `SameSite = Lax`  
  - `HttpOnly = true`  
- CORS allows the deployed frontend host (US‑139)  
- Frank hosting abstractions resolve configuration (no direct environment access)

## Behavioral Differences

- **Cold starts** occur on Render’s free tier after ~15 minutes of inactivity  
  (20–60 seconds typical)
- **CORS** is restricted to the deployed frontend host  
- **Connection strings** are never stored in config files  
  (only environment variables)
- **Preview environments** use ephemeral Neon branches created by CI  
- **Production** uses a persistent Neon branch  
- **Secure cookies** are enforced automatically due to HTTPS  
- **Redirect URIs** must match Auth0 configuration exactly  

Hosted environments enforce the same **code paths** and **EF Core configuration** as local development, but with stricter security and hosting constraints.

---

# Summary

Local and hosted environments share:

- The same API code  
- The same EF Core configuration  
- The same authentication + session pipeline  
- The same routing and middleware  
- The same identity mapping + session creation behavior  

They differ in:

- TLS requirements  
- Cookie security flags  
- CORS configuration  
- Database provisioning  
- Cold start behavior  
- Environment variable injection  
- Auth0 callback + logout URL requirements  

This ensures local development remains fast and flexible while hosted environments remain secure, predictable, and production‑ready.
