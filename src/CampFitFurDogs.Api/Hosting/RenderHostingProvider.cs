using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
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
    private const string Env_IsPullRequest = "IS_PULL_REQUEST";
    private const string Env_GitRepoSlug = "RENDER_GIT_REPO_SLUG";
    private const string Env_RenderServiceName = "RENDER_SERVICE_NAME";
    private const string Env_GithubPat = "GITHUB_PAT";

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
        var repoSlug = GetRequiredEnvVar(Env_GitRepoSlug);
        var serviceName = GetRequiredEnvVar(Env_RenderServiceName);
        var githubPat = GetRequiredEnvVar(Env_GithubPat);

        if (!TryGetPrNumber(serviceName, out var prNumber))
        {
            Log("Could not extract PR number from RENDER_SERVICE_NAME.");
            return;
        }

        // CONTRACT:artifact-naming — shared with preview.yaml
        // See docs/conventions/workflow.md → "Artifact Naming Contract"
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
        public required string Name { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string ArchiveDownloadUrl { get; set; }
    }
}
