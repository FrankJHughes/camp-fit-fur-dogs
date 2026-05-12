# Incident Response Runbook

Procedures for diagnosing and resolving outages.

## API Down

1. Check Render logs.
2. Verify DB connection string.
3. Hit \/api/health\.
4. Restart service in Render dashboard.

## Frontend Down

1. Check Vercel deployment logs.
2. Verify \NEXT_PUBLIC_API_URL\.
3. Redeploy from Vercel UI.

## Database Issues

1. Check Neon dashboard.
2. Verify branch exists.
3. Check connection string.
4. Recreate branch if needed.

## Preview Environment Broken

See \preview-recovery-runbook.md\.