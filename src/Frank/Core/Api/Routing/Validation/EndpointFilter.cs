using System.Diagnostics;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Frank.Core.Application.Abstractions.Observations;

namespace Frank.Core.Api.Routing.Validation;

/// <summary>
/// A Minimal API endpoint filter that performs FluentValidation-based request
/// validation and emits structured observability events into the unified
/// <see cref="IRequestObservationContext"/> envelope.
/// <para>
/// This filter runs after ASP.NET Core model binding has created the request
/// DTO but before the endpoint handler executes. It provides API-level
/// validation observability (US‑199), including:
/// <list type="bullet">
/// <item><description>Validation start and end events</description></item>
/// <item><description>Validation duration measurement</description></item>
/// <item><description>Structured validation failure diagnostics</description></item>
/// <item><description>Correlation propagation via <see cref="IRequestObservationContext"/></description></item>
/// </list>
/// </para>
/// <para>
/// If validation fails, the filter emits a structured failure event and returns
/// a <c>400 Bad Request</c> response containing validation errors.
/// </para>
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request DTO to validate.
/// </typeparam>
public sealed class EndpointFilter<TRequest> : IEndpointFilter
{
    private readonly IValidator<TRequest> _validator;
    private readonly ILogger<EndpointFilter<TRequest>> _logger;
    private readonly IRequestObservationContext _obs;

    public EndpointFilter(
        IValidator<TRequest> validator,
        ILogger<EndpointFilter<TRequest>> logger,
        IRequestObservationContext obs)
    {
        _validator = validator;
        _logger = logger;
        _obs = obs;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        var route = http.GetEndpoint()?.DisplayName ?? "unknown";
        var dtoType = typeof(TRequest).FullName ?? "unknown";

        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            throw new InvalidOperationException(
                $"Validation.EndpointFilter could not locate a request argument of type '{typeof(TRequest).Name}'.");
        }

        var sw = Stopwatch.StartNew();

        // Emit validation start event
        _logger.LogInformation(
            new EventId(0, ApiValidationEvents.Start),
            "API validation started for {Route} ({DtoType}) with correlation {CorrelationId}",
            route,
            dtoType,
            _obs.CorrelationId);

        // Perform validation
        var result = await _validator.ValidateAsync(request);

        sw.Stop();

        // Emit validation end event
        _logger.LogInformation(
            new EventId(0, ApiValidationEvents.End),
            "API validation completed for {Route} ({DtoType}) in {DurationMs} ms with correlation {CorrelationId}",
            route,
            dtoType,
            sw.ElapsedMilliseconds,
            _obs.CorrelationId);

        // Handle validation failures
        if (!result.IsValid)
        {
            var errorCodes = result.Errors
                .Select(e => e.ErrorCode ?? e.PropertyName)
                .ToArray();

            var diagnostic = new ApiValidationDiagnostic
            {
                Route = route,
                DtoType = dtoType,
                ErrorCount = errorCodes.Length,
                ErrorCodes = errorCodes,
                DurationMs = sw.ElapsedMilliseconds
            };

            // Enrich unified observability envelope
            _obs.AddMetadata(ObservationMetadataKeys.ApiValidation, diagnostic);

            // Emit failure event
            _logger.LogWarning(
                new EventId(0, ApiValidationEvents.Failed),
                "API validation failed for {Route} ({DtoType}) with {ErrorCount} errors and correlation {CorrelationId}. Errors: {ErrorCodes}",
                route,
                dtoType,
                diagnostic.ErrorCount,
                _obs.CorrelationId,
                diagnostic.ErrorCodes);

            // Return structured 400 response
            return Results.ValidationProblem(
                result.ToDictionary(),
                statusCode: StatusCodes.Status400BadRequest);
        }

        // Continue pipeline
        return await next(context);
    }
}
