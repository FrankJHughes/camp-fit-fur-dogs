namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Provides metadata describing an immutable‑context build step, including its
/// unique identifier and human‑readable display name.
///
/// <para>
/// Metadata enables the immutable‑context pipeline to organize, identify, and
/// introspect build steps. It is used for diagnostics, ordering, logging, and
/// tooling that needs to understand which step executed and how it is
/// represented.
/// </para>
/// </summary>
public sealed class ImmutableContextBuildStepMetadata : IImmutableContextBuildStepMetadata
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="ImmutableContextBuildStepMetadata"/> class.
    /// </summary>
    /// <param name="id">
    /// A stable, unique identifier for the build step. This value is used for
    /// tracing, diagnostics, and pipeline coordination.
    /// </param>
    /// <param name="displayName">
    /// A human‑readable name describing the build step. Useful for logs,
    /// debugging, and developer‑facing tooling.
    /// </param>
    public ImmutableContextBuildStepMetadata(string id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    /// <summary>
    /// Gets the unique identifier for the build step.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the human‑readable name for the build step.
    /// </summary>
    public string DisplayName { get; }
}
