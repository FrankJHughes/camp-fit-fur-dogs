namespace Frank.Identity.Application.Abstractions;

public enum AuthCallbackError
{
    MissingAuthorizationCode,
    IncompleteConfiguration,
    MissingAccessToken,
    UserInfoFailure,
    MissingExternalId,
    MissingResult
}
