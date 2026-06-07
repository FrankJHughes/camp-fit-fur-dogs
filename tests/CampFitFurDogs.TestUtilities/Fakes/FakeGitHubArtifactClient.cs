using System.IO.Compression;
using System.Text.Json;
using CampFitFurDogs.Api.Horizontals.Hosting;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeGitHubArtifactClient : IGitHubArtifactClient
{
    private readonly HttpMessageHandler? _handler;

    public FakeGitHubArtifactClient(HttpMessageHandler? handler)
    {
        _handler = handler;
    }

    public async Task<string?> GetArtifactFileAsync(
        string githubToken,
        string repoSlug,
        string artifactName,
        string fileName)
    {
        if (_handler is null)
            return null;

        using var http = new HttpClient(_handler);

        var url =
            $"https://api.github.com/repos/{repoSlug}/actions/artifacts" +
            $"?per_page=100&name={artifactName}";

        var json = await http.GetStringAsync(url);

        // IMPORTANT: match real provider's SnakeCaseLower policy
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var response = JsonSerializer.Deserialize<ArtifactsResponse>(json, options);

        if (response?.Artifacts is not { Count: > 0 } artifacts)
            return null;

        var latest = artifacts.OrderByDescending(a => a.CreatedAt).First();

        var zipBytes = await http.GetByteArrayAsync(latest.ArchiveDownloadUrl);
        using var zip = new ZipArchive(new MemoryStream(zipBytes));

        var entry = zip.GetEntry(fileName);
        if (entry is null)
            return null;

        using var reader = new StreamReader(entry.Open());
        var value = reader.ReadToEnd().Trim();

        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private sealed class ArtifactsResponse
    {
        public List<Artifact>? Artifacts { get; set; }
    }

    private sealed class Artifact
    {
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string ArchiveDownloadUrl { get; set; } = default!;
    }
}
