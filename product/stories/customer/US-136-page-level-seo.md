---
id: US-136
title: "Page-Level SEO & Social Sharing"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: false
dependencies:
  - US-103
---

# US-136: Page-Level SEO & Social Sharing

## Intent

As a **potential customer**, I should see a clear, compelling preview when Camp
Fit Fur Dogs appears in search results or when someone shares a link on social
media, so that I understand what the service offers before clicking.

## Value

Search engines and social platforms use meta tags, Open Graph tags, and Twitter
Card tags to generate rich previews. Without them, shared links show generic
titles and blank thumbnails — reducing click-through rates and making the site
look unfinished. This is the single highest-impact SEO investment for a
service-based business.

## Acceptance Criteria

- [ ] Every page has a unique, descriptive `<title>` tag
- [ ] Every page has a unique `<meta name="description">` summarizing the page content
- [ ] Every page has a canonical URL (`<link rel="canonical">`) to prevent duplicate content penalties
- [ ] Open Graph tags (`og:title`, `og:description`, `og:image`, `og:url`, `og:type`) are present on all pages
- [ ] Twitter Card tags (`twitter:card`, `twitter:title`, `twitter:description`, `twitter:image`) are present on all pages
- [ ] Social sharing images are properly sized (1200x630 for OG, minimum 800x418 for Twitter)
- [ ] Next.js `metadata` API (or `generateMetadata` for dynamic pages) is used for all meta tag generation
- [ ] Shared links on Facebook, Twitter/X, LinkedIn, and iMessage render with rich previews
- [ ] Meta tags are validated with Facebook Sharing Debugger and Twitter Card Validator

## Emotional Guarantees

- **EG-01 No Surprises** — Every page has a thoughtful, accurate preview — no broken images or missing titles

## Notes

- Depends on US-103 (Next.js frontend) — shipped
- Next.js App Router has built-in metadata support via `export const metadata` and `generateMetadata()`
- Start with static pages (home, about, services), extend to dynamic pages (dog profiles) later
- **Demo:** Share the home page URL on a social platform — show the rich preview with title, description, and image
