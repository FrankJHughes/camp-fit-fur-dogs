# Story Scaffold Script

## Intent
A script exists that generates a matched pair of product spec and story
YAML from a small number of inputs (title, epic, points, priority, labels)
— so that hand-authoring YAML is never required and schema compliance is
guaranteed by construction.

## Value
Every story created by hand has risked schema mismatch — wrong fields,
missing source.productFile, inconsistent formatting. A scaffold script
makes the correct shape the only shape, eliminates copy-paste drift, and
cuts story creation time from minutes to seconds.

## Acceptance Criteria
- [ ] A script exists at a discoverable location (e.g., scripts/ or tools/)
- [ ] Script accepts inputs: title, epic, points, priority, labels, type (infra or customer), destination (backlog or sprint-N)
- [ ] Script generates a product spec markdown file in the correct product/stories/ subdirectory
- [ ] Script generates a story YAML in the correct planning/stories/ subdirectory
- [ ] Generated YAML matches the schema of existing stories on main (all required fields present)
- [ ] Generated YAML includes source.productFile pointing to the generated product spec
- [ ] Script is idempotent — re-running with the same title overwrites cleanly
- [ ] Script prints the paths of both generated files on completion
- [ ] README or planning/README.md documents how to use the script

## Out of Scope
- GitHub issue creation (covered by Post-Merge Sprint Bootstrap story)
- Schema validation (covered by Validate Planning Workflow story)

## Emotional Guarantees: N/A
