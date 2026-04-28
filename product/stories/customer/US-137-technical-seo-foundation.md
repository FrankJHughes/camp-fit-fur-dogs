---
id: US-137
title: "Technical SEO Foundation"
epic: Customer
milestone: M2
status: backlog
domain: customer
vertical_slice: false
dependencies:
  - US-103
---

# US-137: Technical SEO Foundation

## Intent

As a **system operator**, I need the application to provide search engines with
a sitemap, robots directives, and structured data so that pages are crawled,
indexed, and displayed with rich search results.

## Value

Without a sitemap, search engines may miss pages or crawl inefficiently. Without
robots.txt, crawlers waste budget on irrelevant routes (API endpoints, auth pages).
Without structured data, search results show plain blue links instead of rich
cards with ratings, business info, and service details.

## Acceptance Criteria

### Sitemap
- [ ] Dynamic `sitemap.xml` generated via Next.js sitemap support
- [ ] Sitemap includes all public pages with accurate `lastmod` dates
- [ ] Sitemap excludes authenticated-only pages, API routes, and error pages
- [ ] Sitemap is registered in `robots.txt` and submitted to Google Search Console

### Robots.txt
- [ ] `robots.txt` allows crawling of public pages
- [ ] `robots.txt` disallows crawling of API endpoints, auth pages, and admin routes
- [ ] `robots.txt` references the sitemap URL

### Structured Data (JSON-LD)
- [ ] Home page includes `LocalBusiness` schema with business name, address, phone, hours
- [ ] Service pages include `Service` schema describing offered services
- [ ] Structured data validates with Google Rich Results Test
- [ ] JSON-LD is embedded in page `<head>` via Next.js script component

## Emotional Guarantees

- **EG-01 No Surprises** — Search engine results accurately represent what the site offers

## Notes

- Depends on US-103 (Next.js frontend) — shipped
- Next.js App Router supports `sitemap.ts` and `robots.ts` for dynamic generation
- Schema.org vocabulary reference: LocalBusiness, PetStore, AnimalShelter, or custom Service types
- Google Search Console enrollment is an operational step, not code
- **Demo:** Validate sitemap at `/sitemap.xml`, validate robots at `/robots.txt`, run Google Rich Results Test on the home page
