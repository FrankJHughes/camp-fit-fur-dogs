# Local Development Runbook

Procedures for running the full stack locally.

## Backend

\\\ash
cd src/CampFitFurDogs.Api
dotnet restore
dotnet run
\\\

API runs at: http://localhost:5000

## Frontend

\\\ash
cd frontend
npm install
npm run dev
\\\

Frontend runs at: http://localhost:3000

## Database (Neon)

Local development uses the shared Neon branch.

To reset the DB:

\\\ash
dotnet ef database update
\\\

## Running Tests

### Backend
\\\ash
make backend-test
\\\

### Frontend
\\\ash
npm test
\\\

### SharedKernel
\\\ash
make shared-kernel-test
\\\