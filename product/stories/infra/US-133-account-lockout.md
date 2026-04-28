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
---

# US-133: Account Lockout

## Intent

As a **system operator**, I need accounts to temporarily lock after repeated
failed login attempts so that brute-force credential attacks are stopped even
if rate limiting is bypassed.

## Value

Rate limiting (US-132) protects at the IP level, but distributed attacks rotate
IPs. Account lockout adds a second layer — protecting the account itself. After
N failures, the account is temporarily locked regardless of the source IP.

## Acceptance Criteria

- [ ] After a configurable number of consecutive failed login attempts, the account is temporarily locked
- [ ] Lockout duration is configurable (e.g., 15 minutes) and increases on repeated lockouts
- [ ] Locked accounts receive a clear, blame-free message ("For your protection, this account is temporarily locked. Please try again in X minutes.")
- [ ] Successful login resets the failure counter
- [ ] Lockout state is tracked server-side, not in cookies or client state
- [ ] Failed attempt count and lockout timestamp are not exposed in API responses (prevents enumeration)
- [ ] Lockout applies to email/password login only — social login providers handle their own lockout
- [ ] Unit tests verify lockout triggers, duration, reset, and escalation
- [ ] Lockout events are logged for security monitoring

## Emotional Guarantees

- **EG-01 No Surprises** — The owner is told clearly why they can't log in and when to try again
- **EG-02 No Blame** — Messages never imply the owner did something wrong
- **EG-03 Calm Protection** — Lockout is framed as protection, not punishment

## Notes

- Depends on US-110 (login) and US-111 (session management)
- Do NOT reveal whether an account exists — failed login messages should be identical for invalid email vs. wrong password vs. locked account
- Consider: persistent lockout state in the database vs. distributed cache
- **Demo:** Attempt login 5 times with wrong password — show lockout message, wait for the window to expire, log in successfully
