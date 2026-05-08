#!/usr/bin/env bash
# ──────────────────────────────────────────────────────────────────
#  apply-hosting-providers.sh
#
#  Applies the pluggable hosting-provider refactor:
#    • Creates  2 new SharedKernel.Api files
#    • Creates  1 new CampFitFurDogs.Api file
#    • Replaces 1 existing CampFitFurDogs.Api file  (Program.cs)
#    • Deletes  3 old HostingEnvironment files + empty directory
#
#  Run from the repository root:
#    chmod +x apply-hosting-providers.sh
#    ./apply-hosting-providers.sh
# ──────────────────────────────────────────────────────────────────
set -euo pipefail

# ── Guard: must run from repo root ────────────────────────────────
if [[ ! -d "src/CampFitFurDogs.Api" ]]; then
  echo "ERROR: Run this script from the repository root."
  echo "       Expected to find src/CampFitFurDogs.Api/"
  exit 1
fi

echo ""
echo "═══════════════════════════════════════════════════════════"
echo "  Applying pluggable hosting-provider refactor"
echo "═══════════════════════════════════════════════════════════"
echo ""

# ── 1. Create directories ────────────────────────────────────────
mkdir -p src/SharedKernel.Api/Hosting
mkdir -p src/CampFitFurDogs.Api/Hosting

echo "[create] src/SharedKernel.Api/Hosting/"
echo "[create] src/CampFitFurDogs.Api/Hosting/"

# ── 2. Write: IHostingProvider.cs ─────────────────────────────────
cat > src/SharedKernel.Api/Hosting/IHostingProvider.cs << 'CSHARP_EOF'
using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Hosting;

/// <summary>
/// Defines a pluggable hosting environment configuration.
/// Implement this interface for each hosting provider (e.g., Render, Azure, AWS)
/// to encapsulate provider-specific startup overrides such as connection string
/// resolution, TLS configuration, and environment detection.
/// </summary>
public interface IHostingProvider
{
    /// <summary>
    /// Display name used in diagnostic logging (e.g., "Render", "Azure App Service").
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Returns <c>true</c> when the current runtime environment belongs to this
    /// provider.  Typically detected by probing for provider-specific environment
    /// variables.  Must be safe to call at any time and must not throw.
    /// </summary>
    bool IsActive();

    /// <summary>
    /// Applies all provider-specific overrides to the application builder.
    /// Called only when <see cref="IsActive"/> returns <c>true</c>.
    /// </summary>
    Task ConfigureAsync(WebApplicationBuilder builder);
}
CSHARP_EOF
echo "[write]  src/SharedKernel.Api/Hosting/IHostingProvider.cs"

# ── 3. Write: HostingProviderExtensions.cs ────────────────────────
cat > src/SharedKernel.Api/Hosting/HostingProviderExtensions.cs << 'CSHARP_EOF'
using Microsoft.AspNetCore.Builder;

namespace SharedKernel.Api.Hosting;

/// <summary>
/// Wires <see cref="IHostingProvider"/> implementations into the ASP.NET Core
/// startup pipeline.  Consuming projects call
/// <c>builder.UseHostingProviders(…)</c> in <c>Program.cs</c>, passing
/// concrete provider instances in priority order.
/// </summary>
/// <remarks>
/// Providers run <em>before</em> the DI container is built, so they cannot
/// be resolved from the service provider.  Pass concrete instances in
/// priority order instead.  First-active-wins: the first provider whose
/// <see cref="IHostingProvider.IsActive"/> returns <c>true</c> applies its
/// overrides and the rest are skipped.
/// </remarks>
public static class HostingProviderExtensions
{
    /// <summary>
    /// Evaluates each <see cref="IHostingProvider"/> in registration order.
    /// The first whose <see cref="IHostingProvider.IsActive"/> returns
    /// <c>true</c> applies its overrides; the rest are skipped.
    /// At most one provider runs per application start.
    /// </summary>
    public static async Task UseHostingProviders(
        this WebApplicationBuilder builder,
        params IHostingProvider[] providers)
    {
        foreach (var provider in providers)
        {
            if (!provider.IsActive())
            {
                Log($"{provider.ProviderName} — not detected, skipping.");
                continue;
            }

            Log($"{provider.ProviderName} — detected, applying overrides…");

            try
            {
                await provider.ConfigureAsync(builder);
                Log($"{provider.ProviderName} — overrides applied successfully.");
            }
            catch (Exception ex)
            {
                Log($"{provider.ProviderName} — override failed: {ex.Message}");
            }

            return; // first-active-wins
        }

        Log("No hosting provider matched the current environment.");
    }

    private static void Log(string message)
        => Console.WriteLine($"[Hosting] {message}");
}
CSHARP_EOF
echo "[write]  src/SharedKernel.Api/Hosting/HostingProviderExtensions.cs"

