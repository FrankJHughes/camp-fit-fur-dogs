namespace Frank.Application.Abstractions.Identity;

public enum AuthCallbackError
{
    MissingAuthorizationCode,
    IncompleteConfiguration,
    MissingAccessToken,
    UserInfoFailure,
    MissingExternalId,
    MissingResult
}
