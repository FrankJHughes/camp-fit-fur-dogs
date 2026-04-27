---
id: US-148
title: "Email Verification"
epic: Customer
milestone: M1+
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-027
  - US-126
  - US-143
  - US-144
---

# US-148: Email Verification

## Intent

As a **new owner**, I must verify my email address after creating an account so
that Camp Fit Fur Dogs knows the email belongs to me and can communicate with me
reliably.

## Value

Unverified email addresses are a security and deliverability risk. A verification
step confirms ownership before the system trusts the address for password resets,
booking confirmations, and account recovery. Without it, typos, disposable
addresses, and malicious registrations pollute the customer database and erode
trust in the communication channel.

## Acceptance Criteria

### Registration flow
- [ ] After successful account creation (US-126), the owner sees a "Check your email" confirmation page
- [ ] The confirmation page explains what to expect and offers a "Resend verification email" option
- [ ] A `CustomerCreated` domain event triggers a verification email via the outbox (US-143)
- [ ] The verification email contains a unique, time-limited, single-use token embedded in a link
- [ ] Token is stored server-side as a hash (not plaintext) with an expiration timestamp
- [ ] Token expires after a configurable window (e.g., 24 hours)

### Verification flow
- [ ] Clicking the verification link navigates to a verification endpoint
- [ ] Valid token marks the email as verified and displays a success confirmation with a "Log in" call-to-action
- [ ] Expired token displays a clear message with a "Resend verification email" option
- [ ] Already-used token displays a message indicating the email is already verified
- [ ] Invalid/tampered token displays a generic error (does not reveal token structure)

### Account state
- [ ] Accounts have an `EmailVerified` boolean (or `EmailVerifiedAt` timestamp) on the customer entity
- [ ] Unverified accounts can log in but see a persistent, non-blocking banner prompting verification
- [ ] Unverified accounts cannot perform sensitive actions (e.g., password change, booking) — scope TBD
- [ ] Resend verification is rate-limited (e.g., max 3 per hour) to prevent abuse

### Social login bypass
- [ ] Accounts created via social login (US-128 through US-131) are auto-verified — the provider has already confirmed the email
- [ ] The verification flow only applies to email/password registration (US-126)

### Welcome email sequencing
- [ ] Welcome email (US-145) is sent AFTER successful verification, not at account creation
- [ ] The verification email itself serves as the first contact — warm, branded, and clear

## Emotional Guarantees

- **EG-01 No Surprises** — The "check your email" page sets clear expectations; no confusion about next steps
- **EG-02 No Blame** — Expired tokens offer a resend option, never scold
- **EG-03 Calm Protection** — Verification protects the owner from someone else registering with their email
- **EG-05 Respect for Attention** — Verification and welcome are separate emails, but the verification email is warm enough to also feel welcoming

## Design Seam: Hybrid Login Model

> Email verification only applies to the email/password registration path.
> Social login providers (Microsoft, Google, Apple, Amazon) have already verified
> the email through their own flows. Accounts created via social login skip
> verification entirely and are immediately fully active. This means
> `EmailVerifiedAt` can be set to the account creation timestamp for social
> login accounts.

## Notes

- Depends on US-027 (Create Customer Account), US-126 (Create Account Page), US-143 (Outbox), US-144 (Email)
- US-145 (Welcome Email) sequencing changes — welcome is sent on `EmailVerified` event, not `CustomerCreated`
- The verification email IS the first impression — invest in its tone and design
- Consider: should the verification link auto-log-in the owner, or redirect to the login page?
- Consider: grace period before restricting unverified accounts (allow exploration, restrict commitment)
- **Demo:** Register with email/password, check inbox, click verification link, see success, log in — banner is gone, welcome email arrives
