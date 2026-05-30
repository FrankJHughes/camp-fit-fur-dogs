# Changelog Governance

This document defines how changelog entries are created, maintained, and governed across all products.  
It complements (but does not duplicate) the Changelog Conventions in Workflow Conventions.

The changelog is a **user-facing artifact**. It communicates what changed, why it matters, and when it shipped.

---

# 1. Purpose of the Changelog

The changelog exists to:

- Provide a transparent history of user-visible changes  
- Communicate releases and milestones  
- Support debugging and regression analysis  
- Serve as a contract with users about what changed and when  
- Prevent drift between code, stories, and documentation  

The changelog is not a commit log. It is a curated, human-readable summary.

---

# 2. Changelog Structure

Each product maintains its own changelog:

- `CHANGELOG.md` for Camp Fit Fur Dogs  
- `CHANGELOG.md` for Frank (SharedKernel)  

Each changelog contains:

- An **[Unreleased]** section  
- A section per released version  
- Bullet points grouped by category (Added, Changed, Fixed, Removed)  

Example:

```
## [Unreleased]

### Added
- New booking flow (US-162)

### Fixed
- Corrected dog profile validation (US-030)
```

---

# 3. When to Add a Changelog Entry

A changelog entry is required when:

- A user-visible behavior changes  
- A new feature is added  
- A bug is fixed that users could encounter  
- A breaking change occurs  
- A milestone is completed  
- A security fix is applied  
- A story is marked “shipped” in `catalog.csv`  

A changelog entry is **not** required for:

- Internal refactors  
- Test-only changes  
- Documentation-only changes  
- CI-only changes  
- Developer tooling changes  

When in doubt, include an entry.

---

# 4. What a Changelog Entry Must Contain

Each entry must include:

- A short, clear description  
- The story ID (e.g., US-162)  
- The category (Added, Changed, Fixed, Removed)  
- The product it applies to (implicit by file location)  

Entries must **not** include:

- PR numbers  
- Commit hashes  
- Technical implementation details  
- Internal-only notes  

---

# 5. Multi-Product Changelog Governance

Camp Fit Fur Dogs and Frank (SharedKernel) are separate products.

Rules:

- Each product has its own changelog  
- A change affecting both products requires entries in both files  
- Cross-product changes must reference the same story ID  
- SharedKernel changes must never appear in the Camp Fit Fur Dogs changelog  

This prevents cross-product drift and maintains clear boundaries.

---

# 6. Release Governance

A release is created when:

- A milestone is completed  
- A significant feature set ships  
- A version bump is required  
- A deployment is made to production  

Release steps:

1. Move all items from **[Unreleased]** into a new version section  
2. Add a version number and date  
3. Ensure all entries reference story IDs  
4. Ensure no PR numbers appear  
5. Commit the changelog update as part of the release PR  

Versioning:

- Camp Fit Fur Dogs uses semantic versioning  
- Frank uses semantic versioning  
- Patch versions may be used for hotfixes  

---

# 7. Changelog and Story Governance Integration

When a story is marked “shipped”:

- A changelog entry must be added  
- The entry must reference the story ID  
- The milestone must be updated  
- The catalog must be updated  

Stories without changelog entries are considered incomplete.

---

# 8. Changelog and CI Governance Integration

CI enforces:

- Changelog updates for user-facing changes  
- No PR numbers in changelog entries  
- Correct placement under **[Unreleased]**  
- Correct file encoding (utf8NoBOM)  
- No drift between story metadata and changelog entries  

A PR that modifies user-visible behavior without updating the changelog must fail review.

---

# 9. Changelog Hygiene

The changelog must remain:

- **Accurate** — reflects real shipped behavior  
- **Concise** — no implementation details  
- **Consistent** — follows conventions  
- **Ordered** — newest entries first  
- **Readable** — clear, user-facing language  

Changelog drift is a governance violation.

---

# 10. Governance Enforcement

- Reviewers enforce changelog rules  
- Product Owner ensures user-facing accuracy  
- CI enforces formatting and placement  
- Conventions define structure; governance defines process  

No PR may merge if changelog governance is violated.

