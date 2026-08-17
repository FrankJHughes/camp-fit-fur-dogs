namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Represents the base type for all results produced by an immutable‑context
/// builder.
///
/// <para>
/// Builder results encapsulate the final output of the immutable‑context
/// construction process. They typically contain the fully constructed immutable
/// context along with any additional metadata, diagnostics, or supplemental
/// information generated during the build.
/// </para>
///
/// <para>
/// Deriving from <see cref="ImmutableContextBuilderResultBase"/> indicates that
/// the result is part of the immutable‑context pipeline and is intended to be a
/// stable, read‑only representation of the completed build operation.
/// </para>
/// </summary>
public abstract record ImmutableContextBuilderResultBase;
