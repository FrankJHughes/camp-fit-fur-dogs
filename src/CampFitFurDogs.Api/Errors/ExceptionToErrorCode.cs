using FluentValidation;
using SharedKernel.Domain;
using CampFitFurDogs.Domain.Errors;
using CampFitFurDogs.Domain.Customers.Exceptions;

namespace CampFitFurDogs.Api.Errors;

public static class ExceptionToErrorCode
{
    public static ErrorCode Map(Exception ex) =>
        ex switch
        {
            // ───────────────────────────────────────────────
            // External Auth Provider Failures (502)
            // ───────────────────────────────────────────────
            ExternalAuthProviderException => ErrorCode.ExternalAuthProviderFailure,

            // ───────────────────────────────────────────────
            // Server Misconfiguration (500)
            // ───────────────────────────────────────────────
            BadConfigurationException => ErrorCode.BadConfiguration,

            // ───────────────────────────────────────────────
            // Application-level
            // ───────────────────────────────────────────────
            EmailAlreadyExistsException => ErrorCode.DuplicateEmail,

            // ───────────────────────────────────────────────
            // Request-level validation
            // ───────────────────────────────────────────────
            ValidationException => ErrorCode.ValidationFailed,

            // ───────────────────────────────────────────────
            // Domain-level invariants
            // ───────────────────────────────────────────────
            InvalidFirstNameException or
            InvalidLastNameException or
            InvalidEmailException or
            InvalidPhoneNumberException or
            InvalidExternalAuthProviderIdException or
            InvalidPasswordHashException or
            ConflictingIdentitySourcesException or
            MissingIdentitySourceException or
            DomainException => ErrorCode.DomainError,

            // ───────────────────────────────────────────────
            // Everything else → 500
            // ───────────────────────────────────────────────
            _ => ErrorCode.Unexpected
        };
}
