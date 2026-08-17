namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Provides metadata describing an immutable‑context build step.
///
/// <para>
/// Metadata allows the build pipeline to identify, order, and describe
/// individual steps involved in constructing an immutable context. This
/// information is useful for diagnostics, introspection, logging, and
/// pipeline orchestration.
/// </para>
/// </summary>
public interface IImmutableContextBuildStepMetadata
{
    /// <summary>
    /// Gets the unique identifier for the build step.
    ///
    /// <para>
    /// The identifier should be stable and suitable for logging, tracing,
    /// and pipeline coordination. It distinguishes this step from others
    /// in the build process.
    /// </para>
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets a human‑readable name for the build step.
    ///
    /// <para>
    /// The display name is intended for diagnostics, debugging, and
    /// developer‑facing tooling. It provides a clear description of the
    /// step’s purpose or behavior.
    /// </para>
    /// </summary>
    string DisplayName { get; }
}
