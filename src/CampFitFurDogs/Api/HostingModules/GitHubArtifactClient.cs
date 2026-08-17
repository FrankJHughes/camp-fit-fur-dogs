using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CampFitFurDogs.Api.HostingModules;

/// <summary>
/// Provides functionality for retrieving and extracting files from GitHub Actions
/// artifacts using the GitHub REST API.
/// <para>
/// This client is used by hosting modules (such as <c>RenderPrPreviewHostingModule</c>)
/// to dynamically fetch environment‑specific configuration files produced during CI.
/// </para>
/// </summary>
public sealed class GitHubArtifactClient : IGitHubArtifactClient
{
    private readonly Func<string, HttpClient> _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of <see cref="GitHubArtifactClient"/>.
    /// </summary>
    /// <param name="httpClientFactory">
    /// Optional factory for creating <see cref="HttpClient"/> instances.
    /// If omitted, a GitHub‑configured client is created automatically.
    /// </param>
    public GitHubArtifactClient(Func<string, HttpClient>? httpClientFactory = null)
    {
        _httpClientFactory = httpClientFactory ?? CreateGitHubClient;
    }

    /// <summary>
    /// Retrieves a specific file from the latest GitHub Actions artifact matching
    /// the provided artifact name.
    /// </summary>
    /// <param name="githubToken">A GitHub token with permission to access artifacts.</param>
    /// <param name="repoSlug">The repository slug (e.g., <c>owner/repo</c>).</param>
    /// <param name="artifactName">The name of the artifact to search for.</param>
    /// <param name="fileName">The file inside the artifact ZIP archive to extract.</param>
    /// <returns>
    /// The file contents as a trimmed string, or <c>null</c> if the artifact or file
    /// cannot be found.
    /// </returns>
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

        // Select the newest artifact by CreatedAt timestamp
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

    /// <summary>
    /// Creates a GitHub‑configured <see cref="HttpClient"/> using the provided token.
    /// </summary>
    /// <param name="token">The GitHub API token.</param>
    /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
    private static HttpClient CreateGitHubClient(string token)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("CampFitFurDogs-Preview");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return http;
    }

    /// <summary>
    /// Writes diagnostic messages to the console for hosting‑related operations.
    /// </summary>
    /// <param name="message">The message to log.</param>
    private static void Log(string message)
        => Console.WriteLine($"[Hosting:Render:GitHub] {message}");

    /// <summary>
    /// Represents the GitHub API response containing a list of artifacts.
    /// </summary>
    private sealed class ArtifactsResponse
    {
        public List<Artifact>? Artifacts { get; set; }
    }

    /// <summary>
    /// Represents a single GitHub Actions artifact entry.
    /// </summary>
    private sealed class Artifact
    {
        public required string Name { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string ArchiveDownloadUrl { get; set; }
    }
}
