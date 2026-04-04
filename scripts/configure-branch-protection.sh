#!/usr/bin/env bash
set -euo pipefail
# configure-branch-protection.sh
# Applies branch-protection rules to main via GitHub API.
# Requires: gh CLI authenticated with repo admin scope.
#
# NOTE: The status-check context must match the check name shown on PRs.
# If the workflow has a top-level `name:` field, the context may be
# "Workflow Name / Build & Test".  Adjust the "contexts" array below
# if the check name doesn't match.

OWNER="${GITHUB_REPOSITORY_OWNER:-FrankJHughes}"
REPO="${GITHUB_REPOSITORY_NAME:-camp-fit-fur-dogs}"
BRANCH="main"

echo "Applying branch protection to ${OWNER}/${REPO}:${BRANCH} ..."

gh api \
  --method PUT \
  "repos/${OWNER}/${REPO}/branches/${BRANCH}/protection" \
  --input - <<'EOF'
{
  "required_status_checks": {
    "strict": true,
    "contexts": ["Build & Test"]
  },
  "enforce_admins": false,
  "required_pull_request_reviews": {
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": true,
    "required_approving_review_count": 1
  },
  "restrictions": null,
  "allow_force_pushes": false,
  "allow_deletions": false
}
EOF

echo ""
echo "Branch protection applied.  Verifying ..."
echo ""

gh api "repos/${OWNER}/${REPO}/branches/${BRANCH}/protection" \
  --jq '{
    required_status_checks:  .required_status_checks.contexts,
    strict:                  .required_status_checks.strict,
    required_reviewers:      .required_pull_request_reviews.required_approving_review_count,
    dismiss_stale:           .required_pull_request_reviews.dismiss_stale_reviews,
    codeowners_required:     .required_pull_request_reviews.require_code_owner_reviews,
    enforce_admins:          .enforce_admins.enabled,
    allow_force_push:        .allow_force_pushes.enabled,
    allow_deletions:         .allow_deletions.enabled
  }'

echo ""
echo "Done.  Review the JSON above to confirm all settings."