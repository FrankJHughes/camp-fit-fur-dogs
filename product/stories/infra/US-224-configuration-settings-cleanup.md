---
id: US-224
title: "Configuration Settings Cleanup & Capability Co‑location"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-183
  - US-184
  - US-220
  - US-223
---

# US‑224 — Configuration Settings Cleanup & Capability Co‑location

## Intent

As an **admin**, I must have all configuration settings defined, validated, and co‑located within the Frank capabilities that require them, so that configuration becomes predictable, discoverable, strongly typed, and impossible to break through string‑based typos or inconsistent naming.

This ensures the platform has a **single, authoritative configuration contract** for each capability and eliminates hard‑coded configuration keys scattered across the codebase.

---

## Problem

Configuration keys today are:

- hard‑coded as string literals across multiple layers  
- duplicated between capabilities, tests, and app code  
- inconsistent in naming and hierarchy  
- validated in some places but not others  
- not discoverable or documented  
- not co‑located with the capabilities that require them  

This leads to:

- silent failures when keys are misspelled  
- inconsistent configuration shapes  
- test harness instability  
- CI failures due to missing keys  
- difficulty onboarding new developers  
- inability to auto‑generate configuration documentation  

---

## Value

This cleanup provides:

- **Predictability** — every capability defines its own configuration contract  
- **Discoverability** — developers can browse configuration like an API  
- **Safety** — no more stringly‑typed configuration keys  
- **Testability** — tests reference the same constants as production  
- **Refactorability** — renaming a key becomes a single‑file change  
- **Documentation** — configuration can be auto‑generated from capability metadata  

---

## Acceptance Criteria

### **AC‑1 — Each Frank capability defines its own configuration keys**
- Keys must be defined in a `Configuration/` folder within the capability  
- Keys must follow the hierarchical naming convention:  
  `Frank:<Capability>:<Subsection>:<Key>`  
- Keys must be exposed as `public static class` constants  
- No capability may reference configuration keys defined outside itself  

### **AC‑2 — All settings classes bind using capability‑owned keys**
- All `.BindConfiguration(...)` calls must reference capability constants  
- No string literals may appear in configuration binding  

### **AC‑3 — All configuration lookups use capability constants**
- No direct string‑literal lookups (e.g., `config["Authentication:Callback:Oidc:Authority"]`)  
- All lookups must reference capability key constants  

### **AC‑4 — All tests use capability constants**
- Test harness overrides must reference the same constants  
- No test may use string‑literal configuration keys  

### **AC‑5 — All configuration keys are validated at startup**
- Each capability must validate required keys  
- Missing or invalid configuration must fail fast with a clear error  

### **AC‑6 — Remove all hard‑coded configuration strings from the codebase**
- Replace all string literals with capability constants  
- Remove duplicated or inconsistent key definitions  
- Remove unused or obsolete configuration keys  

### **AC‑7 — Provide a configuration manifest for documentation**
- Each capability must expose a list of required keys  
- Manifest must be auto‑generatable for docs  

---

## Emotional Guarantees

- **EG‑01 — Confident Foundations:** Developers can trust that configuration is consistent, validated, and discoverable.  
- **EG‑04 — No Surprises:** Missing or invalid configuration fails fast and predictably.  
- **EG‑05 — Clear Ownership:** Each capability owns its configuration contract, eliminating ambiguity.  

---

## Notes & Future Direction

- This story establishes the **Frank Configuration Contract Model**.  
- Future stories may introduce:
  - automated configuration documentation generation  
  - Roslyn analyzers preventing string‑literal configuration lookups  
  - capability‑level configuration manifests for tooling  
  - environment‑aware configuration validation  

