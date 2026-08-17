# ADR 009 — Identity (OIDC)

## Status
Accepted

## Context

The application requires secure, standards‑based authentication and identity management.  
Implementing custom authentication is risky, costly, and difficult to maintain.  
OpenID Connect (OIDC) provides a modern, interoperable identity layer built on OAuth 2.0,  
and external providers such as Auth0 offer mature, secure, and scalable identity services.

Camp Fit Fur Dogs must support:

- Owner login  
- Session management  
- Email verification  
- Password reset  
- Account lockout  
- Multi‑environment identity configuration (dev, test, production)  

A decision record is needed to document why OIDC was chosen and how it integrates  
with the vertical slice architecture.

## Decision

Adopt OpenID Connect (OIDC) as the authentication and identity protocol, using Auth0  
as the external identity provider.

OIDC handles:

- Authentication flows  
- Token issuance (ID token, access token)  
- User profile claims  
- Session lifecycle  
- Security best practices (PKCE, rotating keys, JWKS)  

The application delegates identity concerns to Auth0 while maintaining domain‑level  
Owner aggregates internally.

## Consequences

### Positive

- Strong security — modern, standards‑based authentication  
- No custom password storage or login logic  
- Easy integration with ASP.NET Core authentication middleware  
- Clear separation between identity provider and domain Owner model  
- Supports email verification, password reset, and lockout workflows  
- Scales across environments (dev, preview, production)  
- Reduces long‑term maintenance burden  

### Negative

- Requires external service dependency  
- Must synchronize Auth0 user with domain Owner aggregate  
- More complexity in local development (Auth0 dev tenant)  
- Requires careful configuration of environments and callback URLs  

## Implementation

- Use Auth0’s OIDC endpoints for login and token issuance  
- Configure authentication middleware in `Api` project  
- Store Auth0 domain, client ID, and audience in environment variables  
- Use JWT bearer authentication for API authorization  
- Map OIDC claims to domain Owner identifiers  
- Registration slice creates Owner aggregate after Auth0 user creation  
- Email verification and password reset handled by Auth0 flows  
- Domain events may be raised (e.g., `OwnerAuthenticatedEvent`)  

## Related

- ADR 001 — Vertical Slice Architecture  
- ADR 002 — CQRS Pattern  
- ADR 007 — Registration System  
- US‑110 — Authentication: Owner Login  
- US‑111 — Authentication: Session Management  
- US‑133 — Account Lockout  
- US‑146 — Password Reset Email  
- US‑148 — Email Verification  

## Notes

Keep this document grounded in the actual OIDC and Auth0 implementation and  
update it as the identity subsystem evolves.
