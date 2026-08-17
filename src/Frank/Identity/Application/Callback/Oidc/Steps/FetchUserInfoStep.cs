using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Abstractions.Oidc;

namespace Frank.Identity.Application.Callback.Oidc.Steps;

/// <summary>
/// Represents the OIDC pipeline step responsible for retrieving UserInfo from the
/// identity provider using the access token obtained during the authorization code
/// exchange.
/// <para>
/// This step executes only when <see cref="CallbackOidcContext.AccessToken"/> is
/// present. It calls the configured <see cref="IOidcUserInfoClient"/> to fetch
/// standard OIDC UserInfo claims and enriches the immutable callback context with
/// the returned values.
/// </para>
/// <para>
/// The step is deterministic and side‑effect free aside from the outbound
/// UserInfo request. It returns a new <see cref="CallbackOidcContext"/> instance
/// containing the enriched claim data.
/// </para>
/// </summary>
public sealed class FetchUserInfoStep : IImmutableContextBuildStep<CallbackOidcContext>
{
    private readonly IOidcUserInfoClient _client;

    /// <summary>
    /// Metadata describing this build step, including its unique identifier and
    /// human‑readable description. Used by pipeline diagnostics and observability.
    /// </summary>
    public IImmutableContextBuildStepMetadata Metadata { get; } =
        new ImmutableContextBuildStepMetadata("oidc.fetch-userinfo", "Fetch OIDC UserInfo");

    /// <summary>
    /// Creates a new <see cref="FetchUserInfoStep"/> using the provided OIDC
    /// UserInfo client.
    /// </summary>
    /// <param name="client">
    /// The OIDC UserInfo client responsible for retrieving user claims from the
    /// identity provider.
    /// </param>
    public FetchUserInfoStep(IOidcUserInfoClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Determines whether this step can execute based on the current callback
    /// context.
    /// <para>
    /// This step executes only when an access token is present, since the UserInfo
    /// endpoint requires a valid bearer token.
    /// </para>
    /// </summary>
    /// <param name="ctx">The current immutable callback context.</param>
    /// <returns>
    /// <c>true</c> if <see cref="CallbackOidcContext.AccessToken"/> is not null;
    /// otherwise <c>false</c>.
    /// </returns>
    public bool CanExecute(CallbackOidcContext ctx)
        => ctx.AccessToken is not null;

    /// <summary>
    /// Fetches OIDC UserInfo using the access token and enriches the callback
    /// context with the returned claims.
    /// <para>
    /// The returned context is a new immutable instance containing:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Subject identifier</description></item>
    /// <item><description>Raw claim dictionary</description></item>
    /// <item><description>Email</description></item>
    /// <item><description>Given name</description></item>
    /// <item><description>Family name</description></item>
    /// <item><description>Profile picture URL</description></item>
    /// </list>
    /// </summary>
    /// <param name="ctx">The current immutable callback context.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    /// A new <see cref="CallbackOidcContext"/> enriched with UserInfo claims.
    /// </returns>
    public async Task<CallbackOidcContext> ExecuteAsync(
        CallbackOidcContext ctx,
        CancellationToken ct)
    {
        var info = await _client.GetUserInfoAsync(ctx.AccessToken!, ct);

        return ctx with
        {
            SubjectId = info.Subject,
            Claims = info.Claims,
            Email = info.Email,
            GivenName = info.GivenName,
            FamilyName = info.FamilyName,
            Picture = info.Picture
        };
    }
}
