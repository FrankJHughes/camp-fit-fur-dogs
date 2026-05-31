using Microsoft.Extensions.Options;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Domain.Errors;
using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Steps;

public sealed class ValidateConfigStep : IAuthCallbackStep
{
    private readonly OidcOptions _options;

    public ValidateConfigStep(IOptions<OidcOptions> options)
    {
        _options = options.Value;
    }

    public Task ExecuteAsync(AuthCallbackContext ctx, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.Authority) ||
            string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret) ||
            string.IsNullOrWhiteSpace(_options.CallbackUrl) ||
            string.IsNullOrWhiteSpace(_options.PostLoginRedirectUrl))
        {
            throw new BadConfigurationException("Auth0 configuration is incomplete");
        }

        return Task.CompletedTask;
    }
}
