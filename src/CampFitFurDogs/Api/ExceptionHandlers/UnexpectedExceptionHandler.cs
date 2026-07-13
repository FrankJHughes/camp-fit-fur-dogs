using CampFitFurDogs.Application.Errors;
using Frank.Core.Application.Abstractions.Errors;
using Frank.Core.Application.Abstractions.Exceptions;

namespace CampFitFurDogs.Api.ExceptionHandlers;

[ExceptionHandler(int.MaxValue)]
public sealed class UnexpectedExceptionHandler : IExceptionHandler
{
    public bool CanHandle(System.Exception ex) => true;

    public IErrorCode GetErrorCode(System.Exception ex) =>
        ErrorCode.Unexpected;

    public ProblemDetails CreateProblemDetails(System.Exception ex) =>
        new()
        {
            Title = "Internal Server Error",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
}
