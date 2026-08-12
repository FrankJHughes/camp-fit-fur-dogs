using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.Core.Api.Routing.Validation;

/// <summary>
/// Adds FluentValidation-based request validation to all endpoints within a
/// Minimal API endpoint group. This plugs into the unified endpoint filtering
/// pipeline created by <see cref="EndpointFilteringRegistration"/>.
/// </summary>
public static class RouteGroupBuilderExtensions
{
    /// <summary>
    /// Adds automatic FluentValidation request validation to all endpoints
    /// within the specified endpoint group.
    /// </summary>
    /// <param name="group">The endpoint group to configure.</param>
    public static void AddRequestValidation(this RouteGroupBuilder group)
    {
        group.AddEndpointFilterFactory((context, next) =>
        {
            // Identify the request DTO type for this endpoint
            var requestType = context.MethodInfo
                .GetParameters()
                .Select(p => p.ParameterType)
                .FirstOrDefault(p =>
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(p);
                    return context.ApplicationServices.GetService(validatorType) != null;
                });

            // No validator found → skip validation
            if (requestType is null)
                return next;

            // Resolve the validator
            var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
            var validator = context.ApplicationServices.GetRequiredService(validatorType);

            // Build the filter instance
            var filterType = typeof(EndpointFilter<>).MakeGenericType(requestType);
            var filter = (IEndpointFilter)Activator.CreateInstance(filterType, validator)!;

            // Wrap the next delegate
            return async invocationContext =>
            {
                return await filter.InvokeAsync(invocationContext, next);
            };
        });
    }
}
