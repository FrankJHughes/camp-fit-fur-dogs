using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Infrastructure;
using Frank.Testing.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using System.Net.Http.Json;
using CampFitFurDogs.TestUtilities.Endpoints.SignIn;

namespace CampFitFurDogs.TestUtilities.Factories;

public sealed class ApiFactory
    : MutatedWebApplicationFactory<Program, ApiContext, ApiClientContext>
{
    public ApiFactory(ApiContext ctx) : base(ctx)
    {
    }

    // ------------------------------------------------------------
    // AUTHENTICATION OVERRIDE (NEW)
    // ------------------------------------------------------------
    protected override async Task ApplyAuthenticationAsync(HttpClient client, ApiClientContext clientCtx)
    {
        if (clientCtx.AuthenticatedUserSub is string sub)
        {
            var request = new SignInRequest(sub);

            var response = await client.PostAsJsonAsync(
                "/__test__/sign-in",
                request);

            response.EnsureSuccessStatusCode();
        }
    }

    // ------------------------------------------------------------
    // DATABASE OVERRIDE
    // ------------------------------------------------------------
    protected override void ConfigureDatabase(
        WebHostBuilderContext context,
        IServiceCollection services,
        PostgreSqlContainer postgres)
    {
        services.RemoveAll<DbContextOptions<AppDbContext>>();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(postgres!.GetConnectionString()));

        services.AddHostedService<TestDatabaseInitializer>();
    }
}
