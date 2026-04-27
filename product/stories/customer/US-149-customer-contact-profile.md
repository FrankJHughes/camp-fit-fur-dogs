---
id: US-149
title: "Customer Contact Profile"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-027
  - US-126
  - US-148
---

# US-149: Customer Contact Profile

## Intent

As an **owner**, I need to manage my contact information — including email
addresses and phone numbers — and choose how Camp Fit Fur Dogs reaches me, so
that I receive important communications through the channels I prefer.

## Value

A dog daycare business lives and dies by operational communication. Appointment
reminders, schedule changes, facility closures, and safety alerts must reach
owners reliably and promptly. Different owners prefer different channels —
some check email religiously, others only respond to texts. Capturing and
respecting channel preferences reduces missed communications, no-shows, and
owner frustration.

Multi-channel contact also provides redundancy. If an email bounces, an SMS
can still reach the owner. If a phone number changes, email is the fallback.

## Acceptance Criteria

### Contact data model
- [ ] Customer entity supports one or more contact methods (email, phone)
- [ ] Each contact method has: type (email/phone), value, verified status, verified-at timestamp, and primary flag
- [ ] At least one verified email is required (established by US-148)
- [ ] Phone numbers are stored in E.164 format with validation
- [ ] One contact method per type can be marked as primary (the default channel for that type)

### Profile management UI
- [ ] Owner profile page includes a "Contact Information" section
- [ ] Owner can add a phone number to their profile
- [ ] Owner can update or remove non-primary contact methods
- [ ] Owner can change which contact method is primary per type
- [ ] Adding a new phone number triggers SMS verification (verification code sent to the number)
- [ ] Adding a new email address triggers email verification (verification link sent to the address)
- [ ] Unverified contact methods are visually distinguished and cannot be set as primary

### Phone verification
- [ ] SMS verification sends a 6-digit code to the phone number
- [ ] Code expires after a configurable window (e.g., 10 minutes)
- [ ] Code entry is rate-limited (e.g., max 5 attempts per code)
- [ ] Resend is rate-limited (e.g., max 3 per hour)
- [ ] Successful verification marks the phone number as verified

### Channel preferences
- [ ] Owner can select preferred channel per notification category: email only, SMS only, or both
- [ ] Default preference for new accounts: email for all categories
- [ ] Channel preference is only selectable for verified contact methods
- [ ] Preferences are respected by the outbox message handlers (US-143)

## Emotional Guarantees

- **EG-01 No Surprises** — Owners always know what channels are active and what they'll receive
- **EG-02 No Blame** — Verification is framed as protection, not bureaucracy
- **EG-03 Calm Protection** — Contact data is handled respectfully; phone numbers are never shared
- **EG-05 Responsible Partner** — Owners control exactly how they're reached

## Design Seam: Notification Preferences

> US-147 (Notification Preferences) defines WHAT categories of notifications an
> owner receives (account alerts, booking reminders, promotional). This story
> defines HOW they receive them (email, SMS, or both). The two compose: an owner
> can say "booking reminders via SMS only" or "account alerts via email and SMS."
> The outbox handler checks both the category preference (US-147) and the channel
> preference (this story) before dispatching.

## Notes

- Depends on US-027 (Create Customer Account), US-126 (Create Account Page), US-148 (Email Verification)
- US-150 (SMS Infrastructure) must be in place for phone verification to work
- E.164 format: international format like +15551234567 — use a validation library (e.g., libphonenumber)
- Consider: should the registration form (US-126) collect phone number optionally, or only via profile later?
- Consider: operational staff may need to view customer contact info for manual outreach
- **Demo:** Add a phone number to your profile, receive and enter the verification code, set booking reminders to SMS — receive a text when a booking is confirmed
