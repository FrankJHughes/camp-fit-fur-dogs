---
id: US-156
title: "Privacy Policy & Terms of Service"
epic: Documentation
milestone: M2
status: backlog
domain: docs
vertical_slice: false
dependencies:
  - US-103
---

# US-156: Privacy Policy & Terms of Service

## Intent

As a **potential customer**, I need to understand what data Camp Fit Fur Dogs
collects, how it is used, and what my rights are before I create an account,
so that I can make an informed decision about trusting the service with my
information.

## Value

A privacy policy is legally required for any website that collects personal
data — regardless of business size. California's CalOPPA (California Online
Privacy Protection Act) requires a conspicuous privacy policy for any
commercial website or app that collects personally identifiable information
from California residents. Unlike CCPA/CPRA (which has revenue thresholds),
CalOPPA has no minimum size — it applies to every business.

Terms of Service establish the legal relationship between the business and
its users, limiting liability and defining acceptable use.

## Acceptance Criteria

### Privacy Policy
- [ ] Privacy policy page is accessible from every page (footer link)
- [ ] Privacy policy is written in plain, readable English (not legalese)
- [ ] Privacy policy discloses:
  - What personal information is collected (name, email, phone, dog profiles)
  - How it is collected (registration, profile updates, cookies)
  - Why it is collected (account management, communication, service delivery)
  - Who it is shared with (email/SMS providers, hosting platforms — named)
  - How it is protected (encryption, hashing, access controls)
  - How long it is retained
  - How to request data export or deletion (links to self-service, US-153)
  - How to opt out of non-essential communications (links to preferences, US-147)
  - Cookie usage and tracking (if applicable, links to US-157)
  - Contact information for privacy questions
- [ ] Privacy policy includes an effective date and a changelog for updates
- [ ] Privacy policy is reviewed by legal counsel before publication

### Terms of Service
- [ ] Terms of Service page is accessible from the footer
- [ ] Terms cover: acceptable use, account responsibilities, liability limitations, dispute resolution, termination
- [ ] Terms are written in plain English with section headings for navigation
- [ ] Terms are reviewed by legal counsel before publication

### Consent flow
- [ ] Registration form includes a checkbox: "I agree to the Privacy Policy and Terms of Service" with links
- [ ] Checkbox is not pre-checked (GDPR-style consent, even though GDPR may not apply)
- [ ] Consent timestamp is recorded alongside the account creation
- [ ] When the privacy policy is materially updated, active users are notified via email and prompted to review

### CalOPPA compliance
- [ ] Privacy policy is conspicuously linked from the home page
- [ ] Policy describes the process by which users are notified of changes
- [ ] Policy describes how users can review and request changes to their personal information
- [ ] Effective date is displayed prominently

## Emotional Guarantees

- **EG-01 No Surprises** — The privacy policy is clear, specific, and honest
- **EG-03 Calm Protection** — Owners understand their rights without needing a lawyer
- **EG-06 Explicit Risk** — Data practices are stated plainly, not buried in jargon

## Notes

- CalOPPA applies to ALL commercial websites/apps collecting PII from California residents — no revenue threshold
- CCPA/CPRA does NOT currently apply (revenue threshold: $26.6M or 100K+ consumers)
- If Camp Fit Fur Dogs grows beyond CCPA thresholds, additional disclosures will be required
- Privacy policy content should be drafted by legal counsel — this story covers the technical implementation
- Consider: hosting the privacy policy as a versioned markdown file in the repo for change tracking
- **Demo:** Navigate to the privacy policy from the footer, read through it, check the registration consent flow
