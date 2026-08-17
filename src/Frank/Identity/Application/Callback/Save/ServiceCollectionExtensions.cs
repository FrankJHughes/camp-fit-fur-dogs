using Frank.Core.Application.Abstractions.ImmutableContexts;
using Frank.Identity.Application.Abstractions.Callback.Save;
using Frank.Identity.Application.Callback.Save.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Identity.Application.Callback.Save;

/// <summary>
/// Provides extension methods for registering all Save‑pipeline components into
/// the application's dependency injection container.
/// <para>
/// The Save pipeline runs after the OIDC callback pipeline has produced a
/// validated external identity context. Its responsibilities include:
/// resolving the internal user, generating a session token and cookie,
/// creating the authenticated session, and emitting audit logs.
/// </para>
/// <para>
/// This extension method wires up all immutable Save‑pipeline steps and the
/// <see cref="ICallbackSaveContextBuilder"/>, enabling the application to
/// execute the full Save pipeline through dependency injection.
/// </para>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Save‑pipeline services, including the immutable context
    /// build steps and the <see cref="CallbackSaveContextBuilder"/>, into the
    /// provided <see cref="IServiceCollection"/>.
    /// <para>
    /// The following steps are registered:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="AuditLoginStep"/> — emits login audit events</description></item>
    /// <item><description><see cref="BuildCookieStep"/> — generates session token + cookie</description></item>
    /// <item><description><see cref="CreateSessionStep"/> — persists authenticated session</description></item>
    /// <item><description><see cref="ResolveUserStep"/> — resolves internal user ID</description></item>
    /// </list>
    /// <para>
    /// All steps are registered as <c>AddTransient</c> to ensure each pipeline
    /// execution receives fresh instances.
    /// </para>
    /// </summary>
    /// <param name="services">
    /// The service collection to which the Save‑pipeline components will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, allowing for fluent
    /// chaining of registration calls.
    /// </returns>
    public static IServiceCollection AddFrankIdentityApplicationCallbackSave(this IServiceCollection services)
    {
        return services

            // Pipeline steps
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, AuditLoginStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, BuildCookieStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, CreateSessionStep>()
            .AddTransient<IImmutableContextBuildStep<CallbackSaveContext>, ResolveUserStep>()

            // Pipeline builder
            .AddTransient<ICallbackSaveContextBuilder, CallbackSaveContextBuilder>();
    }
}
