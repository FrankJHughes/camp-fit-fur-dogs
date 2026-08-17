# Secrets Management
Sensitive values (API keys, database passwords, client secrets) must be secured.
## Never commit secrets
- Never commit \.env\ files
- Never commit \ppsettings.Production.json\ with real secrets
- Never commit OAuth2 client secrets
- Never commit database passwords
Add to \.gitignore\:
\\\
*.local.json
.env
secrets/
\\\
## Development
Use dotnet user secrets:
\\\ash
dotnet user-secrets init
dotnet user-secrets set "Identity:Oidc:ClientSecret" "dev-secret"
dotnet run
\\\
Secrets are stored per-user, never in version control.
## Testing
Inject test secrets via environment variables:
\\\ash
export Identity__Oidc__ClientSecret="test-secret"
dotnet test
\\\
## Production
Use platform-specific secret management:
### Render.com
Set environment variables in project dashboard:
- \ConnectionStrings__DefaultConnection\ ΓÇö database URL
- \Identity__Oidc__ClientSecret\ ΓÇö OAuth2 secret
- \Email__SendGridApiKey\ ΓÇö email API key
### AWS
Use AWS Secrets Manager:
\\\ash
aws secretsmanager create-secret --name prod/db-password --secret-string "..."
\\\
Reference in environment:
\\\ash
ConnectionStrings__DefaultConnection=
\\\
### Azure
Use Azure Key Vault:
\\\ash
az keyvault secret set --vault-name prod --name db-password --value "..."
\\\
## Rotation
- Rotate secrets regularly (every 90 days)
- Use short expiration times when possible
- Monitor secret access logs
- Alert on unauthorized access attempts
