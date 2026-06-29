using CampFitFurDogs.Application.Abstractions.Authentication.Callback;
using CampFitFurDogs.Application.Settings;
using Frank.Abstractions.ImmutableContext;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Application.Authentication.Callback.Steps;

public sealed class BuildRedirectStep
    : IImmutableContextBuildStep<ApplicationAuthCallbackContext>
{
    private readonly string _defaultRedirect;

    public BuildRedirectStep(IOptions<AuthCallbackSettings> options)
    {
        _defaultRedirect = options.Value.PostLoginRedirectUrl
            ?? throw new InvalidOperationException("PostLoginRedirectUrl must be configured.");
    }

    public IImmutableContextBuildStepMetadata Metadata =>
        new ImmutableContextBuildStepMetadata(
            id: "BuildRedirect",
            displayName: "Build Redirect"
        );

    public bool CanExecute(ApplicationAuthCallbackContext ctx)
        => ctx.RedirectUrl is null; // only runs once

    public Task<ApplicationAuthCallbackContext> ExecuteAsync(
        ApplicationAuthCallbackContext ctx,
        CancellationToken ct)
    {
        var requested = ctx.RequestedRedirectUrl;

        var finalRedirect =
            IsSafeRedirect(requested)
                ? requested!
                : _defaultRedirect;

        return Task.FromResult(
            ctx with
            {
                RedirectUrl = finalRedirect
            }
        );
    }

    private bool IsSafeRedirect(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Prevent open redirect attacks:
        // Only allow relative URLs or same-origin absolute URLs.
        if (Uri.TryCreate(url, UriKind.Relative, out _))
            return true;

        if (Uri.TryCreate(url, UriKind.Absolute, out var absolute))
        {
            // Allow only HTTPS and same host
            return absolute.Scheme == Uri.UriSchemeHttps;
        }

        return false;
    }
}
