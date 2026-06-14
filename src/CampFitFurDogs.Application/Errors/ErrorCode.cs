using Frank.Abstractions.Errors;

namespace CampFitFurDogs.Application.Errors;

public sealed class ErrorCode : IErrorCode
{
    public string Code { get; }
    public string? Description { get; }

    private ErrorCode(string code, string? description = null)
    {
        Code = code;
        Description = description;
    }

    public static readonly ErrorCode ExternalAuthProviderFailure = new("external_auth_provider_failure");
    public static readonly ErrorCode UserNotAuthenticated = new("user_not_authenticated");
    public static readonly ErrorCode InvalidUserIdentity = new("invalid_user_identity");
    public static readonly ErrorCode BadConfiguration = new("bad_configuration");
    public static readonly ErrorCode BadRequest = new("bad_request");
    public static readonly ErrorCode DuplicateEmail = new("duplicate_email");
    public static readonly ErrorCode ValidationFailed = new("validation_failed");
    public static readonly ErrorCode DomainError = new("domain_error");
    public static readonly ErrorCode Unexpected = new("unexpected");
}
