using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

using CampFitFurDogs.Api;
using CampFitFurDogs.Application.DependencyInjection;
using CampFitFurDogs.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = validationException.Errors
                .Select(e => new { e.PropertyName, e.ErrorMessage });

            await context.Response.WriteAsJsonAsync(new { Errors = errors });
            return;
        }

        // fallback for other exceptions
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("HealthCheck");

app.MapEndpoints();

app.Run();
