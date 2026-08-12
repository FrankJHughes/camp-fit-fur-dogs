using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Frank.Core.Api.Routing.Validation;

/// <summary>
/// A reusable Minimal API endpoint filter that automatically validates request
/// DTOs using FluentValidation before the endpoint handler executes.
/// <para>
/// This filter runs after ASP.NET Core model binding has created the request
/// object, but before the endpoint handler is invoked.
/// If validation fails, a <see cref="FluentValidation.ValidationException"/> is
/// thrown, allowing the API layer to translate the failure into a structured
/// <c>400 Bad Request</c> response.
/// </para>
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request DTO to validate.
/// </typeparam>
public sealed class EndpointFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> _validator;

    /// <summary>
    /// Creates a new instance of the <see cref="EndpointFilter{TRequest}"/>.
    /// </summary>
    /// <param name="validator">
    /// The FluentValidation validator responsible for validating the request DTO.
    /// </param>
    public EndpointFilter(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// Invokes the validation endpoint filter.
    /// Extracts the request DTO from the endpoint invocation context, validates it,
    /// and throws a <see cref="FluentValidation.ValidationException"/> if validation
    /// fails.
    /// If validation succeeds, the request is passed to the next filter or endpoint
    /// handler.
    /// </summary>
    /// <param name="context">The endpoint invocation context.</param>
    /// <param name="next">The next filter or endpoint handler in the pipeline.</param>
    /// <returns>The result of the next filter or endpoint handler.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the request DTO cannot be located in the endpoint arguments.
    /// </exception>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();

        if (request is null)
        {
            throw new InvalidOperationException(
                $"Validation.EndpointFilter could not locate a request argument of type '{typeof(TRequest).Name}'.");
        }

        await _validator.ValidateAndThrowAsync(request);

        return await next(context);
    }
}
