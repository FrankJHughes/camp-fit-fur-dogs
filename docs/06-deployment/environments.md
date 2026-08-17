# Environments

The application supports multiple deployment environments with distinct configurations.

## Development

- Database: LocalDB or SQL Server Express  
- Identity: Auth0 dev tenant  
- Logging: Debug and Information  
- CORS: `http://localhost:3000`  
- Features: All enabled  

Setup:

```bash
ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

## Testing

- Database: In-memory or test database  
- Identity: Stubbed/mocked  
- Logging: Warnings only  
- CORS: Test URLs only  
- Features: Disabled/mocked as needed  

Run tests:

```bash
dotnet test --configuration Debug
```

## Production

- Database: Managed PostgreSQL (Render, AWS RDS, etc.)  
- Identity: Auth0 production tenant  
- Logging: Information and Errors only  
- CORS: Frontend domain only  
- Features: All enabled, hardened  

Deploy:

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet run
```

## Environment detection

ASP.NET Core automatically reads `ASPNETCORE_ENVIRONMENT`:

```csharp
if (environment.IsProduction())
{
    // Production-only configuration
}
```

Hosting modules can also detect custom environments:

```csharp
if (builder.Environment.IsEnvironment("RenderPrPreview"))
{
    // Render preview-specific configuration
}
```
