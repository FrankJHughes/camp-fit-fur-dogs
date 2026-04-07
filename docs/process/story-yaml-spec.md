# Story YAML Specification

## Purpose
Story YAML is the sprint-planning artifact containing planning metadata.

## Structure
story:
  id: ST-001
  name: Title from Markdown
  sprint: sprint-2
  status: candidate | ready | in-progress | done
  story_points: null
  dependencies: []
  ready: false

## Rules
- YAML may exist early but is not encouraged.
- YAML is required for sprint readiness.
- YAML is generated when a story is added to sprint YAML.

