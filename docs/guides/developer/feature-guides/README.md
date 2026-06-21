# Feature Guides  
**Aligned With Vertical Slice Architecture & Exclusive OIDC Authentication**

This folder contains **feature‑level walkthroughs** for *vertical slices* — domain use cases that cut through all layers (API → Application → Domain → Infrastructure → Frontend).

These guides explain *how a specific slice works today*, not system‑wide architecture.

Authentication, session management, and other cross‑cutting concerns are **horizontals**, not slices, and therefore do **not** appear in this folder.

---

# Contents

- **[Feature Slice Walkthrough](../feature-slice-walkthrough.md)**  
  End‑to‑end walkthrough of a vertical slice.

- **[Abstractions Contract](../abstractions-contract.md)**  
  How commands, queries, DTOs, and interfaces define the Application layer’s public API.

- **[Create Customer Slice](../create-customer-slice.md)**  
  Vertical slice for onboarding a new Owner.  
  Triggered by the OIDC callback (not a form).  
  Domain behavior: Owner aggregate creation.

- **[Register Dog Slice](../register-dog-slice.md)**  
  Vertical slice for registering a dog.

- **[Edit Dog Profile Slice](../edit-dog-profile-slice.md)**  
  Vertical slice for editing a dog’s profile.

- **[Get Dog Profile Slice](../get-dog-profile-slice.md)**  
  Vertical slice for querying a dog’s profile.

- **[List Dogs by Owner Slice](../list-dogs-by-owner-slice.md)**  
  Vertical slice for listing all dogs belonging to the authenticated Owner.

- **[Remove Dog Slice](../remove-dog-slice.md)**  
  Vertical slice for removing a dog.

---

# Purpose

These guides help developers:

- Understand how a vertical slice is structured  
- Learn by example  
- Apply architectural patterns to real features  
- Navigate slice‑level abstractions  
- Follow TDD red–green–refactor across layers  

Vertical slices represent **domain use cases**, not cross‑cutting concerns.

---

# Audit Checklist

When auditing this folder, verify:

- **Contents section is complete**  
  - All implemented vertical slices are listed.  
  - No horizontals (authentication, session, CORS, security headers, etc.) appear here.  
  - No deprecated slices remain.

- **Descriptions are accurate**  
  - Each entry’s one‑line description reflects the current implementation.

- **Cross‑links are valid**  
  - Links to ADRs, architecture, and testing docs resolve correctly.

- **Scope is correct**  
  - Documents here describe *vertical slices only*.  
  - Horizontals belong in architecture or operations docs, not here.

---

# Related Documentation

- [Folder Structure](ca://s?q=Show_folder_structure_guide)  
- [Dispatcher Pipeline](ca://s?q=Show_dispatcher_pipeline_guide)  
- [Vertical Slice Index](ca://s?q=Show_vertical_slice_index)  
