---
id: US-152
title: "Login Activity & Security Notifications"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-110
  - US-111
  - US-143
  - US-144
---

# US-152: Login Activity & Security Notifications

## Intent

As an **owner**, I want to see my recent login history and be notified when
something unusual happens with my account so that I can spot unauthorized
access quickly.

## Value

Most owners will never enable 2FA. Login activity and security notifications
provide passive security awareness with zero friction — no setup, no extra
steps at login. The owner simply receives an alert when a new device or
location accesses their account, and can review their login history anytime.

This is the highest-trust, lowest-friction security feature available. It tells
owners: "We're watching your back."

## Acceptance Criteria

### Login activity log
- [ ] Every successful login records: timestamp, IP address (hashed for display), approximate location (city/region from IP), device/browser (from User-Agent), and login method (password, social provider, 2FA used)
- [ ] Owner can view their login activity from the profile security section
- [ ] Activity log shows the most recent N logins (configurable, e.g., 20)
- [ ] Current active session is highlighted
- [ ] Owner can terminate any session other than the current one ("Log out everywhere")

### Security notifications
- [ ] New device/location login triggers a notification via the owner's preferred channel (email/SMS)
- [ ] Notification includes: timestamp, device info, approximate location, and a "Not you? Secure your account" link
- [ ] Password change triggers a notification
- [ ] 2FA enabled/disabled triggers a notification
- [ ] Email address change triggers a notification to BOTH the old and new addresses
- [ ] Notifications are sent via the outbox (US-143) — guaranteed delivery
- [ ] Security notifications CANNOT be disabled — they are always on (unlike marketing/operational notifications)

### Device recognition
- [ ] A secure, per-device fingerprint (cookie or token) identifies known devices
- [ ] First login from an unrecognized device triggers the new-device notification
- [ ] Recognized devices do not trigger repeated notifications
- [ ] Owner can review and remove recognized devices from security settings

## Emotional Guarantees

- **EG-01 No Surprises** — Owners are proactively informed of security-relevant events
- **EG-03 Calm Protection** — Security monitoring happens silently; alerts only when something unusual occurs
- **EG-06 Explicit Risk** — "Not you?" link provides an immediate remediation path

## Notes

- Depends on US-110 (login), US-111 (session management), US-143 (outbox), US-144 (email)
- IP geolocation: use a free service (e.g., ip-api.com, MaxMind GeoLite2) — city-level accuracy is sufficient
- Device fingerprint should be a secure, HttpOnly cookie — not browser fingerprinting
- Security notifications are mandatory (not governed by US-147 preferences) — this is a deliberate product decision
- **Demo:** Log in from a new browser — receive a "New login detected" email with device and location info, then view the login in your activity log
