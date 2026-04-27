---
id: US-165
title: "Booking Notifications"
epic: Booking
milestone: M3
status: backlog
domain: booking
vertical_slice: true
dependencies:
  - US-143
  - US-144
  - US-147
  - US-162
---

# US-165: Booking Notifications

## Intent

As an **owner**, I want to receive confirmation when I book and reminders
before my appointment so that I never miss a service.

## Value

Booking notifications close the communication loop. Confirmation builds
confidence ("Yes, it's booked"). Reminders reduce no-shows — the #1
operational headache for service businesses. SMS reminders in particular
have been shown to reduce no-shows by up to 40% in service industries.

## Acceptance Criteria

### Confirmation
- [ ] `BookingCreated` domain event triggers a confirmation notification via the outbox
- [ ] Confirmation includes: service name, date, time, dog name, location/address, and any preparation instructions
- [ ] Confirmation is sent via the owner's preferred channel (email, SMS, or both — US-147/US-149)

### Reminder
- [ ] A reminder notification is sent at a configurable interval before the appointment (e.g., 24 hours)
- [ ] Reminder includes: service name, date, time, dog name, and a "Cancel" link
- [ ] Reminder scheduling is handled by outbox entries with a `DispatchAfter` timestamp
- [ ] If the booking is cancelled before the reminder fires, the reminder is suppressed

### Cancellation
- [ ] `BookingCancelled` domain event triggers a cancellation confirmation notification
- [ ] Cancellation notification includes: service name, date, time, and cancellation policy outcome

### Channel routing
- [ ] All notifications respect the owner's channel preferences (US-149)
- [ ] All notifications respect the owner's category preferences (US-147)
- [ ] Booking-related notifications are categorized as "Booking Reminders" in notification preferences

## Emotional Guarantees

- **EG-01 No Surprises** — Owners always know the status of their bookings
- **EG-05 Respect for Attention** — Reminders are timely, not excessive (one reminder per booking)

## Legal Guarantees

- **LG-03 Consensual** — Notifications respect documented channel consent; SMS includes STOP instructions

## Notes

- Depends on US-143 (Outbox), US-144 (Email), US-147 (Preferences), US-162 (Booking)
- Reminder scheduling: the outbox entry is created at booking time with a future `DispatchAfter` — the background processor skips it until the time arrives
- Consider: preparation instructions per service type (e.g., "No food 2 hours before grooming")
- **Demo:** Book a daycare session — receive confirmation email immediately, receive SMS reminder 24 hours before the session
