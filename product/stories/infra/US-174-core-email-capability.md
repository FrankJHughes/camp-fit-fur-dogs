---
id: US-174
title: "Core Email Capability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-143
  - US-144
---

# US‑174: Core Email Capability

## Intent
As a **system operator**, I need a unified, reliable, and testable email delivery capability so that all customer‑facing email flows (verification, welcome, password reset) can be implemented consistently and safely.

## Value
Email is a cross‑cutting platform capability. Without a centralized system:

- Each vertical slice would re‑implement email logic  
- Branding and templates would drift  
- Delivery guarantees would be inconsistent  
- Errors would be hard to diagnose  
- Testing would be fragmented  

A single, well‑designed email capability ensures reliability, observability, and consistent owner experience across all email‑based stories.

## Acceptance Criteria

### Email Provider Integration
- Integrate with a production‑grade email provider (e.g., SendGrid, SES).
- Provider configuration is environment‑specific (dev/staging/prod).
- API keys and secrets are stored in secure configuration (not in code).
- Provider failures surface as retryable outbox errors (US‑143).

### Email Abstraction
- Implement an `IEmailService` abstraction with:
  - `SendEmailAsync(to, subject, body, metadata)`
  - Support for HTML + text fallback
  - Support for template rendering
- No story‑specific logic is allowed in the email service.

### Template System
- Email templates are stored in a structured, version‑controlled location.
- Templates support:
  - Dynamic placeholders (name, links, tokens)
  - Branding (logo, colors, typography)
  - Shared layout components (header, footer)
- Local preview mode exists for developers to view rendered templates.

### Outbox Integration
- Email sending is triggered exclusively via outbox messages (US‑143).
- Email handler (US‑144) consumes outbox messages and dispatches emails.
- Outbox retries failed sends with exponential backoff.
- Outbox messages include correlation IDs for tracing.

### Observability
- Logs include:
  - Email type
  - Recipient (masked)
  - Correlation ID
  - Delivery status (success/failure)
- No sensitive content (tokens, full email bodies) is logged.
- Metrics include:
  - Emails sent per hour
  - Failure rate
  - Retry count

### Local & Staging Behavior
- Local development uses:
  - A local preview inbox (e.g., Papercut, MailDev) **or**
  - A file‑based email drop folder
- Staging environment uses the real provider but with:
  - Restricted recipient list  
  - Safe‑send rules  
  - Clear environment tagging in subject lines  

### Testing
- Unit tests verify:
  - Template rendering
  - Email service abstraction
  - Outbox message creation
- Integration tests verify:
  - Outbox → handler → provider flow
  - Retry behavior
  - Local preview mode

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  Owners receive clear, consistent, branded emails across all flows.
- **EG‑03 — Calm Protection**  
  Email flows behave predictably and securely, without exposing sensitive data.
- **EG‑05 — Responsible Partner**  
  The system communicates reliably and professionally at every step.

## Notes
- This story must be completed before:
  - US‑145 (Welcome Email)
  - US‑146 (Password Reset Email)
  - US‑148 (Email Verification)
- Consider adding a future story for:
  - Email analytics
  - Template versioning
  - Multi‑provider failover
- **Demo:** Trigger a test outbox message → handler processes it → email appears in local preview → logs show correlation ID and success.
