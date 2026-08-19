using FluentValidation;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Frank.Core.Api.Routing.Validation;

/// <summary>
/// Provides a Minimal API endpoint filter factory that automatically attaches
/// FluentValidation-based request validation to endpoints.
/// <para>
/// This factory detects whether an endpoint accepts a request DTO with a
/// registered <see cref="IValidator{T}"/> and, if so, injects an
/// <see cref="EndpointFilter{TRequest}"/> that performs validation and emits
/// structured observability events (US‑199).
/// </para>
/// <para>
/// The filter is constructed with:
/// <list type="bullet">
/// <item><description>The FluentValidation validator</description></item>
/// <item><description>A typed logger</description></item>
/// <item><description>The request‑scope <see cref="IRequestObservationContext"/></description></item>
/// </list>
/// </para>
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// Adds FluentValidation request validation to all endpoints within the
    /// specified <see cref="RouteGroupBuilder"/>. Endpoints whose request DTO
    /// types do not have a registered validator are ignored.
    /// </summary>
    /// <param name="group">The route group to augment with validation filters.</param>
    /// <returns>The same <see cref="RouteGroupBuilder"/> instance.</returns>
    public static RouteGroupBuilder AddRequestValidation(this RouteGroupBuilder group)
    {
        group.AddEndpointFilterFactory((context, next) =>
        {
            var services = context.ApplicationServices;

            // Identify the request DTO type that has a registered validator
            var requestType = context.MethodInfo
                .GetParameters()
                .Select(p => p.ParameterType)
                .FirstOrDefault(p =>
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(p);
                    return services.GetService(validatorType) != null;
                });

            // No validator → no filter
            if (requestType is null)
                return next;

            // Resolve validator
            var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
            var validator = services.GetRequiredService(validatorType);

            // Resolve logger for EndpointFilter<TRequest>
            var filterType = typeof(EndpointFilter<>).MakeGenericType(requestType);
            var loggerType = typeof(ILogger<>).MakeGenericType(filterType);
            var logger = services.GetRequiredService(loggerType);

            // Resolve the unified request‑scope observation context
            var obs = services.GetRequiredService<IRequestObservationContext>();

            // Construct the filter instance
            var filter = (IEndpointFilter)Activator.CreateInstance(
                filterType,
                validator,
                logger,
                obs)!;

            // Return the filter pipeline delegate
            return invocationContext => filter.InvokeAsync(invocationContext, next);
        });

        return group;
    }
}
