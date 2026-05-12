---
id: US-146
title: "Password Reset Email"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-110
  - US-143
  - US-144
urgency: medium
importance: high
covey_quadrant: Q2
emotional_guarantees:
legal_guarantees:
---

# US‑146: Password Reset Email

## Intent
As an **owner**, I need to be able to reset my password via email so that I can regain access to my account if I forget my credentials.

## Value
Password reset is a critical account recovery flow. Without it, a forgotten password becomes a support burden and a blocked owner. Email‑based reset is the industry standard — secure, familiar, and self‑service — and integrates cleanly with the outbox‑driven email pipeline.

## Acceptance Criteria

### Request Flow
- “Forgot Password” link on the login page navigates to a password reset request form.
- Owner enters their email address and submits the form.
- Whether or not the email exists, the response is identical (prevents enumeration).
- If the email matches an account:
  - A time‑limited, single‑use reset token is generated.
  - Token is stored securely server‑side (hashed, not plaintext).
  - A `PasswordResetRequested` domain event is raised.
  - The outbox (US‑143) dispatches a password reset email via the email handler (US‑144).

### Email Content
- Email includes:
  - A secure link containing the reset token.
  - Clear instructions for setting a new password.
  - Reassurance: *“If you didn’t request this, you can safely ignore it.”*
- Email uses the branded Camp Fit Fur Dogs template.
- Tone is calm, supportive, and non‑alarming.

### Reset Flow
- Reset link navigates to a “Set New Password” form.
- Submitting a valid token and new password updates the password (BCrypt hashed via US‑049).
- Used or expired tokens are invalidated.
- Invalid or tampered tokens return a generic error (no token structure revealed).

### Security & Constraints
- Reset tokens expire after a configurable window (e.g., 1 hour).
- Reset requests are rate‑limited per email to prevent abuse.
- Token storage and validation must be deterministic and side‑effect‑free.

### Testing
- Unit tests verify:
  - Token generation
  - Token hashing
  - Domain event emission
- Integration tests verify:
  - Outbox message creation
  - Email dispatch
  - Successful password reset via token

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  The flow behaves exactly like owners expect from any modern service.
- **EG‑02 — No Blame**  
  Forgetting a password is normal; the flow never implies fault.
- **EG‑03 — Calm Protection**  
  The email reassures owners that no action is needed if they didn’t request the reset.

## Notes
- Depends on US‑110 (login), US‑143 (outbox), and US‑144 (email).
- Consider adding a “Reset Successful” confirmation email.
- Consider adding a cooldown period after multiple reset attempts.
- **Demo:** Click “Forgot Password,” enter email, receive reset email, click link, set new password, log in successfully.
