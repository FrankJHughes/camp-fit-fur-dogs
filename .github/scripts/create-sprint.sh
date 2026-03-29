#!/usr/bin/env bash
# create-sprint.sh — Creates a GitHub milestone for a sprint and assigns
#                     stories to the milestone and an existing project board.
#
# Usage:
#   ./create-sprint.sh sprints/sprint-0.yml
#
# What it does:
#   1. Creates a milestone with the sprint end date as due date
#   2. For each story listed in the manifest:
#      - Creates the story issue if not yet created (delegates to create-stories.sh)
#      - Assigns the issue to the milestone
#      - Adds the issue to the existing project board (read from sprint.board)
#
# Prerequisites:
#   - gh CLI authenticated (gh auth login)
#   - yq v4+ installed (https://github.com/mikefarah/yq)
#   - jq installed
#   - Project board must already exist (run create-board.sh first)
#   - sprint.board field must be set to the project number in the sprint YAML
#
# Idempotent: skips if sprint.created is already stamped.
# Re-running after adding stories to the manifest will NOT process new stories
# because the sprint-level stamp gates the entire run.
# For carry-over, list the story in the next sprint manifest — the story's
# own stamp prevents re-creation, and milestone reassignment is the goal.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
source "$SCRIPT_DIR/repo.env"

# --- Functions ---

# Resolve the project board's node ID from its number.
# The GraphQL addProjectV2ItemById mutation needs the node ID, not the number.
resolve_project_id() {
  local project_number="$1"
  gh api graphql -f query="
    query {
      user(login: \"$OWNER\") {
        projectV2(number: $project_number) {
          id
        }
      }
    }" --jq '.data.user.projectV2.id'
}

create_sprint() {
  local file="$1"

  # Idempotency check
  if yq -e '.sprint.created' "$file" &>/dev/null; then
    echo "⏭  Skipping $(basename "$file") — already created"
    return 0
  fi

  local name title end_date board_number
  name=$(yq -r '.sprint.name' "$file")
  title=$(yq -r '.sprint.title // .sprint.name' "$file")
  end_date=$(yq -r '.sprint.end' "$file")
  board_number=$(yq -r '.sprint.board' "$file")

  # Validate board number
  if [[ -z "$board_number" || "$board_number" == "null" ]]; then
    echo "❌ Error: sprint.board is not set in $file"
    echo "   Run create-board.sh first, then add 'board: <number>' to your sprint YAML."
    exit 1
  fi

  echo "🏃 Creating sprint: $title"
  echo ""

  # --- 1. Resolve Project Board Node ID ---
  echo "  📋 Looking up project board #$board_number"
  local project_id
  project_id=$(resolve_project_id "$board_number")

  if [[ -z "$project_id" || "$project_id" == "null" ]]; then
    echo "❌ Error: Could not find project board #$board_number"
    echo "   Verify the board exists: gh project view $board_number --owner $OWNER"
    exit 1
  fi
  echo "     → Board resolved"

  # --- 2. Create Milestone ---
  echo "  📌 Creating milestone: $name"
  local milestone_number
  milestone_number=$(gh api "repos/$FULL_REPO/milestones" \
    -X POST \
    -f title="$name" \
    -f description="$title" \
    -f due_on="${end_date}T23:59:59Z" \
    --jq '.number')
  echo "     → Milestone #$milestone_number"

  # --- 3. Process Stories ---
  echo ""
  local story_count
  story_count=$(yq -r '.stories | length' "$file")

  for i in $(seq 0 $((story_count - 1))); do
    local story_path
    story_path=$(yq -r ".stories[$i]" "$file")
    local full_path="$REPO_ROOT/$story_path"

    if [[ ! -f "$full_path" ]]; then
      echo "  ⚠  Story not found: $story_path — skipping"
      continue
    fi

    local story_title
    story_title=$(yq -r '.title' "$full_path")

    # Create story if not yet created
    if ! yq -e '.created' "$full_path" &>/dev/null; then
      "$SCRIPT_DIR/create-stories.sh" "$full_path"
    else
      echo "  ⏭  Story already exists: $story_title"
    fi

    local issue_number
    issue_number=$(yq -r '.issue_number' "$full_path")

    # Assign to milestone
    gh issue edit "$issue_number" \
      --repo "$FULL_REPO" \
      --milestone "$name" > /dev/null

    # Add to project board (idempotent — GitHub ignores duplicates)
    local issue_node_id
    issue_node_id=$(gh api "repos/$FULL_REPO/issues/$issue_number" --jq '.node_id')

    gh api graphql -f query="
      mutation {
        addProjectV2ItemById(input: {
          projectId: \"$project_id\"
          contentId: \"$issue_node_id\"
        }) {
          item { id }
        }
      }" --silent

    echo "     → #$issue_number assigned to milestone + board"
  done

  # --- 4. Stamp the sprint file ---
  local now
  now=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
  yq -i "
    .sprint.created = \"$now\" |
    .sprint.milestone = $milestone_number
  " "$file"

  echo ""
  echo "✅ Sprint created: $title"
  echo "   Milestone: #$milestone_number"
  echo "   Board:     #$board_number (pre-existing)"
  echo "   Stories:   $story_count processed"
}

# --- Main ---

if [[ $# -lt 1 ]]; then
  echo "Usage: create-sprint.sh <sprint-file.yml>"
  exit 1
fi

create_sprint "$1"
