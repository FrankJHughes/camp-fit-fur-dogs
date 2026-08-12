using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Frank.Core.Api.Routing.Validation;

/// <summary>
/// Provides extension methods for attaching FluentValidation-based request
/// validation to Minimal API endpoints.
/// </summary>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="EndpointFilter{TRequest}"/> to the endpoint pipeline,
    /// enabling automatic FluentValidation execution for the specified request type.
    /// </summary>
    /// <typeparam name="TRequest">The request DTO type to validate.</typeparam>
    /// <param name="builder">The route handler builder.</param>
    /// <returns>The updated route handler builder.</returns>
    public static RouteHandlerBuilder WithValidation<TRequest>(
        this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<EndpointFilter<TRequest>>();
    }
}
