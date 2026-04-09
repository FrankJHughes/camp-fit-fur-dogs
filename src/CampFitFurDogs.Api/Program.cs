using CampFitFurDogs.Api.Customers;

using CampFitFurDogs.Application;
using CampFitFurDogs.Application.Abstractions;
using CampFitFurDogs.Application.Customers.CreateCustomer;

using CampFitFurDogs.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices();

builder.Services.AddOpenApi();

builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddTransient<ICommandHandler<CreateCustomerCommand, Guid>, CreateCustomerHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("HealthCheck");

app.MapCreateCustomer();

app.Run();
