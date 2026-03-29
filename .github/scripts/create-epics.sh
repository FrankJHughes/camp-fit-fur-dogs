#!/usr/bin/env bash
# create-epics.sh — Creates GitHub issues for epics defined in YAML files.
#
# Usage:
#   ./create-epics.sh <file.yml>          # single epic
#   ./create-epics.sh <directory/>        # all epics in directory
#   ./create-epics.sh epics/*.yml         # glob
#
# Prerequisites:
#   - gh CLI authenticated (gh auth login)
#   - yq v4+ installed (https://github.com/mikefarah/yq)
#
# Idempotent: skips files already stamped with 'epic.created'.
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

create_epic() {
  local file="$1"

  # Idempotency check
  if yq -e '.epic.created' "$file" &>/dev/null; then
    echo "⏭  Skipping $(basename "$file") — already created"
    return 0
  fi

  local title description labels
  title=$(yq -r '.epic.title' "$file")
  description=$(yq -r '.epic.description // ""' "$file")
  labels=$(yq -r '(.epic.labels // []) | join(",")' "$file")

  echo "🔨 Creating epic: $title"

  # Ensure labels exist
  [[ -n "$labels" ]] && ensure_labels "$labels"

  # Create the issue
  local issue_url
  issue_url=$(gh issue create \
    --repo "$FULL_REPO" \
    --title "$title" \
    --body "$description" \
    ${labels:+--label "$labels"})

  local issue_number
  issue_number=$(basename "$issue_url")

  # Stamp the file
  local now
  now=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
  yq -i ".epic.created = \"$now\" | .epic.issue_number = $issue_number" "$file"

  echo "✅ Created epic: $title → #$issue_number"
}

# --- Main ---

if [[ $# -lt 1 ]]; then
  echo "Usage: create-epics.sh <file.yml|directory/>"
  exit 1
fi

for arg in "$@"; do
  if [[ -d "$arg" ]]; then
    for file in "$arg"/*.yml; do
      [[ -f "$file" ]] && create_epic "$file"
    done
  elif [[ -f "$arg" ]]; then
    create_epic "$arg"
  else
    echo "⚠  Not found: $arg"
  fi
done
