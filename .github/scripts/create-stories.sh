#!/usr/bin/env bash
# create-stories.sh — Creates GitHub issues for stories defined in YAML files.
#
# Usage:
#   ./create-stories.sh <file.yml>          # single story
#   ./create-stories.sh <directory/>        # all stories in directory
#   ./create-stories.sh stories/*.yml       # glob
#
# Prerequisites:
#   - gh CLI authenticated (gh auth login)
#   - yq v4+ installed (https://github.com/mikefarah/yq)
#   - Run create-epics.sh first if stories reference epics
#
# Idempotent: skips files already stamped with 'created'.
# Labels are auto-created if they don't already exist in the repo.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
source "$SCRIPT_DIR/repo.env"

# --- Functions ---

# Auto-create labels that don't yet exist in the repo.
# Accepts a comma-separated label string (as produced by yq join).
ensure_labels() {
  local labels_csv="$1"
  [[ -z "$labels_csv" ]] && return 0

  IFS=',' read -ra label_array <<< "$labels_csv"
  for label in "${label_array[@]}"; do
    label=$(echo "$label" | xargs)  # trim whitespace
    [[ -z "$label" ]] && continue
    if gh label create "$label" --repo "$FULL_REPO" 2>/dev/null; then
      echo "  🏷  Created label: $label"
    fi
  done
}

# Search local epic YAML files to find the GitHub issue number for an epic title.
# Returns the issue number or empty string if not found/not yet created.
find_epic_issue() {
  local epic_title="$1"
  local epics_dir="$REPO_ROOT/$EPICS_DIR"

  for f in "$epics_dir"/*.yml; do
    [[ -f "$f" ]] || continue
    local title
    title=$(yq -r '.epic.title' "$f")
    if [[ "$title" == "$epic_title" ]]; then
      yq -r '.epic.issue_number // ""' "$f"
      return 0
    fi
  done
  echo ""
}

# Build the GitHub issue body from YAML fields:
#   - description text
#   - epic cross-reference (if epic exists)
#   - tasks as checkboxes
#   - acceptance criteria as checkboxes
build_body() {
  local file="$1"
  local body=""

  # Description
  local description
  description=$(yq -r '.description // ""' "$file")
  if [[ -n "$description" ]]; then
    body+="$description"
    body+=$'\n'
  fi

  # Epic cross-reference
  local epic_title
  epic_title=$(yq -r '.epic // ""' "$file")
  if [[ -n "$epic_title" ]]; then
    local epic_number
    epic_number=$(find_epic_issue "$epic_title")
    if [[ -n "$epic_number" ]]; then
      body+=$'\n'"Part of #${epic_number}"$'\n'
    else
      body+=$'\n'"Epic: ${epic_title} *(not yet created)*"$'\n'
    fi
  fi

  # Tasks as checkboxes
  local task_count
  task_count=$(yq -r '.tasks | length' "$file")
  if [[ "$task_count" -gt 0 ]]; then
    body+=$'\n'"## Tasks"$'\n'
    for i in $(seq 0 $((task_count - 1))); do
      local task
      task=$(yq -r ".tasks[$i]" "$file")
      body+="- [ ] $task"$'\n'
    done
  fi

  # Acceptance criteria as checkboxes
  local ac_count
  ac_count=$(yq -r '.acceptance | length' "$file")
  if [[ "$ac_count" -gt 0 ]]; then
    body+=$'\n'"## Acceptance Criteria"$'\n'
    for i in $(seq 0 $((ac_count - 1))); do
      local criterion
      criterion=$(yq -r ".acceptance[$i]" "$file")
      body+="- [ ] $criterion"$'\n'
    done
  fi

  echo "$body"
}

create_story() {
  local file="$1"

  # Idempotency check
  if yq -e '.created' "$file" &>/dev/null; then
    echo "⏭  Skipping $(basename "$file") — already created"
    return 0
  fi

  local title labels
  title=$(yq -r '.title' "$file")
  labels=$(yq -r '(.labels // []) | join(",")' "$file")

  echo "🔨 Creating story: $title"

  # Ensure labels exist
  [[ -n "$labels" ]] && ensure_labels "$labels"

  # Build issue body
  local body
  body=$(build_body "$file")

  # Create the issue
  local issue_url
  issue_url=$(gh issue create \
    --repo "$FULL_REPO" \
    --title "$title" \
    --body "$body" \
    ${labels:+--label "$labels"})

  local issue_number
  issue_number=$(basename "$issue_url")

  # Stamp the file
  local now
  now=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
  yq -i ".created = \"$now\" | .issue_number = $issue_number" "$file"

  echo "✅ Created story: $title → #$issue_number"
}

# --- Main ---

if [[ $# -lt 1 ]]; then
  echo "Usage: create-stories.sh <file.yml|directory/>"
  exit 1
fi

for arg in "$@"; do
  if [[ -d "$arg" ]]; then
    for file in "$arg"/*.yml; do
      [[ -f "$file" ]] && create_story "$file"
    done
  elif [[ -f "$arg" ]]; then
    create_story "$arg"
  else
    echo "⚠  Not found: $arg"
  fi
done
