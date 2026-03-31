#!/usr/bin/env bash
# create-sprint.sh — Creates a GitHub milestone for a sprint and assigns
# stories to the milestone and project board.
#
# Usage:
#   ./create-sprint.sh [--dry-run] <sprint-file.yml>
#
# Idempotent: skips if sprint.created is already stamped.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
source "$SCRIPT_DIR/repo.env"
source "$SCRIPT_DIR/lib.sh"

show_help() {
  echo "Usage: create-sprint.sh [--dry-run] <sprint-file.yml>"
  echo ""
  echo "Creates a GitHub milestone for a sprint, creates story issues,"
  echo "assigns them to the milestone, and adds them to the project board."
  echo "Idempotent — skips if sprint.created is already stamped."
  echo ""
  echo "Options:"
  echo "  --dry-run   Show what would be created without making API calls"
  echo "  --help, -h  Show this help message"
  echo ""
  echo "Environment:"
  echo "  PROJECT_NUMBER  Board number (default: 14, from repo.env)"
  exit 0
}

create_sprint() {
  local file="$1"
  CURRENT_FILE="$file"

  CURRENT_STEP="checking idempotency stamp"
  if yq -e '.sprint.created' "$file" &>/dev/null; then
    echo "⏭ Skipping $(basename "$file") — already created"
    return 0
  fi

  CURRENT_STEP="reading manifest"
  local name title end_date
  name=$(yq -r '.sprint.name' "$file")
  title=$(yq -r '.sprint.title // .sprint.name' "$file")
  end_date=$(yq -r '.sprint.end' "$file")

  echo "🏃 Creating sprint: $title"
  echo ""

  CURRENT_STEP="resolving project board"
  require_board
  echo "  📋 Board #$PROJECT_NUMBER resolved"

  if [[ "$DRY_RUN" == true ]]; then
    echo "  [dry-run] Would create milestone: $name (due $end_date)"

    local story_count
    story_count=$(yq -r '.stories | length' "$file")

    for i in $(seq 0 $((story_count - 1))); do
      local story_path
      story_path=$(yq -r ".stories[$i]" "$file")
      local full_path="$REPO_ROOT/$story_path"

      if [[ ! -f "$full_path" ]]; then
        echo "  [dry-run] ⚠ Story not found: $story_path"
        continue
      fi

      local story_title
      story_title=$(yq -r '.title' "$full_path")
      echo "  [dry-run] Would process story: $story_title"
    done

    echo ""
    echo "🏜 Dry run complete — $story_count stories would be processed"
    return 0
  fi

  CURRENT_STEP="creating milestone"
  echo "  📌 Creating milestone: $name"
  local milestone_number
  milestone_number=$(gh api "repos/$FULL_REPO/milestones" \
    -X POST \
    -f title="$name" \
    -f description="$title" \
    -f due_on="${end_date}T23:59:59Z" \
    --jq '.number')
  echo "  → Milestone #$milestone_number"

  echo ""
  local story_count
  story_count=$(yq -r '.stories | length' "$file")

  for i in $(seq 0 $((story_count - 1))); do
    local story_path
    story_path=$(yq -r ".stories[$i]" "$file")
    local full_path="$REPO_ROOT/$story_path"

    if [[ ! -f "$full_path" ]]; then
      echo "  ⚠ Story not found: $story_path — skipping"
      continue
    fi

    local story_title
    story_title=$(yq -r '.title' "$full_path")

    CURRENT_STEP="creating story: $story_title"
    if ! yq -e '.created' "$full_path" &>/dev/null; then
      "$SCRIPT_DIR/create-stories.sh" "$full_path"
    else
      echo "  ⏭ Story already exists: $story_title"
    fi

    local issue_number
    issue_number=$(yq -r '.issue_number' "$full_path")

    CURRENT_STEP="assigning #$issue_number to milestone"
    gh issue edit "$issue_number" \
      --repo "$FULL_REPO" \
      --milestone "$name" > /dev/null

    CURRENT_STEP="adding #$issue_number to board"
    add_to_board "$PROJECT_ID" "$issue_number"

    echo "  → #$issue_number assigned to milestone + board"
  done

  CURRENT_STEP="stamping manifest"
  local now
  now=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
  yq -i "
    .sprint.created = \"$now\"
    | .sprint.milestone = $milestone_number
  " "$file"

  echo ""
  echo "✅ Sprint created: $title"
  echo "   Milestone: #$milestone_number"
  echo "   Board:     #$PROJECT_NUMBER"
  echo "   Stories:   $story_count processed"
}

# --- Main ---
[[ "${1:-}" == "--help" || "${1:-}" == "-h" ]] && show_help

parse_flags "$@"
set -- "${REMAINING_ARGS[@]}"

if [[ $# -lt 1 ]]; then
  show_help
fi

create_sprint "$1"
