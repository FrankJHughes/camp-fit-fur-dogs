using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

using CampFitFurDogs.Api.HostingEnvironment;
using CampFitFurDogs.Infrastructure;
using CampFitFurDogs.Infrastructure.Data;
using SharedKernel.DependencyInjection;
using SharedKernel.Api;
using SharedKernel.Infrastructure.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

await EnvironmentBootstrapper.ApplyOverridesAsync(builder);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port)); // IPv4 ANY
});

builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// 0. CORS: allow frontend host
var allowedOrigin = builder.Configuration["Frontend:BaseUrl"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigin ?? "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 1. register infrastructure layer
//          db context
builder.Services.AddInfrastructure(builder.Configuration);

//
// BEGIN SHARED KERNEL ASSISTED REGISTRATION
//

// 2. register ef core infrastructure layer:
//          unit of work
builder.Services.AddSharedKernelEfCore<AppDbContext>();

// 3. register application layer:
//          handlers, validators, dispatchers
builder.Services.AddSharedKernel(
    applicationAssemblies: new[]
    {
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
    },
    configure: options =>
    {
        // 4. register infrastructure layer:
        //          repositories, readers,
        //          providers, services
        options.AddInfrastructureAutoRegistration(
            assemblies: new[]
            {
                typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly
            },
            rules => rules
                .Add("Repository", ServiceLifetime.Scoped)
                .Add("Reader", ServiceLifetime.Scoped)
                .Add("Provider", ServiceLifetime.Scoped)
                .Add("Service", ServiceLifetime.Scoped));
    });

// 5. register api layer:
//          endpoints
var apiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
EndpointDiscovery.AddEndpoints(apiAssembly);

//
// END SHARED KERNEL ASSISTED REGISTRATION
//

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapEndpoints();

app.UseCors();

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

app.Run();
