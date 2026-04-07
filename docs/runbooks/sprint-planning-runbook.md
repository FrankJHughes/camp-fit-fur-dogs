# Sprint Planning Runbook

## Overview
Sprint planning selects stories for the sprint and prepares them for execution.  
Story Markdown is the backlog artifact; Story YAML is the sprint-planning artifact.

## Workflow

### 1. Select Stories
- Stories may or may not have YAML.
- YAML is optional at this stage.
- Evaluate stories based on product intent and readiness.

### 2. Add Selected Stories to Sprint YAML
Adding a story ID to the sprint YAML indicates it is being considered for the sprint.

### 3. Generate Missing Story YAML
A planning tool detects missing Story YAML files and generates them from Markdown.

### 4. Refine Story YAML
Update:
- story_points
- dependencies
- readiness
- sprint assignment

### 5. Validate Sprint Readiness
A sprint cannot be Ready unless:
- All stories have YAML
- All YAML is complete
- All dependencies are resolved

