---
id: US-155
title: "Web Accessibility (WCAG 2.1 AA)"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: false
dependencies:
  - US-103
  - US-126
---

# US-155: Web Accessibility (WCAG 2.1 AA)

## Intent

As a **person with a disability**, I need Camp Fit Fur Dogs to be fully usable
with assistive technology so that I can register, manage my dogs, and use the
service independently.

## Value

Web accessibility is both an ethical imperative and a legal requirement.
California's Unruh Civil Rights Act allows statutory damages of $4,000+ per
violation per visit — and courts do not require proof of intent. Over 4,600
ADA web accessibility lawsuits were filed in US federal courts in 2024 alone,
with businesses of all sizes targeted. WCAG 2.1 Level AA is the de facto legal
standard referenced in virtually every Title III lawsuit.

Beyond compliance, accessible design improves usability for everyone — better
keyboard navigation, clearer form labels, stronger color contrast, and
semantic HTML all benefit every user.

## Acceptance Criteria

### Perceivable
- [ ] All images have meaningful `alt` text (decorative images use `alt=""`)
- [ ] All form inputs have associated `<label>` elements (not just placeholder text)
- [ ] Color is never the sole means of conveying information (e.g., error states include text + icon, not just red)
- [ ] Text meets minimum contrast ratios (4.5:1 for body text, 3:1 for large text)
- [ ] Video content (if any) has captions; audio content has transcripts

### Operable
- [ ] All interactive elements are reachable and operable via keyboard alone (Tab, Enter, Escape, Arrow keys)
- [ ] Focus order follows a logical reading sequence
- [ ] Focus indicator is visible on all interactive elements (no `outline: none` without a replacement)
- [ ] No keyboard traps — users can navigate away from any element
- [ ] Skip navigation link is present on every page ("Skip to main content")
- [ ] No content flashes more than 3 times per second

### Understandable
- [ ] Page language is declared (`<html lang="en">`)
- [ ] Form validation errors are announced by screen readers and describe what went wrong
- [ ] Error messages appear adjacent to the field, not only in a summary at the top
- [ ] Navigation is consistent across pages
- [ ] Labels and instructions are clear and visible before interaction

### Robust
- [ ] HTML is valid and uses semantic elements (`<nav>`, `<main>`, `<header>`, `<footer>`, `<section>`)
- [ ] ARIA attributes are used correctly when semantic HTML is insufficient
- [ ] Components work with major screen readers (NVDA, VoiceOver, JAWS)
- [ ] Next.js `<Image>` components always include `alt` attributes
- [ ] Dynamic content updates (client-side navigation, toasts, modals) are announced to screen readers via ARIA live regions

### Process
- [ ] Automated accessibility audit (axe-core or Lighthouse) runs in CI and blocks on violations
- [ ] Manual keyboard-only walkthrough of all user flows passes
- [ ] Screen reader walkthrough of registration and dog management flows passes
- [ ] Accessibility audit score of 90+ in Lighthouse

## Emotional Guarantees

- **EG-01 No Surprises** — Every user, regardless of ability, has the same experience quality
- **EG-07 Behaves Consistently** — Accessibility is designed in, not bolted on

## Legal Context

| Law | Scope | Risk |
|-----|-------|------|
| **ADA Title III** | Private businesses with public websites | Lawsuits for inaccessible sites; WCAG 2.1 AA is the benchmark |
| **California Unruh Act** | All California businesses | $4,000+ statutory damages per violation per visit — no intent required |
| **DOJ Title II Rule** | Government entities (not directly applicable) | Solidifies WCAG 2.1 AA as the legal standard nationally |

## Notes

- Depends on US-103 (Next.js frontend) — shipped
- Accessibility must be treated as a cross-cutting concern, not a one-time fix
- Next.js has built-in accessibility features: `<Link>` component handles focus, `eslint-plugin-jsx-a11y` for linting
- Consider: automated testing with `@axe-core/react` or `jest-axe` in unit tests
- Accessibility overlays/widgets are NOT compliant — they do not fix underlying code issues
- **Demo:** Navigate the entire registration and dog management flow using only a keyboard and VoiceOver — complete every task without a mouse
