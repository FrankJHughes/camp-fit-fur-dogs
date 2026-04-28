---
id: US-153
title: "Account Data Export & Deletion"
epic: Customer
milestone: M2+
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-027
  - US-125
---

# US-153: Account Data Export & Deletion

## Intent

As an **owner**, I want to download all my data and permanently delete my
account when I choose, so that I remain in full control of my personal
information.

## Value

Data ownership is a trust signal. When owners know they can leave — and take
their data with them — they feel safe staying. Conversely, if an owner can't
find a delete button, anxiety rises: "What are they doing with my data?" Data
export and deletion are also regulatory requirements under GDPR, CCPA, and
similar privacy laws.

## Acceptance Criteria

### Data export
- [ ] Owner can request a full export of their data from the profile settings
- [ ] Export includes: account info, dog profiles, booking history, notification preferences, login activity
- [ ] Export is delivered as a structured, human-readable format (JSON or CSV in a ZIP)
- [ ] Export is generated asynchronously — owner receives a download link via email when ready
- [ ] Export download link is time-limited and single-use
- [ ] Export does not include internal system data (IDs, audit metadata, outbox messages)
- [ ] Export request is rate-limited (e.g., max 1 per day)

### Account deletion
- [ ] Owner can request permanent account deletion from the profile settings
- [ ] Deletion request requires password confirmation (or social login re-authentication)
- [ ] A confirmation dialog clearly explains what will happen: all data permanently removed, cannot be undone
- [ ] Deletion has a grace period (e.g., 30 days) during which the account is soft-deleted (US-125) and can be recovered
- [ ] During the grace period, the owner receives a confirmation email with a "Cancel deletion" link
- [ ] After the grace period, all owner data is permanently purged (hard delete)
- [ ] Active sessions are terminated immediately on deletion request
- [ ] Deletion cancellation restores the account to full visibility (clears soft-delete)

### Privacy compliance
- [ ] Data export and deletion are accessible without contacting support (self-service)
- [ ] The privacy policy references the self-service export and deletion options
- [ ] Deletion removes data from backups within a documented retention window (or documents the limitation)

## Emotional Guarantees

- **EG-01 No Surprises** — The deletion flow clearly explains the timeline and consequences
- **EG-02 No Blame** — Leaving is never guilted; the system respects the decision gracefully
- **EG-03 Calm Protection** — The grace period protects against impulsive or accidental deletion
- **EG-06 Explicit Risk** — Permanence is stated clearly; no ambiguity about what "delete" means

## Design Seam: Soft-Delete

> Account deletion reuses the soft-delete infrastructure from US-125. During the
> grace period, the account is soft-deleted (hidden but recoverable). After
> expiration, a background job permanently purges all associated data. This is
> the same pattern used for dog removal — consistent behavior across the system.

## Notes

- Depends on US-027 (Create Customer Account), US-125 (Soft-Delete & Restore)
- GDPR Article 17: Right to Erasure — deletion must be complete and timely
- CCPA: Right to Delete — similar requirements for California residents
- Consider: what happens to orphaned data (e.g., reviews, shared bookings) when an account is deleted?
- Consider: should deletion be available via API as well as UI?
- **Demo:** Request data export, receive ZIP via email. Request deletion, receive confirmation email, cancel within grace period — account restored. Request again, wait for grace period — account permanently gone.
