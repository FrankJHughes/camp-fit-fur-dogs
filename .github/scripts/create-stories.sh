#!/usr/bin/env bash
# create-stories.sh — Creates GitHub issues for stories.
#
# Usage:
#   ./create-stories.sh [--dry-run] <file.yml|directory/>
#
# Idempotent: skips files already stamped with 'created'.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
source "$SCRIPT_DIR/repo.env"
source "$SCRIPT_DIR/lib.sh"

show_help() {
  echo "Usage: create-stories.sh [--dry-run] <file.yml|directory/>"
  echo ""
  echo "Creates GitHub issues for stories defined in YAML manifests."
  echo "Idempotent — skips files already stamped with 'created'."
  echo ""
  echo "Options:"
  echo "  --dry-run   Show what would be created without making API calls"
  echo "  --help, -h  Show this help message"
  echo ""
  echo "Environment:"
  echo "  PROJECT_NUMBER  Board number (default: 14, from repo.env)"
  exit 0
}

# Search local epic YAMLs for an epic's issue number by title.
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

build_body() {
  local file="$1"
  local body=""

  local description
  description=$(yq -r '.description // ""' "$file")
  [[ -n "$description" ]] && body+="$description"$'\n'

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

  local task_count
  task_count=$(yq -r '.tasks | length' "$file")
  if [[ "$task_count" -gt 0 ]]; then
    body+=$'\n'"## Tasks"$'\n'
    for i in $(seq 0 $((task_count - 1))); do
      body+="- [ ] $(yq -r ".tasks[$i]" "$file")"$'\n'
    done
  fi

  local ac_count
  ac_count=$(yq -r '.acceptance | length' "$file")
  if [[ "$ac_count" -gt 0 ]]; then
    body+=$'\n'"## Acceptance Criteria"$'\n'
    for i in $(seq 0 $((ac_count - 1))); do
      body+="- [ ] $(yq -r ".acceptance[$i]" "$file")"$'\n'
    done
  fi

  echo "$body"
}

create_story() {
  local file="$1"
  CURRENT_FILE="$file"

  CURRENT_STEP="checking idempotency stamp"
  if yq -e '.created' "$file" &>/dev/null; then
    echo "⏭ Skipping $(basename "$file") — already created"
    return 0
  fi

  CURRENT_STEP="reading manifest"
  local title labels
  title=$(yq -r '.title' "$file")
  labels=$(yq -r '(.labels // []) | join(",")' "$file")

  echo "🔨 Creating story: $title"

  [[ -n "$labels" ]] && ensure_labels "$labels"

  CURRENT_STEP="building issue body"
  local body
  body=$(build_body "$file")

  if [[ "$DRY_RUN" == true ]]; then
    echo "  [dry-run] Would create issue: $title"
    echo "  [dry-run] Would add to board #$PROJECT_NUMBER"
    return 0
  fi

  CURRENT_STEP="creating GitHub issue"
  local -a label_args=()
  [[ -n "$labels" ]] && label_args=(--label "$labels")

  local issue_url
  issue_url=$(gh issue create \
    --repo "$FULL_REPO" \
    --title "$title" \
    --body "$body" \
    "${label_args[@]}")

  local issue_number
  issue_number=$(basename "$issue_url")

  CURRENT_STEP="adding to project board"
  require_board
  add_to_board "$PROJECT_ID" "$issue_number"
  echo "  📋 Added to board #$PROJECT_NUMBER"

  CURRENT_STEP="stamping manifest"
  local now
  now=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
  yq -i ".created = \"$now\" | .issue_number = $issue_number" "$file"

  echo "✅ Created story: $title → #$issue_number"
}

# --- Main ---
[[ "${1:-}" == "--help" || "${1:-}" == "-h" ]] && show_help

parse_flags "$@"
set -- "${REMAINING_ARGS[@]}"

if [[ $# -lt 1 ]]; then
  show_help
fi

for arg in "$@"; do
  if [[ -d "$arg" ]]; then
    for file in "$arg"/*.yml; do
      [[ -f "$file" ]] && create_story "$file"
    done
  elif [[ -f "$arg" ]]; then
    create_story "$arg"
  else
    echo "⚠ Not found: $arg"
  fi
done
