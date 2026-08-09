namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Represents the base type for all requests used to initiate the construction
/// of an immutable context.
///
/// <para>
/// Requests supply the raw input data required by an immutable‑context builder.
/// They typically contain values that must be validated, normalized, or
/// enriched before being transformed into a concrete immutable context.
/// </para>
///
/// <para>
/// Deriving from <see cref="ImmutableContextBuilderRequestBase"/> indicates that
/// the request participates in the immutable‑context pipeline and is intended
/// solely as an input model. It does not enforce immutability itself, but
/// serves as the starting point for producing immutable state.
/// </para>
/// </summary>
public abstract record ImmutableContextBuilderRequestBase;
