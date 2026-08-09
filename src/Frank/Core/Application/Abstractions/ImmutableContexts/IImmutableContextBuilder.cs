namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Defines the contract for building an immutable context from a request.
///
/// <para>
/// Immutable contexts represent structured, read‑only state derived from an
/// incoming request. They are typically used to prepare validated, normalized,
/// and enriched data that downstream components can rely on without mutation.
/// </para>
///
/// <para>
/// The <see cref="IImmutableContextBuilder{TRequest, TContext, TResult}"/>
/// abstraction encapsulates the logic required to transform a request into a
/// fully constructed context and a corresponding result object. This pattern
/// supports deterministic, side‑effect‑free context creation and improves
/// testability by isolating context‑building behavior.
/// </para>
/// </summary>
/// <typeparam name="TRequest">
/// The request type used to initiate context building. Must derive from
/// <see cref="ImmutableContextBuilderRequestBase"/>.
/// </typeparam>
/// <typeparam name="TContext">
/// The immutable context type produced during the build process. Must derive
/// from <see cref="ImmutableContextBase"/>.
/// </typeparam>
/// <typeparam name="TResult">
/// The result type returned after the context is built. Must derive from
/// <see cref="ImmutableContextBuilderResultBase"/>.
/// </typeparam>
public interface IImmutableContextBuilder<TRequest, TContext, TResult>
    where TRequest : ImmutableContextBuilderRequestBase
    where TContext : ImmutableContextBase
    where TResult : ImmutableContextBuilderResultBase
{
    /// <summary>
    /// Builds an immutable context from the specified request.
    ///
    /// <para>
    /// Implementations typically validate the request, construct the immutable
    /// context, and return a result object containing the context and any
    /// additional metadata. The returned task completes when the context has
    /// been fully constructed or when cancellation is requested.
    /// </para>
    /// </summary>
    /// <param name="request">
    /// The request used to build the immutable context.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <typeparamref name="TResult"/> containing the constructed immutable
    /// context and any associated result data.
    /// </returns>
    Task<TResult> BuildAsync(TRequest request, CancellationToken ct);
}
