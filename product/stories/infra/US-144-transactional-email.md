---
id: US-144
title: "Transactional Email Infrastructure"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-143
---

# US-144: Transactional Email Infrastructure

## Intent

As a **system operator**, I need a transactional email service integrated into
the application so that the outbox can dispatch emails reliably through a
proven email delivery provider.

## Value

Email is the primary communication channel for account lifecycle events
(welcome, password reset, booking confirmation). A transactional email provider
handles deliverability, reputation management, bounce handling, and analytics —
problems that are impractical to solve with a raw SMTP server.

## Acceptance Criteria

- [ ] Transactional email provider is selected and integrated (see options below)
- [ ] `IEmailSender` abstraction in the application layer defines the email sending contract
- [ ] `EmailOutboxHandler` implements `IOutboxMessageHandler` to dispatch email-type outbox messages
- [ ] Email templates are stored as structured objects (not inline strings) with placeholder substitution
- [ ] Sender address and reply-to are configurable
- [ ] API keys and credentials are stored in configuration, not source code
- [ ] Failed sends are retried via the outbox retry mechanism (US-143)
- [ ] Sent emails are logged for debugging (subject, recipient, timestamp — not body)
- [ ] Integration test sends a test email via the provider's sandbox/test mode

## Emotional Guarantees

- **EG-01 No Surprises** — Emails arrive promptly and from a recognizable sender address
- **EG-03 Calm Protection** — No email content is logged; owner privacy is preserved

## Provider Options (free tier)

| Provider | Free Tier | API/SMTP | Best For |
|----------|-----------|----------|----------|
| **Resend** | 3,000 emails/mo, 100/day | REST API | Best DX, modern API, generous free tier |
| **Brevo (Sendinblue)** | 300 emails/day | REST + SMTP | Good all-around, SMTP fallback |
| **Mailgun** | 1,000 emails/mo (trial) | REST + SMTP | Proven at scale, limited free tier |
| **SendGrid** | 100 emails/day | REST + SMTP | Industry standard, Twilio ecosystem |

> **Recommendation:** Resend — modern REST API, excellent developer experience,
> 3,000 emails/month free tier is more than enough for a portfolio project, and
> custom domain verification is straightforward.

## Notes

- Depends on US-143 (Outbox Pattern) — emails are dispatched from the outbox, not inline
- Email templates should support a base layout (header, footer, branding) with per-email content
- Consider: HTML email rendering library (e.g., Mjml, Razor templates)
- **Demo:** Trigger a domain event that produces an outbox message, watch the background service dispatch it as an email, check the inbox
