#!/usr/bin/env bash
# bootstrap-sprint.sh — One-command sprint standup.
#
# Usage:
#   ./bootstrap-sprint.sh [--dry-run] [--clean] <sprint-file.yml>
#
# Sequence:
#   1. (Optional --clean) Strip idempotency stamps from story YAMLs
#   2. Create milestone + issues + board assignments (delegates to create-sprint.sh)
#   3. Verify: milestone exists, issue count matches, board items confirmed
#
# Idempotent: safe to re-run — existing items are skipped.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
source "$SCRIPT_DIR/repo.env"
source "$SCRIPT_DIR/lib.sh"

CLEAN=false

show_help() {
    echo "Usage: bootstrap-sprint.sh [--dry-run] [--clean] <sprint-file.yml>"
    echo ""
    echo "Stands up a sprint end-to-end: milestone, issues, board, verification."
    echo "Idempotent — safe to re-run at any point."
    echo ""
    echo "Options:"
    echo "  --dry-run  Show what would happen without making API calls"
    echo "  --clean    Strip idempotency stamps before running (forces re-creation)"
    echo "  --help, -h Show this help message"
    echo ""
    echo "Environment:"
    echo "  PROJECT_NUMBER  Board number (default: 14, from repo.env)"
    exit 0
}

# ── Step 1: Clean stamps ─────────────────────────────────────────
clean_stamps() {
    local file="$1"
    local story_count
    story_count=$(yq -r '.stories | length' "$file")

    echo "🧹 Cleaning idempotency stamps ..."

    yq -i 'del(.sprint.board_synced) | del(.sprint.milestone)' "$file"
    echo "  ✓ Sprint manifest stamps removed"

    for i in $(seq 0 $((story_count - 1))); do
        local story_path
        story_path=$(yq -r ".stories[$i]" "$file")
        local full_path="$REPO_ROOT/$story_path"

        if [[ -f "$full_path" ]]; then
            yq -i 'del(.issue_number) | del(.created)' "$full_path"
            echo "  ✓ $(basename "$full_path")"
        fi
    done
    echo ""
}

# ── Step 3: Verify ───────────────────────────────────────────────
verify_sprint() {
    local file="$1"
    local name story_count
    name=$(yq -r '.sprint.name' "$file")
    story_count=$(yq -r '.stories | length' "$file")

    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "🔍 Verifying sprint: $name"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo ""

    local pass=0
    local fail=0

    # ── Check 1: Milestone exists ──
    local milestone_number
    milestone_number=$(gh api "repos/$FULL_REPO/milestones" \
        --jq ".[] | select(.title == \"$name\") | .number")

    if [[ -n "$milestone_number" ]]; then
        local open_issues
        open_issues=$(gh api "repos/$FULL_REPO/milestones/$milestone_number" \
            --jq '.open_issues')
        echo "  ✅ Milestone #$milestone_number exists ($open_issues open issues)"
        ((pass++))
    else
        echo "  ❌ Milestone '$name' not found"
        ((fail++))
    fi

    # ── Check 2: All stories have issue numbers ──
    local stamped=0
    local missing_list=()
    for i in $(seq 0 $((story_count - 1))); do
        local story_path
        story_path=$(yq -r ".stories[$i]" "$file")
        local full_path="$REPO_ROOT/$story_path"

        if [[ -f "$full_path" ]]; then
            local issue_num
            issue_num=$(yq -r '.issue_number // ""' "$full_path")
            if [[ -n "$issue_num" && "$issue_num" != "null" ]]; then
                ((stamped++))
            else
                missing_list+=("$(basename "$full_path")")
            fi
        fi
    done

    if [[ ${#missing_list[@]} -eq 0 ]]; then
        echo "  ✅ All $stamped/$story_count stories have issue numbers"
        ((pass++))
    else
        echo "  ❌ ${#missing_list[@]}/$story_count stories missing issue numbers:"
        for m in "${missing_list[@]}"; do
            echo "     ⚠  $m"
        done
        ((fail++))
    fi

    # ── Check 3: Sprint manifest is stamped ──
    local board_synced
    board_synced=$(yq -r '.sprint.board_synced // ""' "$file")
    if [[ -n "$board_synced" && "$board_synced" != "null" ]]; then
        echo "  ✅ Sprint manifest stamped (board_synced: $board_synced)"
        ((pass++))
    else
        echo "  ❌ Sprint manifest not stamped with board_synced"
        ((fail++))
    fi

    # ── Check 4: Board item count ──
    if [[ -n "${PROJECT_ID:-}" ]]; then
        local board_count
        board_count=$(gh api graphql -f query="
            query {
                node(id: \"$PROJECT_ID\") {
                    ... on ProjectV2 {
                        items(first: 100) {
                            totalCount
                        }
                    }
                }
            }" --jq '.data.node.items.totalCount')
        echo "  ✅ Board has $board_count total items"
        ((pass++))
    fi

    # ── Check 5: Issues assigned to milestone ──
    if [[ -n "$milestone_number" ]]; then
        local milestone_issues
        milestone_issues=$(gh api "repos/$FULL_REPO/issues?milestone=$milestone_number&state=all&per_page=100" \
            --jq 'length')
        if [[ "$milestone_issues" -ge "$story_count" ]]; then
            echo "  ✅ Milestone has $milestone_issues issues (expected ≥$story_count)"
            ((pass++))
        else
            echo "  ❌ Milestone has $milestone_issues issues (expected ≥$story_count)"
            ((fail++))
        fi
    fi

    echo ""
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    if [[ "$fail" -eq 0 ]]; then
        echo "✅ Verification passed ($pass checks)"
    else
        echo "❌ Verification failed ($fail failures, $pass passed)"
    fi
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

    return "$fail"
}

# ── Main ─────────────────────────────────────────────────────────
[[ "${1:-}" == "--help" || "${1:-}" == "-h" ]] && show_help

# Parse --clean before parse_flags (which handles --dry-run)
ARGS=()
for arg in "$@"; do
    if [[ "$arg" == "--clean" ]]; then
        CLEAN=true
    else
        ARGS+=("$arg")
    fi
done

parse_flags "${ARGS[@]}"
set -- "${REMAINING_ARGS[@]}"

if [[ $# -lt 1 ]]; then
    show_help
fi

SPRINT_FILE="$1"

if [[ ! -f "$SPRINT_FILE" ]]; then
    echo "❌ Sprint file not found: $SPRINT_FILE"
    exit 1
fi

echo "🏁 Sprint Bootstrap"
echo "   File:  $SPRINT_FILE"
echo "   Board: #$PROJECT_NUMBER"
echo ""

# Step 1: Optional clean
if [[ "$CLEAN" == true ]]; then
    if [[ "$DRY_RUN" == true ]]; then
        echo "[dry-run] Would clean stamps from manifest and stories"
        echo ""
    else
        clean_stamps "$SPRINT_FILE"
    fi
fi

# Step 2: Create milestone + issues + board
echo "📋 Running create-sprint.sh ..."
echo ""
if [[ "$DRY_RUN" == true ]]; then
    "$SCRIPT_DIR/create-sprint.sh" --dry-run "$SPRINT_FILE"
else
    "$SCRIPT_DIR/create-sprint.sh" "$SPRINT_FILE"
fi

# Step 3: Verify
if [[ "$DRY_RUN" == true ]]; then
    echo ""
    echo "[dry-run] Would verify: milestone, issue stamps, board items"
else
    require_board
    verify_sprint "$SPRINT_FILE"
fi