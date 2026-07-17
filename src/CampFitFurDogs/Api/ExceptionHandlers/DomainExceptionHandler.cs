using CampFitFurDogs.Application.Errors;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Core.Domain;
using Frank.Identity.Domain.Users.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

[ExceptionHandler(500)]
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is InvalidFirstNameException or
            InvalidLastNameException or
            InvalidEmailException or
            InvalidPhoneNumberException or
            InvalidExternalAuthProviderIdException or
            InvalidPasswordHashException or
            ConflictingIdentitySourcesException or
            MissingIdentitySourceException or
            DomainException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.DomainError;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Domain Error",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
}
