using CampFitFurDogs.Application.Errors;
using Frank.Core.Application.Abstractions.Errors;
using Frank.Core.Application.Abstractions.Exceptions;
using Frank.Identity.Domain.Users.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

[ExceptionHandler(300)]
public sealed class DuplicateEmailExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) =>
        ex is EmailAlreadyExistsException;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.DuplicateEmail;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Duplicate Email",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict,
            Type = "https://httpstatuses.com/409"
        };
}
