---
id: US-150
title: "SMS Notification Infrastructure"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-143
  - US-149
---

# US-150: SMS Notification Infrastructure

## Intent

As a **system operator**, I need an SMS delivery service integrated into the
outbox pipeline so that owners who prefer text messages receive notifications
reliably.

## Value

SMS has near-instant delivery and a 98% open rate — far higher than email. For
time-sensitive operational messages (appointment reminders, same-day schedule
changes, facility closures), SMS is the most reliable channel. Integrating SMS
into the existing outbox pipeline means the same transactional guarantee applies:
if the domain event happened, the SMS will be delivered.

## Acceptance Criteria

- [ ] SMS provider is selected and integrated (see options below)
- [ ] `ISmssender` abstraction in the application layer defines the SMS sending contract
- [ ] `SmsOutboxHandler` implements `IOutboxMessageHandler` to dispatch SMS-type outbox messages
- [ ] SMS messages are plain text with a configurable sender name or short code
- [ ] Messages include opt-out instructions ("Reply STOP to unsubscribe") per carrier requirements
- [ ] API keys and credentials are stored in configuration, not source code
- [ ] Failed sends are retried via the outbox retry mechanism (US-143)
- [ ] Sent messages are logged for debugging (recipient hash, timestamp — not content or full number)
- [ ] SMS message length is validated (160 chars for single segment, graceful handling of multi-segment)
- [ ] Integration test sends a test SMS via the provider's sandbox/test mode
- [ ] Phone verification codes (US-149) are sent via this infrastructure

## Emotional Guarantees

- **EG-01 No Surprises** — SMS messages arrive promptly from a recognizable sender
- **EG-03 Calm Protection** — Phone numbers are never logged in full; owner privacy is preserved
- **EG-05 Respect for Attention** — SMS is reserved for high-value, time-sensitive communications

## Provider Options

| Provider | Free Tier | Per SMS (US) | Best For |
|----------|-----------|-------------|----------|
| **Twilio** | $15 trial credit (~1,900 SMS) | ~$0.0079 | Industry standard, best docs, phone verification API |
| **Vonage (Nexmo)** | $2 trial credit | ~$0.0068 | Competitive pricing, good API |
| **Amazon SNS** | 100 free SMS/month (with AWS Free Tier) | ~$0.00645 | Best if already on AWS |
| **Plivo** | Free trial credit | ~$0.0050 | Lowest per-message cost |

> **Recommendation:** Twilio — industry standard, excellent developer experience,
> trial credit covers early development, and Twilio Verify provides a turnkey
> phone verification API that could simplify US-149. The per-message cost is
> minimal for a portfolio project's volume.

> **Cost note:** Unlike email (which has genuinely free tiers), SMS always has a
> per-message cost after trial credits. Budget ~$5-10/month for a portfolio
> project's volume. This is the one hosting cost that is not truly free.

## Architecture

```
Outbox message (type: SMS)
       |
SmsOutboxHandler reads message
       |
Checks channel preference (US-149) — owner opted into SMS?
       |
ISmseSender.SendAsync(to, body)
       |
Twilio/Vonage/SNS API
       |
Mark OutboxMessage.ProcessedAt
```

## Design Seam: Channel Routing

> The outbox produces messages typed by WHAT happened (CustomerCreated,
> BookingConfirmed, etc.). The channel routing layer determines HOW to deliver
> based on the owner's preferences (US-149). A single domain event can produce
> both an email AND an SMS if the owner opted into both channels. The
> `EmailOutboxHandler` (US-144) and `SmsOutboxHandler` (this story) operate
> independently on the same outbox message.

## Notes

- Depends on US-143 (Outbox Pattern) and US-149 (Customer Contact Profile — phone numbers)
- Twilio Verify handles phone verification out of the box — may simplify US-149 phone verification AC
- US carriers require 10DLC registration for application-to-person messaging — research requirements before production
- Consider: message templates with variable substitution (same pattern as email templates)
- **Demo:** Set notification preference to SMS for a category, trigger the event — receive a text message within minutes
