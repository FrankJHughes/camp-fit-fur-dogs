#!/usr/bin/env bash
# create-stories.sh — Creates a GitHub issue from a story YAML file.
#
# Usage:
#   ./create-stories.sh [--dry-run] <story-file.yml> [<story-file.yml> ...]
#
# When no files are given, processes all *.yml / *.yaml in $STORIES_DIR.
#
# Idempotent: skips stories that already have .issue_number stamped.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/repo.env"
source "$SCRIPT_DIR/lib.sh"

show_help() {
  echo "Usage: create-stories.sh [--dry-run] <story-file.yml> [...]"
  echo ""
  echo "Creates GitHub issues from story YAML files."
  echo "Idempotent — skips if .issue_number is already stamped."
  echo ""
  echo "Options:"
  echo "  --dry-run   Show what would be created without making API calls"
  echo "  --help, -h  Show this help message"
  echo ""
  echo "Environment:"
  echo "  MILESTONE        Milestone name to assign (optional)"
  echo "  PROJECT_NUMBER   Board number (default: from repo.env)"
  exit 0
}

[[ "${1:-}" == "--help" || "${1:-}" == "-h" ]] && show_help

parse_flags "$@"
set -- "${REMAINING_ARGS[@]}"

MILESTONE="${MILESTONE:-}"

# ── Schema 1 body (infra stories with inline fields) ──────────────

build_body() {
  local file="$1"
  local body=""

  local desc
  desc=$(yq -r '.description // ""' "$file")
  [[ -n "$desc" ]] && body+="$desc"$'\n\n'

  local epic
  epic=$(yq -r '.epic // ""' "$file")
  [[ -n "$epic" ]] && body+="**Epic:** $epic"$'\n\n'

  local task_count
  task_count=$(yq '.tasks | length' "$file")
  if [[ "$task_count" -gt 0 ]]; then
    body+="## Tasks"$'\n'
    for i in $(seq 0 $((task_count - 1))); do
      body+="- [ ] $(yq -r ".tasks[$i]" "$file")"$'\n'
    done
    body+=$'\n'
  fi

  local ac_count
  ac_count=$(yq '.acceptance | length' "$file")
  if [[ "$ac_count" -gt 0 ]]; then
    body+="## Acceptance Criteria"$'\n'
    for i in $(seq 0 $((ac_count - 1))); do
      body+="- [ ] $(yq -r ".acceptance[$i]" "$file")"$'\n'
    done
    body+=$'\n'
  fi

  echo "$body"
}

# ── Schema 2 body (customer/edge stories → product markdown) ──────

build_body_v2() {
  local file="$1"
  local body=""

  local story_id type capability egs dor product_file
  story_id=$(yq -r '.id // ""' "$file")
  type=$(yq -r '.type // ""' "$file")
  capability=$(yq -r '.capability.primary // ""' "$file")
  egs=$(yq -r '(.emotionalGuarantees // []) | join(", ")' "$file")
  dor=$(yq -r '.definitionOfReady.verified // false' "$file")
  product_file=$(yq -r '.source.productFile // ""' "$file")

  body+="**Story ID:** $story_id"$'\n'
  body+="**Type:** $type"$'\n'
  body+="**Capability:** $capability"$'\n'
  [[ -n "$egs" ]] && body+="**Emotional Guarantees:** $egs"$'\n'
  body+="**DoR Verified:** $([ "$dor" == "true" ] && echo "✅" || echo "❌")"$'\n'
  body+=$'\n---\n\n'

  if [[ -n "$product_file" ]]; then
    local rel_path="${product_file#/}"
    if [[ -f "$rel_path" ]]; then
      body+="$(cat "$rel_path")"
      body+=$'\n\n---\n\n'
      body+="**Product Spec:** \`$rel_path\`"$'\n'
    else
      echo "  ⚠️  Product file not found: $rel_path" >&2
      body+="_Product spec not found: \`$rel_path\`_"$'\n'
    fi
  fi

  body+="**Planning YAML:** \`$file\`"$'\n'
  echo "$body"
}

# ── Schema 2 labels (derived from .type) ──────────────────────────

derive_labels_v2() {
  local file="$1"
  yq -r '.type // ""' "$file"
}

# ── Issue creation (unified, both schemas) ────────────────────────

create_issue() {
  local file="$1"

  CURRENT_FILE="$file"
  CURRENT_STEP="reading YAML"

  local title
  title=$(yq -r '.title' "$file")

  # Idempotency — skip if issue_number already stamped
  local existing
  existing=$(yq -r '.issue_number // ""' "$file")
  if [[ -n "$existing" && "$existing" != "null" ]]; then
    echo "  ⏭  Skipping '$title' — already #$existing"
    return 0
  fi

  # Detect schema and build accordingly
  local schema body labels
  schema=$(detect_schema "$file")

  CURRENT_STEP="building body (schema $schema)"
  if [[ "$schema" == "2" ]]; then
    body=$(build_body_v2 "$file")
    labels=$(derive_labels_v2 "$file")
    local sid
    sid=$(yq -r '.id // ""' "$file")
    [[ -n "$sid" && "$sid" != "null" ]] && title="$sid: $title"
  else
    body=$(build_body "$file")
    labels=$(yq -r '(.labels // []) | join(",")' "$file")
  fi

  # Body validation
  if [[ -z "$body" || ${#body} -lt 10 ]]; then
    echo "  ❌ FAILED: Empty body for '$title' (schema $schema)"
    echo "     File: $file"
    exit 1
  fi

  if [[ "$DRY_RUN" == true ]]; then
    echo "  [dry-run] Would create: $title (schema $schema)"
    [[ -n "$labels" ]] && echo "    Labels: $labels"
    [[ -n "$MILESTONE" ]] && echo "    Milestone: $MILESTONE"
    echo "    Body: ${#body} chars"
    return 0
  fi

  CURRENT_STEP="ensuring labels"
  if [[ -n "$labels" ]]; then
    ensure_labels "$labels"
  fi

  CURRENT_STEP="creating issue"
  local args=("--repo" "$FULL_REPO" "--title" "$title" "--body" "$body")
  [[ -n "$labels" ]] && args+=("--label" "$labels")
  [[ -n "$MILESTONE" ]] && args+=("--milestone" "$MILESTONE")

  local url
  url=$(gh issue create "${args[@]}")
  local num
  num=$(echo "$url" | grep -oE '[0-9]+$')

  echo "  ✅ Created #$num: $title"

  CURRENT_STEP="stamping YAML"
  yq -i ".created = \"$(date -u +%Y-%m-%dT%H:%M:%SZ)\"" "$file"
  yq -i ".issue_number = $num" "$file"

  if [[ -n "${PROJECT_ID:-}" ]]; then
    CURRENT_STEP="adding to board"
    add_to_board "$PROJECT_ID" "$num"
    echo "  📋 Added to board"
  fi
}

# ── Main ──────────────────────────────────────────────────────────

require_board

if [[ $# -gt 0 ]]; then
  for file in "$@"; do
    [[ -f "$file" ]] && create_issue "$file" || echo "⚠️  Not found: $file"
  done
else
  for file in "$STORIES_DIR"/*.yml "$STORIES_DIR"/*.yaml; do
    [[ -f "$file" ]] || continue
    create_issue "$file"
  done
fi