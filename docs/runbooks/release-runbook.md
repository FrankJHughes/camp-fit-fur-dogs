# Release Runbook

## 1. Prepare Release Notes

Update \CHANGELOG.md\:

- Added
- Changed
- Fixed
- Removed

## 2. Tag the Release

\\\ash
git tag -a vX.Y.Z -m "Release vX.Y.Z"
git push origin vX.Y.Z
\\\

## 3. Deploy

Render + Vercel deploy automatically on tag.

## 4. Verify

- API healthy
- Frontend loads
- DB migrations applied