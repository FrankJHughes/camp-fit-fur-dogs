---
id: US-133
title: "Account Lockout"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-110
  - US-111
urgency: high
importance: high
covey_quadrant: Q1
emotional_guarantees:
legal_guarantees:
---

# US‑133: Account Lockout

## Intent
As a **system operator**, I need accounts to **temporarily lock after repeated failed login attempts** so that brute‑force credential attacks are stopped even if rate limiting is bypassed.

## Value
Rate limiting (US‑132) protects at the IP level, but distributed attacks rotate IPs.  
Account lockout adds a second layer — protecting the account itself.  
After *N* failures, the account is temporarily locked regardless of the source IP, preventing automated guessing attacks and reducing risk to owners.

## Acceptance Criteria

### Lockout Triggering
- After a configurable number of consecutive failed login attempts, the account is locked.
- Lockout duration is configurable (e.g., 15 minutes).
- Repeated lockouts increase the duration (progressive backoff).
- Successful login resets the failure counter.

### Security Behavior
- Lockout state is tracked server‑side (database or distributed cache).
- API responses do **not** reveal whether:
  - the email exists  
  - the password is wrong  
  - the account is locked  
  (all responses use the same generic message)
- Lockout applies only to email/password login — social login providers handle their own lockout.

### User Experience
- Locked accounts receive a clear, blame‑free message:  
  *“For your protection, this account is temporarily locked. Please try again in X minutes.”*
- Message never implies fault or wrongdoing.
- Message never reveals internal counters or lockout duration beyond the user‑facing text.

### Observability & Testing
- Lockout events are logged for security monitoring (no sensitive data).
- Unit tests verify:
  - lockout triggers  
  - duration  
  - reset behavior  
  - escalation  
- Integration test simulates repeated failed logins and confirms lockout activates.

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  Owners are told clearly why they can’t log in and when they can try again.
- **EG‑02 — No Blame**  
  Messages never imply the owner did something wrong.
- **EG‑03 — Calm Protection**  
  Lockout is framed as protection, not punishment.

## Notes
- Depends on US‑110 (login) and US‑111 (session management).
- Do **not** reveal whether an account exists — all failed login responses must be identical.
- Consider persistent lockout state in the database vs. distributed cache.
- **Demo:** Attempt login 5 times with wrong password → lockout message → wait for window to expire → successful login.
