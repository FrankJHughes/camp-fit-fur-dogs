---
id: US-145
title: "Welcome Email"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-027
  - US-143
  - US-144
urgency: medium
importance: high
covey_quadrant: Q2
emotional_guarantees:
legal_guarantees:
---

# US‑145: Welcome Email

## Intent
As a **new owner**, I should receive a warm welcome email after creating my account so that I know my registration succeeded and I understand what to do next.

## Value
The welcome email is the first communication from Camp Fit Fur Dogs. It sets the tone for the entire relationship — confirming the account was created, introducing the service, and guiding the owner toward their first action (registering a dog).  
This story is also the proving slice for the outbox‑to‑email pipeline.

## Acceptance Criteria

### Event & Outbox Flow
- Creating a customer account (US‑027) raises a `CustomerCreated` domain event.
- The outbox interceptor (US‑143) writes the event as an outbox message.
- The email handler (US‑144) processes the outbox message and dispatches a welcome email.

### Email Content
- Email includes:
  - Owner’s name  
  - Confirmation that the account was created  
  - A clear call‑to‑action to register their first dog  
- Email uses the branded Camp Fit Fur Dogs template.
- Email tone is warm, friendly, and human — not transactional.

### Delivery Guarantees
- Email is sent within 5 minutes of account creation (based on outbox polling interval).
- Undeliverable emails are retried via the outbox retry mechanism.
- Email delivery failures are logged with correlation IDs (no sensitive data).

### Testing
- Unit test verifies the `CustomerCreated` domain event is raised.
- Integration test verifies:
  - Outbox message is created  
  - Outbox message is processed  
  - Email is dispatched successfully  

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  The email confirms exactly what happened and what the owner should do next.
- **EG‑02 — No Blame**  
  Tone is warm and welcoming — never robotic or transactional.
- **EG‑03 — Calm Protection**  
  The email does not ask for sensitive information or create anxiety.

## Notes
- This is the proving slice for the outbox‑to‑email pipeline (US‑143 + US‑144).
- The `CustomerCreated` event is the first domain event with a real consumer.
- Welcome email content should be reviewed by a human for tone and warmth.
- **Demo:** Create an account via the registration page → check inbox within minutes → see welcome email with name + “Register Your Dog” button.
