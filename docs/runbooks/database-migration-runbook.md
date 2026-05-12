# Database Migration Runbook

## Add a migration

\\\ash
cd src/CampFitFurDogs.Infrastructure
dotnet ef migrations add <Name>
\\\

## Apply migrations locally

\\\ash
dotnet ef database update
\\\

## Apply migrations in preview

Preview environments apply migrations automatically during \deploy_fresh_db\.

## Apply migrations in production

Handled by Render during deployment.

## Rollback

\\\ash
dotnet ef database update <PreviousMigration>
\\\