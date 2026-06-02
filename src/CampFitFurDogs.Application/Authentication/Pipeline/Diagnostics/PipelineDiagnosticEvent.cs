namespace CampFitFurDogs.Application.Authentication.Pipeline.Diagnostics;

/// <summary>
/// A structured diagnostic event emitted by the authentication callback pipeline.
/// </summary>
public sealed record PipelineDiagnosticEvent(
    string StepId,
    string StepName,
    string Phase,              // "Start" or "End"
    long? DurationMs,          // null for Start
    AuthCallbackContext Before,
    AuthCallbackContext After
);
