using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Save;

/// <summary>
/// Defines the contract for constructing a <see cref="CallbackSaveContext"/> from
/// upstream identity data and application‑level save‑phase inputs.
/// <para>
/// The save phase of the OIDC callback pipeline is responsible for resolving or
/// creating a local user, establishing a session, generating a cookie value, and
/// determining the final redirect URL.
/// This builder encapsulates that multi‑step process and produces:
/// </para>
/// <list type="number">
/// <item><description>
/// A <see cref="CallbackSaveContextBuilderRequest"/> containing upstream identity
/// information, an optional redirect request, and a clock‑captured timestamp.
/// </description></item>
/// <item><description>
/// A <see cref="CallbackSaveContextBuilderResult"/> containing the resolved user,
/// created session, token hash, and cookie value.
/// </description></item>
/// <item><description>
/// A final immutable <see cref="CallbackSaveContext"/> used by the Identity
/// application pipeline to complete the callback flow.
/// </description></item>
/// </list>
/// </summary>
/// <remarks>
/// This interface extends
/// <see cref="IImmutableContextBuilder{TRequest,TContext,TResult}"/>, ensuring
/// that the save‑phase logic adheres to the deterministic, immutable‑context
/// construction pattern used throughout the Identity subsystem.
/// <para>
/// Implementations are responsible for all application‑level behavior required to
/// complete the callback, including user resolution, session creation, token
/// hashing, cookie generation, and redirect determination.
/// </para>
/// </remarks>
public interface ICallbackSaveContextBuilder : IImmutableContextBuilder<
    CallbackSaveContextBuilderRequest,
    CallbackSaveContext,
    CallbackSaveContextBuilderResult>
{ }
