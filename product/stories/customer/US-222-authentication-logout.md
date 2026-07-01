---
id: US-222
title: "Authentication: Logout"
epic: ""
milestone: ""
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-110
  - US-111
  - US-179
---

# US‑222 — Authentication: Logout

## Intent
As an **owner**, I must be able to securely log out of my session so that I can ensure my account cannot be accessed by others using my device.

## Acceptance Criteria
- Session cookie invalidated server‑side.
- OIDC logout triggered when OIDC is enabled.
- Redirect to post‑logout page.
- Observability events emitted for logout.
- Works consistently across local, staging, and production hosting engines.
- Frontend logout action implemented.
- Logout endpoint implemented.
- Logout request → command → command handler pipeline implemented.

---

## Frontend

### Logout UI Element
A logout action must be available in the authenticated UI shell (header menu or account menu).

### Behavior
- Calls `POST /api/auth/logout`.
- On success:
  - If OIDC logout is required: follow redirect URL.
  - Otherwise: navigate to `/logged-out`.
- Clears client-side state (React Query cache, local UI state).

---

## Endpoint

### Route
`POST /api/auth/logout`

### Responsibilities
- Validate session cookie.
- Dispatch `LogoutCommand`.
- Return:
  - `204 No Content` for local logout, or
  - `302 Redirect` to OIDC provider logout URL when OIDC logout is enabled.

---

## Request Model

### LogoutRequest
A request object (likely empty) that maintains symmetry with the command pipeline and allows future expansion.

Fields:
- *(none initially)*

---

## Command

### LogoutCommand
Fields:
- `SessionId` — resolved from session cookie.
- `UserId` — resolved from AuthenticatedUserService.
- `IsOidcLogoutEnabled` — resolved from configuration.

---

## Command Handler

### LogoutCommandHandler

Responsibilities:
1. **Invalidate session**
   - Remove session record from persistence.
   - Remove session cookie via response pipeline.

2. **Trigger OIDC logout (optional)**
   - Construct provider logout URL.
   - Include post‑logout redirect URI.

3. **Emit observability events**
   - `Authentication.Logout.Start`
   - `Authentication.Logout.Success`
   - `Authentication.Logout.Failure`

4. **Return redirect or success**
   - If OIDC: return redirect URL.
   - If local: return success and allow frontend redirect.

---

## Hosting / Startup Integration

### Hosting Engine
- Ensure logout endpoint is registered.
- Ensure middleware ordering allows session invalidation before response finalization.

### Startup Engine
- Register `LogoutCommandHandler`.
- Register logout endpoint via Frank.Registration.

---

## Observability

Logout must emit structured events:
- `Authentication.Logout.Start`
- `Authentication.Logout.Success`
- `Authentication.Logout.Failure`

Each event must include:
- Correlation ID
- Session ID
- User ID
- OIDC mode (enabled/disabled)

These events integrate with:
- US‑195 (API Auth Boundary Observability)
- US‑199 (Request Validation Observability)
- US‑200 (Error Boundary Observability)
- US‑201 / US‑202 (Hosting/Startup Observability)

---

## Notes
This story completes the authentication lifecycle by providing a secure, observable logout flow that works consistently across all hosting environments and aligns with Frank’s command/handler architecture.
