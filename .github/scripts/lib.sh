#!/usr/bin/env bash
# lib.sh — Shared functions for all creation engines.
# Sourced by each script after repo.env.

# --- Dependency Check ---

require_tools() {
  local missing=()
  for cmd in gh yq jq; do
    command -v "$cmd" &>/dev/null || missing+=("$cmd")
  done
  if [[ ${#missing[@]} -gt 0 ]]; then
    echo "❌ Missing required tools: ${missing[*]}"
    echo "  Install them before running creation engines."
    exit 1
  fi
}

require_tools

# --- Dry-Run Support ---

DRY_RUN=false

parse_flags() {
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --dry-run) DRY_RUN=true; shift ;;
      *) break ;;
    esac
  done
  REMAINING_ARGS=("$@")

  if [[ "$DRY_RUN" == true ]]; then
    echo "🏜 Dry run — no issues or milestones will be created"
    echo ""
  fi
}

# --- Error Trap ---

CURRENT_FILE=""
CURRENT_STEP=""

trap 'echo ""; echo "💥 Failed during: $CURRENT_STEP"; echo "   File: $CURRENT_FILE"; exit 1' ERR

# --- Board Functions ---

# Resolve the project board's node ID from its number.
resolve_project_id() {
  local project_number="$1"
  gh api graphql -f query="
    query {
      user(login: \"$OWNER\") {
        projectV2(number: $project_number) { id }
      }
    }" --jq '.data.user.projectV2.id'
}

# Add an issue to the project board. Idempotent.
add_to_board() {
  local project_id="$1"
  local issue_number="$2"

  local issue_node_id
  issue_node_id=$(gh api "repos/$FULL_REPO/issues/$issue_number" --jq '.node_id')

  gh api graphql -f query="
    mutation {
      addProjectV2ItemById(input: {
        projectId: \"$project_id\"
        contentId: \"$issue_node_id\"
      }) { item { id } }
    }" --silent
}

# Resolve and validate the project board. Sets PROJECT_ID in caller scope.
require_board() {
  CURRENT_STEP="resolving project board"

  if [[ -z "${PROJECT_NUMBER:-}" ]]; then
    echo "❌ Error: PROJECT_NUMBER is not set."
    echo "  Set it in repo.env or export PROJECT_NUMBER=<number>"
    exit 1
  fi

  PROJECT_ID=$(resolve_project_id "$PROJECT_NUMBER")

  if [[ -z "$PROJECT_ID" || "$PROJECT_ID" == "null" ]]; then
    echo "❌ Error: Could not find project board #$PROJECT_NUMBER"
    echo "  Verify: gh project view $PROJECT_NUMBER --owner $OWNER"
    exit 1
  fi
}

# --- Label Functions ---

# Auto-create labels that don't yet exist in the repo.
ensure_labels() {
  local labels_csv="$1"
  [[ -z "$labels_csv" ]] && return 0

  IFS=',' read -ra label_array <<< "$labels_csv"
  for label in "${label_array[@]}"; do
    label=$(echo "$label" | xargs)
    [[ -z "$label" ]] && continue
    if [[ "$DRY_RUN" == true ]]; then
      echo "  [dry-run] Would ensure label: $label"
    elif gh label create "$label" --repo "$FULL_REPO" 2>/dev/null; then
      echo "  🏷 Created label: $label"
    fi
  done
}

# ── Schema detection ──────────────────────────────────────────────
# Returns "1" for infra stories (have inline .description).
# Returns "2" for customer/edge stories (no .description, use product spec).
detect_schema() {
  local file="$1"
  if yq -e '.description' "$file" &>/dev/null; then
    echo "1"
  else
    echo "2"
  fi
}
