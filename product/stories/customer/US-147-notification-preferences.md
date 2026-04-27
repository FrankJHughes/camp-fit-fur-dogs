---
id: US-147
title: "Notification Preferences"
epic: Customer
milestone: M2+
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-143
  - US-144
---

# US-147: Notification Preferences

## Intent

As an **owner**, I want to control what communications I receive from Camp Fit
Fur Dogs so that I only get messages I find valuable.

## Value

Respect for attention is a core emotional guarantee. Owners who feel spammed
lose trust; owners who control their preferences feel respected. Notification
preferences also ensure compliance with email marketing regulations (CAN-SPAM,
GDPR) by providing a clear opt-out mechanism.

## Acceptance Criteria

- [ ] Owner profile includes a notification preferences section
- [ ] Preferences are categorized by type (e.g., account alerts, booking reminders, promotional)
- [ ] Account-critical emails (password reset, security alerts) cannot be disabled — clearly labeled as mandatory
- [ ] Non-critical emails can be toggled on or off per category
- [ ] Preferences are checked by the outbox message handler before dispatching
- [ ] Unsubscribe link in every non-critical email navigates to the preferences page
- [ ] One-click unsubscribe header (`List-Unsubscribe`) is included in all marketing emails
- [ ] Preferences default to a sensible, minimal set (opt-in for promotional, opt-out for transactional)
- [ ] Changes to preferences take effect immediately

## Emotional Guarantees

- **EG-01 No Surprises** — Owners always know what they're signed up for
- **EG-03 Calm Protection** — No unwanted email; full control over communication
- **EG-05 Respect for Attention** — The system earns the right to communicate by making every message valuable

## Notes

- Depends on US-143 (Outbox Pattern) and US-144 (Email Infrastructure)
- CAN-SPAM requires a visible unsubscribe mechanism in every commercial email
- GDPR requires explicit consent for marketing communications
- Consider: per-channel preferences (email, SMS, push) if additional channels are added later
- **Demo:** Navigate to notification preferences, disable promotional emails, trigger a promotional event — verify no email is sent
