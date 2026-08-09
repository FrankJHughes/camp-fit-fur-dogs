namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Represents the base type for all immutable context objects used within the
/// application.
///
/// <para>
/// Immutable contexts encapsulate structured, read‑only state derived from
/// validated and normalized inputs. They are produced by immutable‑context
/// builders and consumed by downstream components that rely on deterministic,
/// side‑effect‑free data.
/// </para>
///
/// <para>
/// Deriving from <see cref="ImmutableContextBase"/> indicates that the context
/// instance must not be mutated after creation. Any transformation should
/// produce a new context instance, preserving immutability throughout the
/// pipeline.
/// </para>
/// </summary>
public abstract record ImmutableContextBase;
