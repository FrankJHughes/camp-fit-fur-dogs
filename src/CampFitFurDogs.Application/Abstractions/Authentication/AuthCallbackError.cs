namespace CampFitFurDogs.Application.Abstractions.Authentication;

public enum AuthCallbackError
{
    MissingAuthorizationCode,
    IncompleteConfiguration,
    MissingAccessToken,
    UserInfoFailure,
    MissingExternalId
}
