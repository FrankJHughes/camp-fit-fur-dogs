using FluentValidation;

namespace Frank.Core.Application.Validation;

/// <summary>
/// Provides extension methods for executing FluentValidation validators in a
/// concise, expressive manner within API request pipelines.
/// <para>
/// This helper enables a clean pattern where request DTOs validate themselves
/// through their associated <see cref="IValidator{T}"/> before being passed deeper
/// into the application layer.
/// It throws a <see cref="ValidationException"/> automatically when validation
/// fails, allowing your global exception handlers to translate failures into
/// structured <c>ProblemDetails</c> responses.
/// </para>
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Validates the specified request using the provided FluentValidation
    /// validator.
    /// If validation fails, a <see cref="ValidationException"/> is thrown.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the request object being validated.
    /// </typeparam>
    /// <param name="request">
    /// The request instance to validate.
    /// </param>
    /// <param name="validator">
    /// The FluentValidation validator responsible for validating the request.
    /// </param>
    /// <param name="ct">
    /// A <see cref="CancellationToken"/> used to observe cancellation.
    /// </param>
    /// <returns>
    /// The original request instance, allowing fluent chaining in endpoint
    /// handlers or pipelines.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown when validation fails.
    /// This exception is handled by <c>ValidationExceptionHandler</c> in the API
    /// layer, producing a structured <c>400 Bad Request</c> response.
    /// </exception>
    public static async Task<T> Validate<T>(
        this T request,
        IValidator<T> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);
        return request;
    }
}
