using Frank.Core.Application.Abstractions.ImmutableContexts;

namespace Frank.Identity.Application.Abstractions.Callback.Oidc;

/// <summary>
/// Defines the contract for building a <see cref="CallbackOidcContext"/> from
/// upstream OIDC callback inputs.
/// <para>
/// An OIDC callback involves multiple stages: receiving the authorization code,
/// exchanging it for tokens, extracting identity claims, calling the UserInfo
/// endpoint, and normalizing provider‑specific metadata.
/// This builder encapsulates that multi‑step process and produces:
/// </para>
/// <list type="number">
/// <item><description>
/// A <see cref="CallbackOidcContextBuilderRequest"/> containing the minimal
/// upstream inputs (e.g., the authorization code).
/// </description></item>
/// <item><description>
/// A <see cref="CallbackOidcContextBuilderResult"/> containing normalized
/// identity information extracted from tokens and UserInfo.
/// </description></item>
/// <item><description>
/// A final immutable <see cref="CallbackOidcContext"/> used by the Identity
/// application pipeline.
/// </description></item>
/// </list>
/// </summary>
/// <remarks>
/// This interface extends <see cref="IImmutableContextBuilder{TRequest,TContext,TResult}"/>,
/// ensuring that the OIDC callback flow adheres to the immutable‑context
/// construction pattern used throughout the Identity subsystem.
/// <para>
/// Implementations are responsible for all protocol‑level interactions with the
/// upstream OIDC provider, including token exchange, claim extraction, and
/// provider‑specific normalization.
/// </para>
/// </remarks>
public interface ICallbackOidcContextBuilder : IImmutableContextBuilder<
    CallbackOidcContextBuilderRequest,
    CallbackOidcContext,
    CallbackOidcContextBuilderResult>
{ }
