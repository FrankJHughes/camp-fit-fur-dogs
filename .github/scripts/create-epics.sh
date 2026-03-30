#!/usr/bin/env bash
# create-epics.sh — Creates GitHub issues for epics.
#
# Usage:
#   ./create-epics.sh [--dry-run] <file.yml|directory/>
#
# Idempotent: skips files already stamped with 'epic.created'.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
source "$SCRIPT_DIR/repo.env"
source "$SCRIPT_DIR/lib.sh"

show_help() {
  echo "Usage: create-epics.sh [--dry-run] <file.yml|directory/>"
  echo ""
  echo "Creates GitHub issues for epics defined in YAML manifests."
  echo "Idempotent — skips files already stamped with 'epic.created'."
  echo ""
  echo "Options:"
  echo "  --dry-run   Show what would be created without making API calls"
  echo "  --help, -h  Show this help message"
  echo ""
  echo "Environment:"
  echo "  PROJECT_NUMBER  Board number (default: 14, from repo.env)"
  exit 0
}

create_epic() {
  local file="$1"
  CURRENT_FILE="$file"

  CURRENT_STEP="checking idempotency stamp"
  if yq -e '.epic.created' "$file" &>/dev/null; then
    echo "⏭ Skipping $(basename "$file") — already created"
    return 0
  fi

  CURRENT_STEP="reading manifest"
  local title description labels
  title=$(yq -r '.epic.title' "$file")
  description=$(yq -r '.epic.description // ""' "$file")
  labels=$(yq -r '(.epic.labels // []) | join(",")' "$file")

  echo "🔨 Creating epic: $title"

  [[ -n "$labels" ]] && ensure_labels "$labels"

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
    --body "$description" \
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
  yq -i ".epic.created = \"$now\" | .epic.issue_number = $issue_number" "$file"

  echo "✅ Created epic: $title → #$issue_number"
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
      [[ -f "$file" ]] && create_epic "$file"
    done
  elif [[ -f "$arg" ]]; then
    create_epic "$arg"
  else
    echo "⚠ Not found: $arg"
  fi
done
