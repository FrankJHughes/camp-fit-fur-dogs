using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CampFitFurDogs.Api.Horizontals.Hosting.Modules;

public sealed class GitHubArtifactClient : IGitHubArtifactClient
{
    private readonly Func<string, HttpClient> _httpClientFactory;

    public GitHubArtifactClient(Func<string, HttpClient>? httpClientFactory = null)
    {
        _httpClientFactory = httpClientFactory ?? CreateGitHubClient;
    }

    public async Task<string?> GetArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName)
    {
        using var http = _httpClientFactory(githubToken);

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
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("CampFitFurDogs-Preview");
        http.DefaultRequestHeaders.Accept.ParseAdd(
            "application/vnd.github.v3+json");
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return http;
    }

    private static void Log(string message)
        => Console.WriteLine($"[Hosting:Render:GitHub] {message}");

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
