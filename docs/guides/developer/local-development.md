## Running the API Locally vs Hosted (US‑140)

### Local Development
The API can be run locally using:

```
dotnet run --project src/CampFitFurDogs.Api
```

Local configuration is loaded from:
- `appsettings.json`
- `appsettings.Development.json`
- User secrets (if configured)

### Hosted Environment (Render)
The hosted API runs inside a Dockerized .NET 10 container with:

- Environment variables injected by Render
- Neon as the backing PostgreSQL database
- HTTPS termination handled by Render
- Health check endpoint at `/health`

### Behavioral Differences
- **Cold starts** occur on Render’s free tier after 15 minutes of inactivity  
  (20–60 seconds typical)
- **CORS** is configured to allow the deployed frontend host (US‑139)
- **Connection strings** are never stored locally in production config

Local and hosted environments share the same code paths and EF Core configuration.
