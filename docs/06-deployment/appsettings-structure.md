# AppSettings Structure

Configuration is organized by subsystem and environment in `appsettings*.json` files.

## Root structure

```json
{
  "Logging": { ... },
  "AllowedHosts": "*",
  "ConnectionStrings": { ... },
  "Frontend": { ... },
  "Identity": { ... },
  "Email": { ... },
  "RateLimiting": { ... }
}
```

## Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Frank.Core": "Debug"
    }
  }
}
```

## Database

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=campfitfurdogs;Trusted_Connection=true;"
  }
}
```

## Identity (Auth0)

```json
{
  "Identity": {
    "Oidc": {
      "Authority": "https://dev-f73yf4vyecgf51qh.us.auth0.com",
      "ClientId": "8BxEHo6DLRpG7Bo8sNlMjunQYKlippPH",
      "ClientSecret": "YOUR_CLIENT_SECRET",
      "CallbackUrl": "https://app.example.com/identity/callback",
      "Disabled": "false"
    },
    "Session": {
      "Ttl": "7.00:00:00"
    }
  }
}
```

## Frontend

```json
{
  "Frontend": {
    "BaseUrl": "https://app.example.com"
  }
}
```

## Email

```json
{
  "Email": {
    "Provider": "SendGrid",
    "FromAddress": "noreply@example.com",
    "SendGridApiKey": "SG.xxxxx"
  }
}
```

## Environment overrides

Each environment can override base settings:

- `appsettings.Development.json` — local development  
- `appsettings.Testing.json` — integration tests  
- `appsettings.Production.json` — production (if tracked in repo)

Best practice: store production secrets in secure vaults and inject them via environment variables.
