using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication.Diagnostics;

public sealed record AuthCallbackDiagnosticEvent(
    string StepId,
    string StepName,
    string Phase,          // "Start" or "End"
    long? DurationMs,      // null for Start, populated for End
    AuthCallbackContext Before,
    AuthCallbackContext After
);
