using CampFitFurDogs.Api.Customers;
using CampFitFurDogs.Api.Dogs;
using CampFitFurDogs.Application;
using CampFitFurDogs.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
    .WithName("HealthCheck");

app.MapCreateCustomer();
app.MapRegisterDog();

app.Run();
