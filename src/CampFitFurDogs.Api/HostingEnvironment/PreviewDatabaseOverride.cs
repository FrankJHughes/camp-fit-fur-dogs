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

        var prNumber = Render.GetPrNumber();
        if (string.IsNullOrWhiteSpace(prNumber))
        {
            Console.WriteLine("[PR PREVIEW] Could not extract PR number from RENDER_EXTERNAL_URL.");
            return;
        }

        var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrWhiteSpace(githubToken))
        {
            Console.WriteLine("[PR PREVIEW] Missing GITHUB_TOKEN.");
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

            if (artifacts?.Artifacts == null)
            {
                Console.WriteLine("[PR PREVIEW] GitHub API returned no artifacts.");
                return;
            }

            var artifactName = $"pr-{prNumber}";
            var artifact = artifacts.Artifacts
                .FirstOrDefault(a => a.Name == artifactName);

            if (artifact == null)
            {
                Console.WriteLine($"[PR PREVIEW] No artifact found for PR #{prNumber}.");
                return;
            }

            var zipBytes = await http.GetByteArrayAsync(artifact.ArchiveDownloadUrl);

            using var zip = new ZipArchive(new MemoryStream(zipBytes));
            var entry = zip.GetEntry("db-conn.txt");

            if (entry == null)
            {
                Console.WriteLine($"[PR PREVIEW] Artifact pr-{prNumber} missing db-conn.txt.");
                return;
            }

            using var reader = new StreamReader(entry.Open());
            var connString = reader.ReadToEnd().Trim();

            builder.Configuration["ConnectionStrings:DefaultConnection"] = connString;

            Console.WriteLine($"[PR PREVIEW] Loaded DB connection for PR #{prNumber}.");
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
