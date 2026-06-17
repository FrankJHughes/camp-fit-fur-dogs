using System.Net.Http.Json;
using CampFitFurDogs.Infrastructure.Data;
using CampFitFurDogs.TestUtilities.Contexts;
using CampFitFurDogs.TestUtilities.Infrastructure;
using CampFitFurDogs.TestUtilities.TestEndpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CampFitFurDogs.TestUtilities.Factories;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly ApiContext _ctx;

    public ApiFactory(ApiContext ctx)
    {
        _ctx = ctx;
    }

    public HttpClient CreateClient(ApiClientContext clientCtx)
    {
        var client = base.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true,
            AllowAutoRedirect = false
        });

        foreach (var kvp in clientCtx.DefaultHeaders!)
            client.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);

        if (clientCtx.AuthenticatedUserSub is string id)
        {
            var payload = new { Sub = id };
            var response = client.PostAsJsonAsync("/__test__/sign-in", payload).Result;
            response.EnsureSuccessStatusCode();
        }

        return client;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_ctx.Environment);

        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = _ctx.Environment,
                ["Frontend:BaseUrl"] = "http://localhost:5173",
                ["Authentication:Callback:Oidc:Disabled"] = "true"
            });

            foreach (var apply in _ctx.ConfigOverrides)
                apply(cfg);
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Cookie defaults for TestServer
            services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
            {
                opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
                opts.Cookie.SameSite = SameSiteMode.Lax;
            });

            if (_ctx.UseCookieAuthOnly)
            {
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.HttpOnly = true;
                });
            }

            if (!_ctx.OverrideCookiesForHttp)
            {
                services.PostConfigureAll<CookieAuthenticationOptions>(opts =>
                {
                    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            }

            // Apply cookie option overrides
            foreach (var apply in _ctx.CookieOptionsOverrides)
                services.PostConfigureAll(apply);

            // Apply service overrides
            foreach (var apply in _ctx.ServiceOverrides)
                apply(services);

            // Database
            if (!_ctx.DisableDatabase)
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_ctx.Postgres!.GetConnectionString()));

                services.AddHostedService<TestDatabaseInitializer>();
            }
            else
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
            }

            services.AddTransient<IStartupFilter, TestEndpointsStartupFilter>();
        });
    }
}
