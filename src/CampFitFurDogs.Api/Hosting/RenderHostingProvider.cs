using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Frank.Api.Hosting;

namespace CampFitFurDogs.Api.Hosting;

public sealed class RenderHostingProvider : IHostingProvider
{
    public static Func<string, HttpClient>? HttpClientFactoryOverride { get; set; }

    private const string Env_IsPullRequest = "IS_PULL_REQUEST";
    private const string Env_GitRepoSlug = "RENDER_GIT_REPO_SLUG";
    private const string Env_RenderServiceName = "RENDER_SERVICE_NAME";
    private const string Env_GithubPat = "GITHUB_PAT";

    private const string DbConnFileName = "db-conn.txt";
    private const string FrontendUrlFileName = "frontend-url.txt";

    private const string ConfigKey_DbConn = "ConnectionStrings:DefaultConnection";
    private const string ConfigKey_FrontendBaseUrl = "Frontend__BaseUrl";

    public string ProviderName => "Render";

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
            // 🔥 Harden: if Render says it's active but we can't parse PR, fail fast
            throw new InvalidOperationException(
                $"Render hosting provider is active, but could not extract PR number from '{Env_RenderServiceName}' value '{serviceName}'.");
        }

        var dbArtifactName = $"pr-{prNumber}-db";
        var frontendArtifactName = $"pr-{prNumber}-frontend";

        // DB connection string is REQUIRED
        var dbConn = await DownloadSingleArtifactFileAsync(
            githubPat, repoSlug, dbArtifactName, DbConnFileName);

        if (string.IsNullOrWhiteSpace(dbConn))
        {
            throw new InvalidOperationException(
                $"Render hosting provider could not load required database connection string from GitHub artifact '{dbArtifactName}/{DbConnFileName}'.");
        }

        builder.Configuration[ConfigKey_DbConn] = dbConn;
        Log("DB connection string overridden from GitHub artifact.");

        // Frontend base URL is REQUIRED
        var frontendUrl = await DownloadSingleArtifactFileAsync(
            githubPat, repoSlug, frontendArtifactName, FrontendUrlFileName);

        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            throw new InvalidOperationException(
                $"Render hosting provider could not load required frontend base URL from GitHub artifact '{frontendArtifactName}/{FrontendUrlFileName}'.");
        }

        builder.Configuration[ConfigKey_FrontendBaseUrl] = frontendUrl;
        Log($"Frontend base URL overridden from GitHub artifact: {frontendUrl}.");
    }

    private static bool TryGetPrNumber(string renderServiceName, out string? prNumber)
    {
        prNumber = null;
        var parts = renderServiceName.Split(
            ["-"], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return false;

        prNumber = parts.Last();
        return true;
    }

    private static async Task<string?> DownloadSingleArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName)
    {
        using var http = CreateGitHubClient(githubToken);

        var artifactsUrl =
            $"https://api.github.com/repos/{repoSlug}/actions/artifacts" +
            $"?per_page=100&name={artifactName}";

        var json = await http.GetStringAsync(artifactsUrl);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var response = JsonSerializer.Deserialize<ArtifactsResponse>(json, options);

        if (response?.Artifacts is not { Count: > 0 } artifacts)
        {
            Log($"No artifacts found matching '{artifactName}'.");
            return null;
        }

        var latest = artifacts.Aggregate((newest, candidate) =>
            candidate.CreatedAt > newest.CreatedAt ? candidate : newest);

        var zipBytes = await http.GetByteArrayAsync(latest.ArchiveDownloadUrl);
        using var zip = new ZipArchive(new MemoryStream(zipBytes));

        var entry = zip.GetEntry(fileName);
        if (entry is null)
        {
            Log($"Artifact '{artifactName}' does not contain file '{fileName}'.");
            return null;
        }

        using var reader = new StreamReader(entry.Open());
        var value = reader.ReadToEnd().Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static HttpClient CreateGitHubClient(string token)
    {
        if (HttpClientFactoryOverride is not null)
            return HttpClientFactoryOverride(token);

        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("CampFitFurDogs-Preview");
        http.DefaultRequestHeaders.Accept.ParseAdd(
            "application/vnd.github.v3+json");
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return http;
    }

    private static bool HasEnvVar(string name)
        => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name));

    private static string GetRequiredEnvVar(string name)
        => Environment.GetEnvironmentVariable(name)
           ?? throw new InvalidOperationException(
               $"Required environment variable '{name}' is not set.");

    private static void Log(string message)
        => Console.WriteLine($"[Hosting:Render] {message}");

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
