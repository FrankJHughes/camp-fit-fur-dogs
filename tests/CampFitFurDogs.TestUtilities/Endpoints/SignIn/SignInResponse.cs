namespace CampFitFurDogs.TestUtilities.Endpoints.SignIn;

public sealed record SignInResponse(
    Guid OwnerId,
    Guid SessionId,
    string PlaintextToken);

