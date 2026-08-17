# Frank.CrossCutting — Rate Limiting

Rate limiting protects the API from abuse, ensures fair resource distribution, and prevents brute‑force attacks against authentication endpoints. It is a **cross‑cutting concern** applied uniformly across all vertical slices.

This document describes the rate limiting subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Core.Api
/src/Frank/Core.Infrastructure/Security
```

Rate limiting integrates with:

- authentication protection (US‑110, US‑111, US‑133)  
- security middleware (US‑132)  
- observability (US‑183)  
- configuration management  
- reverse proxies and gateways  

---

## When to Use Rate Limiting

Rate limiting is applied to endpoints with higher risk or higher cost:

- **Public endpoints** — high abuse risk  
- **Authentication endpoints** — brute‑force protection  
- **Resource‑intensive operations** — expensive queries or writes  
- **Bulk operations** — user‑initiated multi‑resource actions  

Examples:

- `/api/identity/login`  
- `/api/identity/callback`  
- `/api/dogs/bulk`  
- `/api/dogs` (POST)  

See also:  
- [Security](ca://s?q=Explain_crosscutting_security)  
- [Authentication](ca://s?q=Explain_crosscutting_authentication)

---

## Implementation Options

Rate limiting can be applied at multiple levels depending on the scenario.

### Per‑User Rate Limiting

Limits requests per authenticated user:

- **X requests per minute**  
- **Y requests per hour**  
- **Z concurrent requests**  

Useful for:

- authenticated API usage  
- preventing user‑initiated abuse  
- protecting expensive operations  

See: [User‑Scoped Limits](ca://s?q=Explain_user_scoped_rate_limits)

---

### Per‑IP Rate Limiting

Limits requests per IP address, protecting against:

- DDoS attacks  
- bot traffic  
- unauthenticated brute‑force attempts  

Useful for:

- public endpoints  
- login endpoints  
- anonymous browsing  

See: [IP‑Scoped Limits](ca://s?q=Explain_ip_rate_limiting)

---

### Per‑Endpoint Rate Limiting

Different endpoints require different limits:

- `POST /api/dogs` — limited (creates resources)  
- `GET /api/dogs` — higher limit (read‑only)  
- `POST /api/dogs/bulk` — very limited (expensive operation)  

This ensures resource‑intensive operations are protected without penalizing lightweight reads.

See: [Endpoint‑Scoped Limits](ca://s?q=Explain_endpoint_rate_limiting)

---

## Response Behavior

When a rate limit is exceeded:

- **Status code:** `429 Too Many Requests`  
- **Headers:** `Retry-After` indicating when the client may retry  
- **Body:** clear error message describing the limit and wait time  

Example:

```json
{
  "error": "Rate limit exceeded",
  "retryAfterSeconds": 30
}
```

This ensures clients can respond gracefully and retry appropriately.

See: [Error Handling](ca://s?q=Generate_crosscutting_error_handling_doc)

---

## Implementation Notes

Rate limiting can be implemented using:

- **ASP.NET Core built‑in rate limiting middleware**  
- **third‑party libraries** (e.g., AspNetCoreRateLimit)  
- **reverse proxies** (nginx, Envoy, Cloudflare)  
- **API gateways** (AWS API Gateway, Kong, Azure APIM)  

### Platform Recommendation

The platform typically uses:

- **ASP.NET Core rate limiting middleware** for per‑endpoint and per‑user limits  
- **reverse proxy rate limiting** for IP‑based protection  
- **observability integration** for logging rate‑limit events  

See:  
- [Security Middleware](ca://s?q=Explain_identity_security_middleware_tests)  
- [Observability](ca://s?q=Generate_crosscutting_observability_doc)

---

## Runtime Collaboration Points

Rate limiting interacts with:

- **Authentication** — protects login endpoints  
- **Authorization** — prevents abuse of protected resources  
- **Security Headers** — unified security posture  
- **Logging** — structured rate‑limit events  
- **Observability** — correlation IDs and diagnostics  
- **Testing** — mutated contexts for simulating rate‑limit scenarios  

Rate limiting is a foundational cross‑cutting capability that ensures platform stability and fairness.

---

## Notes

Keep this document grounded in the actual rate limiting implementation.  
Whenever new endpoints, security requirements, or infrastructure layers are added, update this section to reflect the current architecture.
