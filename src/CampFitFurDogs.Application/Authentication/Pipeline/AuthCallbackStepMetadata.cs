namespace CampFitFurDogs.Application.Authentication.Pipeline;

public sealed record AuthCallbackStepMetadata(
    string Id,
    string DisplayName,
    AuthCallbackStepCategory Category);
