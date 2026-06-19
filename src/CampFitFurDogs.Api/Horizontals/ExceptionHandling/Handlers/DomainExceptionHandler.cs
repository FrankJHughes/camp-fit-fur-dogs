using CampFitFurDogs.Application.Errors;
using CampFitFurDogs.Domain.Customers.Exceptions;
using Frank.Abstractions.Errors;
using Frank.Abstractions.ExceptionHandling;
using Frank.Domain;

namespace CampFitFurDogs.Api.Horizontals.ExceptionHandling.Handlers;

[ExceptionHandler(500)]
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) =>
        ex is InvalidFirstNameException or
            InvalidLastNameException or
            InvalidEmailException or
            InvalidPhoneNumberException or
            InvalidExternalAuthProviderIdException or
            InvalidPasswordHashException or
            ConflictingIdentitySourcesException or
            MissingIdentitySourceException or
            DomainException;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.DomainError;

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new()
        {
            Title = "Domain Error",
            Detail = ex.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400"
        };
}
