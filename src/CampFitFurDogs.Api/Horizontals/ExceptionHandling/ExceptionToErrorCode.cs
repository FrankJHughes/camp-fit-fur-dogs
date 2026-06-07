using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Customers.Exceptions;
using CampFitFurDogs.Domain.Errors;
using FluentValidation;
using Frank.Domain;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling;

public static class ExceptionToErrorCode
{
    public static ErrorCode Map(Exception ex) =>
        ex switch
        {
            // ───────────────────────────────────────────────
            // External Auth Provider Failures (502)
            // Includes AuthCallbackException
            // ───────────────────────────────────────────────
            ExternalAuthProviderException => ErrorCode.ExternalAuthProviderFailure,
            AuthCallbackException => ErrorCode.ExternalAuthProviderFailure,

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
