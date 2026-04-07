# US-XX — Documentation Architecture Consolidation

## Story ID
story.usXX.doc-architecture

## Parent Epic
epic.documentation-governance

## Intent
As a product owner,  
I want a unified documentation architecture that consolidates governance, workflows, reference specs, and runbooks,  
so that documentation remains coherent, durable, and emotionally safe as the system evolves.

## Problem
Documentation is currently fragmented across multiple files:
- governance rules
- workflow descriptions
- YAML specs
- runbooks
- contributor guidance
- CI rules

This fragmentation increases cognitive load, creates drift risk, and makes onboarding harder.

## Outcome
A single-source-of-truth documentation architecture with:
- clear folder structure
- index files
- no duplication
- cross-linked governance and workflows
- durable reference specs
- updated contributor guidance

## Acceptance Criteria
- A new documentation architecture is defined under docs/
- Governance, workflows, reference specs, and runbooks are reorganized into clear categories
- Duplicate content is removed
- Index files exist for each documentation category
- Internal links are updated
- Documentation architecture is described in a dedicated page
- Sprint planning, Story YAML lifecycle, and CI readiness rules reference governance instead of duplicating it

## Emotional Safety Guarantees
- Contributors can navigate documentation without confusion
- No single workflow requires reading multiple scattered files
- Documentation changes are predictable and non-fragile

