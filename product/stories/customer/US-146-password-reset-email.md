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
---

# US-146: Password Reset Email

## Intent

As an **owner**, I need to be able to reset my password via email so that I can
regain access to my account if I forget my credentials.

## Value

Password reset is a critical account recovery flow. Without it, a forgotten
password means a locked-out owner and a support burden. The email-based reset
flow is the industry-standard approach — secure, familiar, and self-service.

## Acceptance Criteria

- [ ] "Forgot Password" link on the login page navigates to a password reset request form
- [ ] Owner enters their email address and submits the form
- [ ] If the email matches an account, a time-limited, single-use reset token is generated
- [ ] Reset token is stored securely server-side (hashed, not plaintext)
- [ ] A `PasswordResetRequested` domain event is raised
- [ ] The outbox dispatches a password reset email with a secure link containing the token
- [ ] Reset link navigates to a "Set New Password" form
- [ ] Submitting a valid token and new password updates the password (BCrypt hashed via US-049)
- [ ] Used or expired tokens are invalidated
- [ ] Reset flow returns the same response whether or not the email exists (prevents enumeration)
- [ ] Email includes: security guidance ("If you didn't request this, you can ignore it")

## Emotional Guarantees

- **EG-01 No Surprises** — The reset flow works exactly as expected from any other site
- **EG-02 No Blame** — Forgetting a password is normal; the flow never implies fault
- **EG-03 Calm Protection** — The email includes reassurance that no action is needed if the request wasn't theirs

## Notes

- Depends on US-110 (login), US-143 (outbox), US-144 (email)
- Reset tokens should expire after a configurable window (e.g., 1 hour)
- Rate-limit reset requests per email to prevent abuse
- **Demo:** Click "Forgot Password," enter email, receive reset email, click link, set new password, log in successfully
