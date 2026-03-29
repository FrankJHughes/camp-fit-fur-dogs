#!/usr/bin/env bash
# create-board.sh — Creates a GitHub Projects V2 board for the repo.
#
# Usage:
#   ./create-board.sh --title "Camp Fit Fur Dogs"
#
# What it does:
#   1. Creates a GitHub Projects V2 board owned by OWNER
#   2. Prints the project number so you can add it to sprint YAML files
#
# Prerequisites:
#   - gh CLI authenticated (gh auth login)
#   - OWNER in repo.env must match your GitHub user or org
#
# This script is designed to run ONCE per project. The board persists
# across sprints — each sprint's stories flow through it, filtered by milestone.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "$SCRIPT_DIR/repo.env"

# --- Defaults ---
BOARD_TITLE=""

# --- Parse Args ---
while [[ $# -gt 0 ]]; do
  case "$1" in
    --title)
      BOARD_TITLE="$2"
      shift 2
      ;;
    -h|--help)
      echo "Usage: create-board.sh --title \"Board Title\""
      echo ""
      echo "Creates a GitHub Projects V2 board for $FULL_REPO."
      echo "Run once per project. Add the printed project number to your sprint YAML files."
      exit 0
      ;;
    *)
      echo "Unknown option: $1"
      echo "Usage: create-board.sh --title \"Board Title\""
      exit 1
      ;;
  esac
done

if [[ -z "$BOARD_TITLE" ]]; then
  echo "Error: --title is required"
  echo "Usage: create-board.sh --title \"Board Title\""
  exit 1
fi

# --- Create Board ---
echo "📋 Creating project board: $BOARD_TITLE"

project_json=$(gh project create \
  --title "$BOARD_TITLE" \
  --owner "$OWNER" \
  --format json)

project_number=$(echo "$project_json" | jq -r '.number')
project_url=$(echo "$project_json" | jq -r '.url')

echo ""
echo "✅ Board created!"
echo "   Title:   $BOARD_TITLE"
echo "   Number:  $project_number"
echo "   URL:     $project_url"
echo ""
echo "👉 Add this to your sprint YAML files:"
echo "   board: $project_number"
echo ""
echo "💡 Configure columns (Backlog, To Do, In Progress, Review, Done)"
echo "   in the GitHub Projects UI — the V2 API has limited column support."
