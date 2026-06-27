using CampFitFurDogs.Application.Errors;
using Frank.Abstractions.Errors;
using Frank.Abstractions.Problem;

[ExceptionHandler(int.MaxValue)]
public sealed class UnexpectedExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception ex) => true;

    public IErrorCode GetErrorCode(Exception ex) =>
        ErrorCode.Unexpected;

    public ProblemDetails CreateProblemDetails(Exception ex) =>
        new()
        {
            Title = "Internal Server Error",
            Detail = ex.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
}
