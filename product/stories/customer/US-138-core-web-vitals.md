---
id: US-138
title: "Core Web Vitals & Performance"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: false
dependencies:
  - US-103
---

# US-138: Core Web Vitals & Performance

## Intent

As a **potential customer**, I should experience fast page loads and smooth
interactions so that I don't abandon the site before discovering what Camp Fit
Fur Dogs offers.

## Value

Google uses Core Web Vitals (LCP, INP, CLS) as ranking signals. Slow sites rank
lower and lose visitors. For a service business competing locally, page speed
directly affects discoverability and conversion. Next.js provides strong defaults
but performance must be measured, monitored, and maintained.

## Acceptance Criteria

### Metrics (measured on mobile via Lighthouse)
- [ ] Largest Contentful Paint (LCP) under 2.5 seconds
- [ ] Interaction to Next Paint (INP) under 200 milliseconds
- [ ] Cumulative Layout Shift (CLS) under 0.1
- [ ] Lighthouse Performance score of 90 or higher on all public pages

### Implementation
- [ ] Images use Next.js `<Image>` component with automatic optimization, lazy loading, and proper `sizes` attributes
- [ ] Fonts use `next/font` with font-display swap to eliminate render-blocking
- [ ] Critical CSS is inlined; non-critical CSS is deferred
- [ ] JavaScript bundle size is monitored — no single page bundle exceeds a reasonable threshold
- [ ] Pages that can be statically generated use Static Site Generation (SSG) or Incremental Static Regeneration (ISR)

### Monitoring
- [ ] Core Web Vitals are measured in production via `web-vitals` library or Next.js built-in reporting
- [ ] Performance regression is detectable in CI (Lighthouse CI or similar)

## Emotional Guarantees

- **EG-01 No Surprises** — Pages load fast and don't jump around during rendering

## Notes

- Depends on US-103 (Next.js frontend) — shipped
- Next.js provides strong defaults (automatic code splitting, image optimization, font optimization)
- This story is about measuring, tuning, and maintaining — not rebuilding
- Consider: Lighthouse CI in GitHub Actions for automated performance regression detection
- **Demo:** Run Lighthouse on the home page — show green scores across all Core Web Vitals
