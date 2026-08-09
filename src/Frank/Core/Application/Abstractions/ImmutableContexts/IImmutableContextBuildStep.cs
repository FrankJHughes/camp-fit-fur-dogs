namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Represents a single step in the immutable‑context build pipeline.
///
/// <para>
/// Immutable context build steps allow the construction of an immutable
/// <typeparamref name="TContext"/> to be broken into discrete, ordered,
/// self‑contained operations. Each step can determine whether it should execute
/// based on the current state of the context and can produce an updated context
/// without mutating existing state.
/// </para>
///
/// <para>
/// This pattern supports deterministic, composable, and testable context
/// construction, where each step contributes a well‑defined transformation.
/// </para>
/// </summary>
/// <typeparam name="TContext">
/// The immutable context type being constructed. Must derive from
/// <see cref="ImmutableContextBase"/>.
/// </typeparam>
public interface IImmutableContextBuildStep<TContext>
    where TContext : ImmutableContextBase
{
    /// <summary>
    /// Gets metadata describing this build step, including ordering,
    /// identification, or other step‑specific characteristics.
    ///
    /// <para>
    /// Metadata enables the build pipeline to organize steps, apply ordering,
    /// and perform diagnostics or introspection.
    /// </para>
    /// </summary>
    IImmutableContextBuildStepMetadata Metadata { get; }

    /// <summary>
    /// Determines whether this step should execute for the given context.
    ///
    /// <para>
    /// Steps may inspect the context to decide whether they are applicable.
    /// This allows conditional execution, enabling flexible and environment‑ or
    /// state‑dependent context construction.
    /// </para>
    /// </summary>
    /// <param name="context">
    /// The current immutable context instance.
    /// </param>
    /// <returns>
    /// <c>true</c> if the step should execute; otherwise, <c>false</c>.
    /// </returns>
    bool CanExecute(TContext context);

    /// <summary>
    /// Executes the build step asynchronously and returns an updated immutable
    /// context instance.
    ///
    /// <para>
    /// Implementations must not mutate the incoming context. Instead, they
    /// should produce a new <typeparamref name="TContext"/> instance containing
    /// the updated state. This ensures immutability and supports deterministic
    /// pipeline behavior.
    /// </para>
    /// </summary>
    /// <param name="context">
    /// The current immutable context instance.
    /// </param>
    /// <param name="ct">
    /// A cancellation token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A new <typeparamref name="TContext"/> instance representing the updated
    /// immutable context.
    /// </returns>
    Task<TContext> ExecuteAsync(TContext context, CancellationToken ct);
}
