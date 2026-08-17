using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Oidc;
using Frank.Identity.Application.Callback.Oidc.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback.Oidc;

/// <summary>
/// Provides extension methods for registering all OIDC callback pipeline
/// components into the application's dependency injection container.
/// <para>
/// This includes the immutable context build steps:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="ExchangeCodeStep"/> — exchanges the authorization code for tokens</description></item>
/// <item><description><see cref="FetchUserInfoStep"/> — retrieves OIDC UserInfo claims</description></item>
/// <item><description><see cref="ValidateTokensStep"/> — validates the ID token and extracts claims</description></item>
/// </list>
/// <para>
/// As well as the <see cref="ICallbackOidcContextBuilder"/>, which orchestrates
/// the full OIDC callback pipeline.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all OIDC callback pipeline services, including the immutable
    /// context build steps and the <see cref="CallbackOidcContextBuilder"/>,
    /// into the provided <see cref="IServiceCollection"/>.
    /// <para>
    /// This method wires up the complete OIDC authentication callback pipeline,
    /// enabling the application to process authorization codes, validate tokens,
    /// retrieve UserInfo, and produce a fully enriched
    /// <see cref="CallbackOidcContextBuilderResult"/>.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the OIDC callback pipeline components
    /// will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, allowing for fluent
    /// chaining of registration calls.
    /// </returns>
    public static IServiceCollection AddFrankIdentityApplicationCallbackOidc(this IServiceCollection services)
    {
        return services
            // Pipeline steps
            .AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, ExchangeCodeStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, FetchUserInfoStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackOidcContext>, ValidateTokensStep>()

            // Pipeline builder
            .AddTransient<ICallbackOidcContextBuilder, CallbackOidcContextBuilder>();
    }
}
