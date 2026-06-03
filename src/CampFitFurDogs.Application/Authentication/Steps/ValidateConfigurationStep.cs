using Microsoft.Extensions.Options;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Domain.Errors;
using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ValidateConfigurationStep : IAuthCallbackStep
{
    private readonly OidcOptions _options;

    public AuthCallbackStepMetadata Metadata =>
        new("ValidateConfiguration", "Validate Configuration");

    public ValidateConfigurationStep(IOptions<OidcOptions> options)
    {
        _options = options.Value;
    }

    public bool CanExecute(AuthCallbackContext ctx)
        => true; // Always runs

    public Task<AuthCallbackContext> ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.Authority) ||
            string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret) ||
            string.IsNullOrWhiteSpace(_options.CallbackUrl) ||
            string.IsNullOrWhiteSpace(_options.PostLoginRedirectUrl))
        {
            throw new BadConfigurationException("Auth0 configuration is incomplete");
        }

        return Task.FromResult(ctx);
    }
}
