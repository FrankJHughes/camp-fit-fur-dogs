---
id: US-158
title: "CAN-SPAM & TCPA Compliance Hardening"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-144
  - US-147
  - US-150
---

# US-158: CAN-SPAM & TCPA Compliance Hardening

## Intent

As a **system operator**, I need all outbound email and SMS communications to
comply with CAN-SPAM and TCPA regulations so that the business is protected
from fines and carrier blocking.

## Value

CAN-SPAM violations carry penalties up to $51,744 per email. TCPA violations
range from $500 to $1,500 per text message and can escalate to class-action
lawsuits. US mobile carriers actively block non-compliant SMS senders. These
are not theoretical risks — they are actively enforced.

US-144 (email), US-147 (notification preferences), and US-150 (SMS) already
cover the major technical requirements. This story formalizes the compliance
layer with explicit verification, documentation, and audit trail.

## Acceptance Criteria

### Email (CAN-SPAM)
- [ ] Every commercial email includes a visible, functional unsubscribe link
- [ ] Unsubscribe requests are honored within 10 business days (must be honored, existing AC covers this)
- [ ] Every email includes the physical mailing address of the business (required by CAN-SPAM)
- [ ] "From" and "Reply-To" addresses accurately identify the sender
- [ ] Subject lines are not deceptive or misleading
- [ ] Transactional emails (password reset, security alerts) are clearly distinguished from commercial emails

### SMS (TCPA)
- [ ] Prior express written consent is obtained before sending any marketing SMS
- [ ] Consent record captures: phone number, timestamp, consent language displayed, and method (checkbox, form)
- [ ] Consent is one-to-one (specific to Camp Fit Fur Dogs — not shared or sold)
- [ ] Every SMS includes opt-out instructions ("Reply STOP to unsubscribe")
- [ ] STOP requests are processed within 10 business days
- [ ] Opt-out confirmation message is sent after STOP is received
- [ ] SMS is only sent during acceptable hours (8 AM - 9 PM in the recipient's local time zone)
- [ ] Sending numbers are registered for 10DLC (Application-to-Person messaging) before production use

### Documentation & Audit
- [ ] Consent records are immutable and retained for the duration of the customer relationship + 5 years
- [ ] Unsubscribe/opt-out events are logged with timestamp
- [ ] A compliance checklist document exists in the repo covering CAN-SPAM and TCPA requirements
- [ ] The outbox (US-143) checks consent status before dispatching any non-transactional message

## Emotional Guarantees

- **EG-03 Calm Protection** — The system never sends a message without proper consent
- **EG-05 Respect for Attention** — Only consented, relevant communications are delivered

## Legal Reference

| Law | Scope | Penalty |
|-----|-------|---------|
| **CAN-SPAM** (2003) | Commercial email | Up to $51,744 per email |
| **TCPA** (1991, amended) | SMS and voice calls | $500-$1,500 per message; class-action eligible |
| **CTIA Guidelines** | SMS via US carriers | Non-compliance leads to carrier filtering/blocking |
| **FCC One-to-One Rule** (2025) | SMS consent | Consent must name a single specific business |

## Notes

- Most technical requirements are already covered by US-144, US-147, and US-150 — this story adds the compliance verification and audit layer
- 10DLC registration is required for any business sending SMS in the US via A2P channels — carriers will filter unregistered traffic
- Physical mailing address in emails: can use a PO Box or registered agent address
- Consider: a compliance review checklist that runs before the first production email/SMS is sent
- **Demo:** Send a commercial email — verify unsubscribe link, physical address, and accurate sender. Send an SMS — verify STOP instructions and consent record exists.
