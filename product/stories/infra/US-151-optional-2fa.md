---
id: US-151
title: "Optional Two-Factor Authentication"
epic: Infrastructure
milestone: M2+
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-110
  - US-111
  - US-144
  - US-150
---

# US-151: Optional Two-Factor Authentication

## Intent

As an **owner**, I want the option to add a second factor to my login so that
my account is protected even if my password is compromised.

## Value

2FA is the single most effective defense against credential compromise. Offering
it as an option — not a mandate — respects owners who value simplicity while
giving security-conscious owners the protection they expect. The mere presence
of the option signals that Camp Fit Fur Dogs takes security seriously.

## Acceptance Criteria

### Setup
- [ ] Owner profile includes a "Security" section with a "Set up two-factor authentication" option
- [ ] Owner can enable 2FA via authenticator app (TOTP — Google Authenticator, Authy, etc.)
- [ ] Setup flow displays a QR code and manual entry key for the authenticator app
- [ ] Owner must enter a valid TOTP code to confirm setup (proves the authenticator is working)
- [ ] Recovery codes are generated and displayed once — owner is prompted to save them securely
- [ ] Recovery codes are stored as hashes, not plaintext

### Login flow
- [ ] After successful password entry, 2FA-enabled accounts are prompted for a TOTP code
- [ ] Invalid TOTP code displays a clear error with retry option
- [ ] Owner can use a recovery code instead of TOTP (one-time use, consumed on use)
- [ ] "Remember this device" option skips 2FA on trusted devices for a configurable period
- [ ] Social login bypasses 2FA — the provider handles its own second factor

### Management
- [ ] Owner can disable 2FA from the security settings (requires current password confirmation)
- [ ] Owner can regenerate recovery codes (invalidates previous set)
- [ ] Disabling 2FA sends a security notification (US-152)

### SMS option (stretch)
- [ ] Owner can optionally receive 2FA codes via SMS instead of authenticator app
- [ ] SMS 2FA uses the verified phone number from US-149
- [ ] Authenticator app is recommended over SMS (communicated in UI)

## Emotional Guarantees

- **EG-01 No Surprises** — 2FA is never forced; the owner opts in deliberately
- **EG-03 Calm Protection** — Recovery codes ensure the owner is never locked out permanently
- **EG-06 Explicit Risk** — The setup flow clearly explains what 2FA does and what happens if the authenticator is lost

## Notes

- 2FA is OPTIONAL — this is a deliberate product decision to reduce friction
- TOTP is preferred over SMS (SIM swapping risk) but both should be available
- Use a standard TOTP library (e.g., `OtpNet` for .NET)
- Recovery codes: generate 10, each usable once — standard pattern
- **Demo:** Enable 2FA in settings, scan QR with Google Authenticator, log out, log in — enter TOTP code from the app
