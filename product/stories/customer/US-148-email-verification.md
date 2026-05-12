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

# US‑148: Email Verification

## Intent
As a **new owner**, I must verify my email address after creating an account so that Camp Fit Fur Dogs knows the email belongs to me and can communicate with me reliably.

## Value
Unverified email addresses create security, deliverability, and trust problems.  
Verification ensures:

- The email truly belongs to the owner  
- Password resets and booking confirmations reach the right person  
- Typos and disposable emails don’t pollute the customer database  
- The communication channel is trustworthy before sensitive actions occur  

This story also establishes the sequencing rules for the welcome email (US‑145).

## Acceptance Criteria

### Registration Flow
- After successful account creation (US‑126), the owner sees a **“Check your email”** confirmation page.
- The page explains what to expect and includes a **“Resend verification email”** option.
- A `CustomerCreated` domain event triggers a verification email via the outbox (US‑143 → US‑144).
- The verification email contains:
  - A unique, time‑limited, single‑use token embedded in a link  
  - Warm, branded messaging  
  - Clear next steps  
- Token is stored server‑side as a **hash**, not plaintext.
- Token includes an expiration timestamp (default: 24 hours).

### Verification Flow
- Clicking the verification link navigates to a verification endpoint.
- Valid token:
  - Marks the email as verified  
  - Displays a success confirmation  
  - Includes a **“Log in”** call‑to‑action  
- Expired token:
  - Shows a clear message  
  - Offers a **“Resend verification email”** option  
- Already‑used token:
  - Shows a message indicating the email is already verified  
- Invalid or tampered token:
  - Returns a **generic error** (no token structure revealed)

### Account State
- Customer entity includes `EmailVerified` or `EmailVerifiedAt`.
- Unverified accounts:
  - Can log in  
  - See a persistent, non‑blocking banner prompting verification  
  - Cannot perform sensitive actions (password change, booking) — scope TBD  
- Resend verification is **rate‑limited** (e.g., max 3 per hour).

### Social Login Bypass
- Accounts created via social login (Microsoft, Google, Apple, Amazon) are **auto‑verified**.
- Verification flow applies **only** to email/password registration (US‑126).
- For social login accounts, `EmailVerifiedAt` is set to the account creation timestamp.

### Welcome Email Sequencing
- Welcome email (US‑145) is sent **after successful verification**, not at account creation.
- The verification email itself must be warm enough to serve as the first impression.

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  The “check your email” page sets clear expectations; no confusion about next steps.
- **EG‑02 — No Blame**  
  Expired tokens offer a resend option — never scolding or punitive.
- **EG‑03 — Calm Protection**  
  Verification protects owners from someone else registering with their email.
- **EG‑05 — Responsible Partner**  
  Verification and welcome are separate emails, but both feel warm and human.

## Design Seam: Hybrid Login Model
> Email verification applies only to the email/password registration path.  
> Social login providers have already verified the email through their own flows.  
> Accounts created via social login skip verification entirely and are immediately active.

## Notes
- Depends on US‑027 (Create Customer Account), US‑126 (Create Account Page), US‑143 (Outbox), US‑144 (Email).
- Welcome email sequencing changes: welcome is sent on `EmailVerified`, not `CustomerCreated`.
- The verification email **is the first impression** — invest in tone and design.
- Consider whether the verification link should auto‑log‑in the owner or redirect to login.
- Consider a grace period before restricting unverified accounts.
- **Demo:** Register with email/password → see “Check your email” → click verification link → see success → log in → banner disappears → welcome email arrives.
