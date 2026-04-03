# Validate Planning Workflow

## Intent
Add a CI gate that validates planning artifact integrity — ensuring every story YAML has a matching product spec, required fields are present, and sprint manifests reference only existing files.

## Value
Catches planning drift at PR time rather than at sprint standup, enforcing the governance rule that every story is grounded in a product spec.

## Acceptance Criteria
- [ ] CI workflow validates all planning YAMLs have required fields per their schema
- [ ] Every YAML with source.productFile is checked for file existence
- [ ] Sprint manifest entries reference existing story files
- [ ] Validation failures block merge with clear error messages

## Emotional Guarantees: N/A
