using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Exceptions;
using Frank.Domain;

namespace CampFitFurDogs.Api.Horizontal.Exception.Handler;

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
