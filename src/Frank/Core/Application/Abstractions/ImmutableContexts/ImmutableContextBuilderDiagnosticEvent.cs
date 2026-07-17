namespace Frank.Core.Application.Abstractions.ImmutableContexts;

public sealed record ImmutableContextBuilderDiagnosticEvent(
    string StepId,
    string StepName,
    string Phase,
    long? DurationMs,
    ImmutableContextBase Before,
    ImmutableContextBase After);
