namespace Frank.Core.Application.Abstractions.ImmutableContexts;

/// <summary>
/// Represents a diagnostic event emitted during the execution of an immutable
/// context build step.
///
/// <para>
/// Diagnostic events capture before/after snapshots of the immutable context,
/// along with metadata describing which step executed, what phase it was in,
/// and how long the step took. These events enable deep introspection into the
/// context‑building pipeline, supporting debugging, profiling, and
/// observability.
/// </para>
///
/// <para>
/// Because contexts are immutable, the <see cref="Before"/> and <see cref="After"/>
/// values always represent distinct instances, allowing precise tracking of
/// transformations applied by each step.
/// </para>
/// </summary>
/// <param name="StepId">
/// The unique identifier of the build step that produced this diagnostic event.
/// </param>
/// <param name="StepName">
/// A human‑readable name describing the build step.
/// </param>
/// <param name="Phase">
/// The phase of execution in which the event occurred (e.g., "Before",
/// "After", "Execute").
/// </param>
/// <param name="DurationMs">
/// The duration of the step in milliseconds, if available. May be <c>null</c>
/// when timing is not recorded.
/// </param>
/// <param name="Before">
/// The immutable context instance before the step executed.
/// </param>
/// <param name="After">
/// The immutable context instance after the step executed.
/// </param>
public sealed record ImmutableContextBuilderDiagnosticEvent(
    string StepId,
    string StepName,
    string Phase,
    long? DurationMs,
    ImmutableContextBase Before,
    ImmutableContextBase After);