# ── 4. Write: RenderHostingProvider.cs ────────────────────────────
cat > src/CampFitFurDogs.Api/Hosting/RenderHostingProvider.cs << 'CSHARP_EOF'
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using SharedKernel.Api.Hosting;

namespace CampFitFurDogs.Api.Hosting;

/// <summary>
/// Hosting provider for Render (https://render.com).
/// Detects a Render PR-preview environment and overrides the database connection
/// string by downloading the Neon branch connection string from a GitHub Actions
/// artifact.
/// </summary>
public sealed class RenderHostingProvider : IHostingProvider
{
    // ── Environment variable names set by Render ──────────────────────
    private const string Env_IsPullRequest       = "IS_PULL_REQUEST";
    private const string Env_GitRepoSlug         = "RENDER_GIT_REPO_SLUG";
    private const string Env_RenderServiceName   = "RENDER_SERVICE_NAME";
    private const string Env_GithubPat           = "GITHUB_PAT";

    // ── Artifact / config constants ──────────────────────────────────
    private const string DbConnFileName = "db-conn.txt";
    private const string ConfigKey_DbConn = "ConnectionStrings:DefaultConnection";

    // ── IHostingProvider ─────────────────────────────────────────────

    public string ProviderName => "Render";

    /// <summary>
    /// Returns <c>true</c> when all four Render-specific environment variables
    /// are present and <c>IS_PULL_REQUEST</c> equals <c>"true"</c>.
    /// </summary>
    public bool IsActive()
    {
        return HasEnvVar(Env_IsPullRequest)
            && HasEnvVar(Env_GitRepoSlug)
            && HasEnvVar(Env_RenderServiceName)
            && HasEnvVar(Env_GithubPat)
            && string.Equals(
                   Environment.GetEnvironmentVariable(Env_IsPullRequest),
                   "true",
                   StringComparison.OrdinalIgnoreCase);
    }

    public async Task ConfigureAsync(WebApplicationBuilder builder)
    {
        var repoSlug    = GetRequiredEnvVar(Env_GitRepoSlug);
        var serviceName = GetRequiredEnvVar(Env_RenderServiceName);
        var githubPat   = GetRequiredEnvVar(Env_GithubPat);

        if (!TryGetPrNumber(serviceName, out var prNumber))
        {
            Log("Could not extract PR number from RENDER_SERVICE_NAME.");
            return;
        }

        var artifactName = $"pr-{prNumber}";
        var dbConn = await DownloadDbConnFromArtifactAsync(
            githubPat, repoSlug, artifactName);

        if (dbConn is null)
        {
            Log("DB connection override skipped — artifact not found or empty.");
            return;
        }

        builder.Configuration[ConfigKey_DbConn] = dbConn;
        Log("DB connection string overridden from GitHub artifact.");
    }

    // ── PR-number extraction ─────────────────────────────────────────

