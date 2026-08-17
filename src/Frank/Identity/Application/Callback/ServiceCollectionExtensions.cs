using Frank.Identity.Application.Callback.Oidc;
using Frank.Identity.Application.Callback.Save;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback;

/// <summary>
/// Provides extension methods for registering all authentication callback
/// pipelines—both OIDC and Save—into the application's dependency injection
/// container.
/// <para>
/// This extension method is the top‑level entry point for wiring up the entire
/// authentication callback subsystem. It composes:
/// </para>
/// <list type="bullet">
/// <item><description>The OIDC callback pipeline (authorization code → external identity)</description></item>
/// <item><description>The Save pipeline (external identity → internal session)</description></item>
/// </list>
/// <para>
/// Together, these pipelines form the complete authentication callback flow,
/// transforming an incoming OIDC authorization code into a fully persisted,
/// auditable, authenticated session.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the full authentication callback subsystem—including both the
    /// OIDC pipeline and the Save pipeline—into the provided
    /// <see cref="IServiceCollection"/>.
    /// <para>
    /// This method is a convenience aggregator that calls:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="Oidc.ServiceCollectionExtensions.AddFrankIdentityApplicationCallbackOidc"/></description></item>
    /// <item><description><see cref="Save.ServiceCollectionExtensions.AddFrankIdentityApplicationCallbackSave"/></description></item>
    /// </list>
    /// <para>
    /// After registration, the application can resolve:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="ICallbackOidcContextBuilder"/></description></item>
    /// <item><description><see cref="ICallbackSaveContextBuilder"/></description></item>
    /// </list>
    /// <para>
    /// enabling execution of the full authentication callback pipeline through DI.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the authentication callback pipelines will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, allowing for fluent
    /// chaining of registration calls.
    /// </returns>
    public static IServiceCollection AddFrankIdentityApplicationCallback(this IServiceCollection services)
    {
        return services

            // Register OIDC callback pipeline
            .AddFrankIdentityApplicationCallbackOidc()

            // Register Save pipeline
            .AddFrankIdentityApplicationCallbackSave();
    }
}
