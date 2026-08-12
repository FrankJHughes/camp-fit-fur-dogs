using CampFitFurDogs.Api.Endpoints;
using CampFitFurDogs.Api.Helpers;
using CampFitFurDogs.Api.Platform;
using Frank.Core.Api.Endpoints;
using Frank.Core.Api.Platform;
using Frank.Identity.Api.Endpoints;
using Frank.Identity.Api.Platform;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Adapts the web host to the current environment (Development, Testing, Production).
/// <para>
/// This includes environment‑specific configuration loading, platform initialization,
/// and any hosting‑level adjustments required before service registration.
/// </para>
/// </summary>
await Hosting.AdaptToHostingEnvironment(builder);

var services = builder.Services;
var configuration = builder.Configuration;

/// <summary>
/// Registers all platform‑level services required by the Camp Fit Fur Dogs API.
/// <para>
/// This includes:
/// <list type="bullet">
/// <item><description>CampFitFurDogs API platform services</description></item>
/// <item><description>Frank.Core API platform services</description></item>
/// <item><description>Frank.Identity API platform services</description></item>
/// </list>
/// These services provide cross‑cutting concerns such as logging, observability,
/// authentication, authorization, and environment‑specific hosting behavior.
/// </para>
/// </summary>
services
    .AddCampFitFurDogsApiPlatform(configuration)
    .AddFrankCoreApiPlatform(configuration)
    .AddFrankIdentityApiPlatform(configuration);

/// <summary>
/// Registers all production API endpoints exposed by the application.
/// <para>
/// Endpoints are discovered via assembly scanning and grouped into vertical slices.
/// This call wires up:
/// <list type="bullet">
/// <item><description>Identity endpoints</description></item>
/// <item><description>CampFitFurDogs domain endpoints</description></item>
/// </list>
/// </para>
/// </summary>
services
    .AddFrankIdentityApiEndpoints()
    .AddCampFitFurDogsApiEndpoints();

var app = builder.Build();

/// <summary>
/// Configures the API pipeline by applying platform‑level middleware.
/// <para>
/// This includes:
/// <list type="bullet">
/// <item><description>Frank.Core middleware (routing, exception handling, observability)</description></item>
/// <item><description>Frank.Identity middleware (authentication, session management)</description></item>
/// </list>
/// </para>
/// </summary>
app
    .UseFrankCoreApiPlatform()
    .UseFrankIdentityApiPlatform();

/// <summary>
/// Maps all registered API endpoints under the <c>/api</c> route group.
/// <para>
/// The <c>/api</c> prefix is applied automatically to all endpoints, ensuring a
/// consistent and predictable public API surface.
/// </para>
/// <para>
/// The group is tagged <c>API</c> for documentation and tooling purposes.
/// </para>
/// </summary>
app.MapRegisteredApiEndpoints("/api")
    .WithTags("API")
    .WithDescription("Camp Fit Fur Dogs API");

app.Run();
