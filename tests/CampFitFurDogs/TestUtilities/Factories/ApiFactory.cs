using System.Net.Http.Json;
using CampFitFurDogs.Infrastructure.Persistence;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Endpoints.SignIn;
using CampFitFurDogs.TestUtilities.Infrastructure;
using Frank.Identity.EntityFrameworkCore.DbContexts;
using Frank.Testing.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace CampFitFurDogs.TestUtilities.Factories;

public sealed class ApiFactory
    : MutatedWebApplicationFactory<Program, ApiContext, ApiClientContext>
{
    public ApiFactory(ApiContext ctx) : base(ctx)
    {
    }

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

    protected override void ConfigureDatabase(WebHostBuilderContext context, IServiceCollection services, PostgreSqlContainer postgres)
    {
        var connectionString = postgres!.GetConnectionString();

        services

        // Frank identity DB
            .RemoveAll<DbContextOptions<FrankIdentityDbContext>>()
            .AddDbContext<FrankIdentityDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsHistoryTable("__EFMigrationsHistory", "frank_identity")),
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped)
            .AddHostedService<TestDatabaseInitializer<FrankIdentityDbContext>>()

        // CFFD application DB (dogs, owners, etc.)
            .RemoveAll<DbContextOptions<AppDbContext>>()
            .AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsHistoryTable("__EFMigrationsHistory", "cffd")),
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped)
            .AddHostedService<TestDatabaseInitializer<AppDbContext>>();
    }
}
