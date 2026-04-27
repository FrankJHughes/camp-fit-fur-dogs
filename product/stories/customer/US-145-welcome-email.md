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
---

# US-145: Welcome Email

## Intent

As a **new owner**, I should receive a warm welcome email after creating my
account so that I know my registration succeeded and I understand what to do
next.

## Value

The welcome email is the first communication from Camp Fit Fur Dogs. It sets the
tone for the entire relationship — confirming the account was created, introducing
the service, and guiding the owner toward their first action (registering a dog).
This is also the proving slice for the outbox-to-email pipeline.

## Acceptance Criteria

- [ ] Creating a customer account (US-027) raises a `CustomerCreated` domain event
- [ ] The outbox interceptor (US-143) writes the event as an outbox message
- [ ] The email handler (US-144) dispatches a welcome email to the new owner
- [ ] Email includes: owner's name, confirmation of account creation, call-to-action to register their first dog
- [ ] Email uses the branded template with Camp Fit Fur Dogs styling
- [ ] Email is sent within 5 minutes of account creation (outbox polling interval)
- [ ] Undeliverable emails are retried via outbox retry mechanism
- [ ] Unit test verifies the domain event is raised on account creation
- [ ] Integration test verifies the outbox message is created and processed

## Emotional Guarantees

- **EG-01 No Surprises** — The email confirms exactly what happened and what's next
- **EG-02 No Blame** — The tone is warm and welcoming, never transactional or robotic
- **EG-03 Calm Protection** — The email doesn't ask for any sensitive information

## Notes

- This is the proving slice for the outbox-to-email pipeline (US-143 + US-144)
- The `CustomerCreated` domain event is the first domain event with a real consumer
- Welcome email content should be reviewed by a human for tone and warmth
- **Demo:** Create an account via the registration page, check inbox within minutes — see the welcome email with your name and a "Register Your Dog" button
