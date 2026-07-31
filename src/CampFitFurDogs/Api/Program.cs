using CampFitFurDogs.Api.Endpoints;
using CampFitFurDogs.Api.Helpers;
using CampFitFurDogs.Api.Platform;
using Frank.Core.Api.Endpoints;
using Frank.Core.Api.Platform;
using Frank.Identity.Api.Endpoints;
using Frank.Identity.Api.Platform;

var builder = WebApplication.CreateBuilder(args);

await Hosting.AdaptToHostingEnvironment(builder);

var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddCampFitFurDogsApiPlatform(configuration)
    .AddFrankCoreApiPlatform(configuration)
    .AddFrankIdentityApiPlatform(configuration)
    ;

services
    .AddFrankIdentityApiEndpoints()
    .AddCampFitFurDogsApiEndpoints()
    ;

var app = builder.Build();

app
    .UseFrankCoreApiPlatform()
    .UseFrankIdentityApiPlatform()
    ;

app
    .MapRegisteredApiEndpoints()
    ;

app.Run();
