using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CampFitFurDogs.Api.HostingEnvironment;

public static class PreviewDatabaseOverride
{
    public static async Task ApplyAsync(WebApplicationBuilder builder)
    {
        var isPreview = Environment.GetEnvironmentVariable("IS_PULL_REQUEST") == "true";
        if (!isPreview)
            return;

        var branch = Environment.GetEnvironmentVariable("RENDER_GIT_BRANCH");
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_PAT");

        if (string.IsNullOrWhiteSpace(branch) || string.IsNullOrWhiteSpace(githubToken))
        {
            Console.WriteLine("[PR PREVIEW] Missing RENDER_GIT_BRANCH or GITHUB_PAT.");
            return;
        }

        try
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("CampFitFurDogs-Preview");
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", githubToken);

            var owner = builder.Configuration["GitHub:Owner"];
            var repo = builder.Configuration["GitHub:Repo"];

            if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repo))
            {
                Console.WriteLine("[PR PREVIEW] Missing GitHub:Owner or GitHub:Repo configuration.");
                return;
            }

            var artifactsUrl =
                $"https://api.github.com/repos/{owner}/{repo}/actions/artifacts?per_page=100";

            var artifactsJson = await http.GetStringAsync(artifactsUrl);
            var artifacts = JsonSerializer.Deserialize<GitHubArtifactsResponse>(artifactsJson);

            var artifactName = $"{branch}-db-conn";

            var artifact = artifacts?.Artifacts?
                .FirstOrDefault(a => a?.Name == artifactName);

            if (artifact is null)
            {
                Console.WriteLine($"[PR PREVIEW] No artifact found for branch '{branch}'.");
                return;
            }

            var zipBytes = await http.GetByteArrayAsync(artifact.ArchiveDownloadUrl);

            using var zip = new ZipArchive(new MemoryStream(zipBytes));
            var entry = zip.GetEntry($"{branch}-db-conn.txt");

            if (entry == null)
            {
                Console.WriteLine($"[PR PREVIEW] ZIP missing expected file for '{branch}'.");
                return;
            }

            using var reader = new StreamReader(entry.Open());
            var connString = reader.ReadToEnd().Trim();

            builder.Configuration["ConnectionStrings:DefaultConnection"] = connString;

            Console.WriteLine($"[PR PREVIEW] Loaded DB connection for branch '{branch}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PR PREVIEW] Failed to load DB override: {ex.Message}");
        }
    }

    private class GitHubArtifactsResponse
    {
        public List<GitHubArtifact>? Artifacts { get; set; }
    }

    private class GitHubArtifact
    {
        public string? Name { get; set; }
        public string? ArchiveDownloadUrl { get; set; }
    }
}
