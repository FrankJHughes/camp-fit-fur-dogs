---
id: US-157
title: "Cookie Consent & Tracking Transparency"
epic: Customer
milestone: M2+
status: backlog
domain: customer
vertical_slice: false
dependencies:
  - US-103
  - US-156
---

# US-157: Cookie Consent & Tracking Transparency

## Intent

As a **visitor**, I want to understand what cookies and tracking the site uses
and have the option to decline non-essential tracking, so that my browsing
activity is respected.

## Value

Cookie consent is a legal requirement under GDPR (for EU visitors) and
increasingly expected by US consumers. Even without a legal mandate for US-only
sites, a cookie banner signals respect for privacy and builds trust. If Camp
Fit Fur Dogs ever serves EU visitors (even accidentally), GDPR cookie consent
is required.

More importantly, transparent tracking practices are a trust signal. Owners
who see a cookie banner think: "This business respects my privacy."

## Acceptance Criteria

- [ ] If the site uses analytics (e.g., Google Analytics, Plausible, PostHog), a cookie consent banner is displayed on first visit
- [ ] Banner clearly describes what cookies are used and why
- [ ] Banner offers three options: Accept All, Reject Non-Essential, Manage Preferences
- [ ] Rejecting non-essential cookies disables analytics/tracking scripts — they must not load until consent is granted
- [ ] Cookie preferences are stored in a first-party cookie (not a tracking cookie)
- [ ] Cookie preferences can be changed at any time (link in footer: "Cookie Preferences")
- [ ] If no analytics or tracking cookies are used, no banner is needed — but a cookie policy page should still exist explaining session cookies
- [ ] Essential cookies (session, authentication, CSRF) do not require consent and are always active
- [ ] Cookie policy is linked from the privacy policy (US-156)

## Emotional Guarantees

- **EG-01 No Surprises** — Visitors know exactly what's being tracked before any tracking occurs
- **EG-03 Calm Protection** — Declining cookies does not degrade the experience

## Design Decision: Analytics

> Before implementing this story, decide whether Camp Fit Fur Dogs needs
> analytics at all. Privacy-first alternatives:
>
> - **Plausible** — cookieless, GDPR-compliant, no consent banner needed ($9/mo or self-hosted free)
> - **Umami** — open source, self-hostable, cookieless
> - **No analytics** — a valid choice for a portfolio project
>
> If a cookieless analytics tool is chosen, this story reduces to a simple
> cookie policy page explaining session cookies only — no banner needed.

## Notes

- Depends on US-103 (Next.js frontend), US-156 (Privacy Policy)
- If Camp Fit Fur Dogs uses only session cookies and cookieless analytics, the banner is unnecessary
- GDPR cookie consent: required if any EU visitor accesses the site
- CalOPPA: requires disclosure of cookie usage in the privacy policy (covered by US-156)
- **Demo:** Visit the site for the first time — see the cookie banner, reject non-essential, verify analytics scripts did not load