    /// <summary>
    /// Extracts the PR number from RENDER_SERVICE_NAME.
    /// Expected format: "campfitfurdogsapi-pr-209" → "209".
    /// </summary>
    private static bool TryGetPrNumber(
        string renderServiceName, out string? prNumber)
    {
        prNumber = null;
        var parts = renderServiceName.Split(
            ["-"], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return false;

        prNumber = parts.Last();
        return true;
    }

    // ── GitHub artifact download ─────────────────────────────────────

    private static async Task<string?> DownloadDbConnFromArtifactAsync(
        string githubToken, string repoSlug, string artifactName)
    {
        using var http = CreateGitHubClient(githubToken);

        var artifactsUrl =
            $"https://api.github.com/repos/{repoSlug}/actions/artifacts"
          + $"?per_page=100&name={artifactName}";

        var json = await http.GetStringAsync(artifactsUrl);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var response = JsonSerializer.Deserialize<ArtifactsResponse>(
            json, options);

        if (response?.Artifacts is not { Count: > 0 } artifacts)
        {
            Log($"No artifacts found matching '{artifactName}'.");
            return null;
        }

        // Single-pass O(n) selection — find the most recent artifact
        // without sorting the entire list.
        var latest = artifacts.Aggregate((newest, candidate) =>
            candidate.CreatedAt > newest.CreatedAt ? candidate : newest);

        var zipBytes = await http.GetByteArrayAsync(latest.ArchiveDownloadUrl);
        using var zip = new ZipArchive(new MemoryStream(zipBytes));
        var entry = zip.GetEntry(DbConnFileName);

        if (entry is null)
        {
            Log($"Artifact '{artifactName}' does not contain {DbConnFileName}.");
            return null;
        }

        using var reader = new StreamReader(entry.Open());
        var value = (await reader.ReadToEndAsync()).Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static HttpClient CreateGitHubClient(string token)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("CampFitFurDogs-Preview");
        http.DefaultRequestHeaders.Accept.ParseAdd(
            "application/vnd.github.v3+json");
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return http;
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private static bool HasEnvVar(string name)
        => !string.IsNullOrWhiteSpace(
               Environment.GetEnvironmentVariable(name));

    private static string GetRequiredEnvVar(string name)
        => Environment.GetEnvironmentVariable(name)
           ?? throw new InvalidOperationException(
                  $"Required environment variable '{name}' is not set.");

    private static void Log(string message)
        => Console.WriteLine($"[Hosting:Render] {message}");

    // ── DTOs ─────────────────────────────────────────────────────────

    private sealed class ArtifactsResponse
    {
        public List<Artifact>? Artifacts { get; set; }
    }

    private sealed class Artifact
    {
        public required string   Name               { get; set; }
        public required DateTime CreatedAt           { get; set; }
        public required string   ArchiveDownloadUrl  { get; set; }
    }
}
CSHARP_EOF
echo "[write]  src/CampFitFurDogs.Api/Hosting/RenderHostingProvider.cs"

# ── 5. Replace: Program.cs ───────────────────────────────────────
cat > src/CampFitFurDogs.Api/Program.cs << 'CSHARP_EOF'
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using CampFitFurDogs.Api.Hosting;
using CampFitFurDogs.Infrastructure;
using CampFitFurDogs.Infrastructure.Data;
using SharedKernel.Api.Hosting;
using SharedKernel.DependencyInjection;
using SharedKernel.Api;
using SharedKernel.Infrastructure.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Hosting-provider overrides (pluggable) ───────────────────────
// Add new providers here in priority order.  The first whose
// IsActive() returns true wins; the rest are skipped.
await builder.UseHostingProviders(
    new RenderHostingProvider());

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
// db context
builder.Services.AddInfrastructure(builder.Configuration);

//
// BEGIN SHARED KERNEL ASSISTED REGISTRATION
//

// 2. register ef core infrastructure layer:
//    unit of work
builder.Services.AddSharedKernelEfCore<AppDbContext>();

// 3. register application layer:
//    handlers, validators, dispatchers
builder.Services.AddSharedKernel(
    applicationAssemblies: new[]
    {
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
    },
    configure: options =>
    {
        // 4. register infrastructure layer:
        //    repositories, readers,
        //    providers, services
        options.AddInfrastructureAutoRegistration(
            assemblies: new[]
            {
                typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly
            },
            rules => rules
                .Add("Repository", ServiceLifetime.Scoped)
                .Add("Reader",     ServiceLifetime.Scoped)
                .Add("Provider",   ServiceLifetime.Scoped)
                .Add("Service",    ServiceLifetime.Scoped));
    });

// 5. register api layer:
//    endpoints
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
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?.Error;
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode  = StatusCodes.Status400BadRequest;
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
CSHARP_EOF
echo "[write]  src/CampFitFurDogs.Api/Program.cs  (replaced)"

# ── 6. Delete old files ──────────────────────────────────────────
OLD_DIR="src/CampFitFurDogs.Api/HostingEnvironment"
if [[ -d "$OLD_DIR" ]]; then
  rm -f "$OLD_DIR/EnvironmentBootstrapper.cs"
  rm -f "$OLD_DIR/PreviewDatabaseOverride.cs"
  rm -f "$OLD_DIR/Render.cs"
  rmdir --ignore-fail-on-non-empty "$OLD_DIR" 2>/dev/null || true
  echo "[delete] $OLD_DIR/EnvironmentBootstrapper.cs"
  echo "[delete] $OLD_DIR/PreviewDatabaseOverride.cs"
  echo "[delete] $OLD_DIR/Render.cs"
  if [[ ! -d "$OLD_DIR" ]]; then
    echo "[delete] $OLD_DIR/  (directory removed)"
  else
    echo "[warn]   $OLD_DIR/  still has files — not removed"
  fi
else
  echo "[skip]   $OLD_DIR/ not found (already removed?)"
fi

# ── 7. Summary ───────────────────────────────────────────────────
echo ""
echo "═══════════════════════════════════════════════════════════"
echo "  Done.  4 files written, 3 files deleted."
echo ""
echo "  New (SharedKernel.Api):"
echo "    src/SharedKernel.Api/Hosting/IHostingProvider.cs"
echo "    src/SharedKernel.Api/Hosting/HostingProviderExtensions.cs"
echo ""
echo "  New (CampFitFurDogs.Api):"
echo "    src/CampFitFurDogs.Api/Hosting/RenderHostingProvider.cs"
echo ""
echo "  Replaced:"
echo "    src/CampFitFurDogs.Api/Program.cs"
echo ""
echo "  Deleted:"
echo "    src/CampFitFurDogs.Api/HostingEnvironment/*"
echo ""
echo "  Next steps:"
echo "    1. dotnet build"
echo "    2. Review the diff:  git diff --stat"
echo "    3. Delete this script if you like"
echo "═══════════════════════════════════════════════════════════"
echo ""
