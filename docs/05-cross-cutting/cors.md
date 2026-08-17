# Frank.CrossCutting — CORS

Cross‑Origin Resource Sharing (CORS) is enforced at the **platform level** to allow approved origins while maintaining strict security guarantees. CORS is a **cross‑cutting concern**: every endpoint inherits the same policy, ensuring consistent behavior across all vertical slices.

This document describes the CORS subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Core.Api
/src/Frank/Core.Infrastructure
```

---

## Configuration

Origins are resolved from strongly typed configuration in `appsettings.json`:

```json
{
  "Frontend": {
    "BaseUrl": "https://app.example.com"
  },
  "Identity": {
    "Oidc": {
      "Authority": "https://auth0.example.com"
    }
  }
}
```

These values are:

- validated  
- normalized into canonical origin strings  
- applied to the platform CORS policy  

See also:  
- [Configuration Management](ca://s?q=Explain_crosscutting_configuration_management)  
- [Security Headers](ca://s?q=Explain_crosscutting_security_headers)

---

## Policy

The platform configures a unified CORS policy that:

- **allows requests from the frontend origin**  
- **allows requests from the identity provider origin**  
- **accepts credentials** (cookies, authorization headers)  
- **allows common HTTP methods** (GET, POST, PUT, DELETE)  
- **sets preflight max‑age** for browser caching  
- **rejects all other origins**  

This ensures:

- the frontend can communicate with the API  
- OIDC callback flows function correctly  
- unauthorized origins are blocked  
- browsers cache preflight responses efficiently  

See: [CORS Policy](ca://s?q=Explain_identity_cors_policy)

---

## How It Works

During startup:

1. **Origins are resolved** from configuration  
2. **Origins are normalized** into canonical form  
3. **The CORS policy is registered** with the DI container  
4. **The policy is applied globally** to all endpoints  
5. **OriginLoggingMiddleware** logs cross‑origin requests for debugging  

This ensures consistent behavior across environments (Development, Testing, Production).

### Origin Normalization

Normalization ensures origins follow the format:

```
scheme://host[:port]
```

Examples:

- `https://app.example.com`  
- `https://auth0.example.com`  
- `http://localhost:3000`  

Normalization prevents misconfiguration and ensures browsers interpret origins correctly.

---

## Runtime Collaboration Points

CORS interacts with:

- **Authentication** — OIDC callback redirects  
- **Identity** — session cookies and authorization headers  
- **Security Middleware** — headers, rate limiting, lockout  
- **Configuration Management** — environment‑specific origins  
- **Testing** — mutated contexts and origin simulation  

CORS is foundational to secure browser‑based access.

---

## Notes

Keep this document grounded in the actual CORS implementation.  
Whenever frontend URLs, identity provider domains, or environment profiles change, update this section to reflect the current architecture.
