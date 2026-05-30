# Form Architecture Guides

This folder contains documentation related to **frontend form architecture**, including validation, UI behavior, and slice‑specific form patterns.

These guides are **frontend‑specific** but intentionally live alongside backend developer guides to avoid splitting the documentation tree.

---

# Contents

- **[Create Account Form](../create-account-form.md)**  
  Detailed walkthrough of the Create Account form, including structure, UX rules, and integration with the FormCommand architecture.

- **[Form Validation Architecture](../form-validation-architecture.md)**  
  How Zod schemas, validator functions, and the form state machine work together to produce deterministic, unified validation.

---

# Purpose

These guides help developers:

- Build consistent, predictable forms  
- Apply the FormCommand + state machine architecture correctly  
- Understand how frontend and backend validation interact  
- Maintain UX and accessibility consistency across the app  
- Implement new form slices with confidence  

---

# Audit Checklist

When auditing this folder, verify:

- **Contents section is complete**  
  - Every file in this folder is listed under *Contents*  
  - Link text and filenames match the actual documents

- **Descriptions are accurate**  
  - Each entry’s one‑line summary reflects the current document

- **Cross‑links are valid**  
  - Links to related guides (validation, testing, slice walkthroughs) resolve correctly

- **Scope is correct**  
  - Documents here are *frontend form architecture*  
  - Not backend validation, not global architecture, not operations

- **Terminology is consistent**  
  - Uses the same vocabulary as FormCommand, state machine, and slice architecture docs

---

# Related Documentation

- [Validation Boundaries](ca://s?q=Show_validation_boundaries_doc)  
- [Frontend Testing](ca://s?q=Show_frontend_testing_doc)  
- [Feature Slice Walkthrough](ca://s?q=Show_feature_slice_walkthrough)  
- [FormCommand Architecture](ca://s?q=Show_form_command_architecture)
