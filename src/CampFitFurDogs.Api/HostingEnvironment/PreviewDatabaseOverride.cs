using System.Collections;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CampFitFurDogs.Api.HostingEnvironment;

public static class PreviewDatabaseOverride
{
    private const string RenderEnv_GitRepoSlug = "RENDER_GIT_REPO_SLUG";
    private const string RenderEnv_IsPullRequest = "IS_PULL_REQUEST";
    private const string RenderEnv_GithubPat = "GITHUB_PAT";
    private const string RenderEnv_RenderServiceName = "RENDER_SERVICE_NAME";
    private const string DbConnFileName = "db-conn.txt";
    private const string ConfigKey_DbConn = "ConnectionStrings:DefaultConnection";
    public static async Task ApplyAsync(WebApplicationBuilder builder)
    {
        string[] requiredEnvVarsArr = [
            RenderEnv_IsPullRequest,
            RenderEnv_GitRepoSlug,
            RenderEnv_GithubPat,
            RenderEnv_RenderServiceName];

        Dictionary<string, string?> requiredEnvVarsDict = requiredEnvVarsArr
            .ToDictionary(ev => ev, ev => (string?)null);

        if (!TryGetAllRequiredEnvVars(requiredEnvVarsDict))
        {
            LogFailure($"The attempt to get all required environment variables failed.");
            return;
        }

        if (!requiredEnvVarsDict[RenderEnv_IsPullRequest]!.Equals("true"))
        {
            LogFailure("The environment is not a pull request environment.");
            return;
        }

        try
        {
            if (!Render.TryGetPrNumber(
                requiredEnvVarsDict[RenderEnv_RenderServiceName]!,
                out var prNumber))
            {
                LogFailure("The attempt to get the pull request number failed.");
                return;
            }

            var artifactName = $"pr-{prNumber}";

            var dbConn = await GetDbConn(requiredEnvVarsDict[RenderEnv_GithubPat]!,
                requiredEnvVarsDict[RenderEnv_GitRepoSlug]!, artifactName);
            if (dbConn is null)
            {
                LogFailure("The attempt to get the DB connection override failed.");
                return;
            }

            builder.Configuration[ConfigKey_DbConn] = dbConn;

            LogDbConnOverrideSucceeded();
        }
        catch (Exception ex)
        {
            LogFailure(ex.Message);
        }
    }

    private static void LogFailure(string message)
    {
        Console.WriteLine($"[PR PREVIEW] The attempt to override the DB connection failed: {message}");
    }

    private static void LogDbConnOverrideSucceeded()
    {
        Console.WriteLine($"[PR PREVIEW] The attempt to override the DB connection succeeded.");
    }

    private static bool TryGetAllRequiredEnvVars(Dictionary<string, string?> requiredEnvVarsDict)
    {
        foreach (var envVar in requiredEnvVarsDict)
        {
            if (!TryGetEnvVar(envVar.Key, out var value))
            {
                return false;
            }

            requiredEnvVarsDict[envVar.Key] = value!;
        }

        return true;
    }

    private static void LogEnvVarUndefined(string envVarName)
    {
        LogFailure($"The attempt to get the environment variable {envVarName} failed.");
    }

    private static async Task<string?> GetDbConn(string githubToken, string repoSlug, string artifactName)
    {
        using var http = GetHttpClient(githubToken!);
        var artifactResponse = await GetArtifactResponse(repoSlug!, artifactName, http);
        if (artifactResponse is null)
        {
            return null;
        }

        if (!TryGetArtifactList(artifactName, artifactResponse!, out var artifactList))
        {
            return null;
        }

        if (!TryGetArtifact(artifactName, artifactList!, out var artifact))
        {
            return null;
        }

        var entry = await GetArtifactFile(http, artifact!);
        if (entry is null)
        {
            return null;
        }

        if (!TryGetDbConn(entry!, out var dbConn))
        {
            return null;
        }

        return dbConn;
    }

    private static bool TryGetArtifactList(string artifactName, GitHubArtifactsResponse artifactResponse, out List<GitHubArtifact>? artifactList)
    {
        artifactList = artifactResponse!.Artifacts;
        if (artifactList is null || artifactList.Count == 0)
        {
            LogFailure($"The attempt to get the artifact list for artifact {artifactName} failed.");
            return false;
        }

        return true;
    }

    private static bool TryGetArtifact(string artifactName, List<GitHubArtifact> artifactList, out GitHubArtifact? artifact)
    {
        artifact = artifactList.Aggregate((latest, current) =>
            current.CreatedAt > latest.CreatedAt ? current : latest);

        if (artifact is null)
        {
            LogFailure($"The attempt to get artifact {artifactName} failed.");
            return false;
        }

        return true;
    }

    private static bool TryGetDbConn(ZipArchiveEntry entry, out string? dbConn)
    {
        var reader = new StreamReader(entry!.Open());
        dbConn = reader.ReadToEnd();
        if (string.IsNullOrWhiteSpace(dbConn))
        {
            LogFailure($"The DB connection is not present in the {entry.Name}.");
            return false;
        }

        dbConn = dbConn.Trim();

        return true;
    }

    private static async Task<ZipArchiveEntry?> GetArtifactFile(HttpClient http, GitHubArtifact artifact)
    {
        var zipBytes = await http.GetByteArrayAsync(artifact.ArchiveDownloadUrl);
        var zip = new ZipArchive(new MemoryStream(zipBytes));
        var entry = zip.GetEntry(DbConnFileName);
        if (entry is null)
        {
            LogFailure($"The attempt to get the artifact file {DbConnFileName} from artifact {artifact.Name} failed.");
            return null;
        }

        return entry;
    }

    private static async Task<GitHubArtifactsResponse?> GetArtifactResponse(string repoSlug, string artifactName, HttpClient http)
    {
        var artifactsUrl =
            $"https://api.github.com/repos/{repoSlug}/actions/artifacts?per_page=100&name={artifactName}";

        var artifactsJson = await http.GetStringAsync(artifactsUrl);

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        return JsonSerializer.Deserialize<GitHubArtifactsResponse>(artifactsJson, serializerOptions);
    }

    private static HttpClient GetHttpClient(string githubToken)
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("CampFitFurDogs-Preview");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
        http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", githubToken);
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2026-03-10");
        return http;
    }

    private static bool TryGetEnvVar(string envVarName, out string? value)
    {
        value = Environment.GetEnvironmentVariable(envVarName);
        if (string.IsNullOrWhiteSpace(value))
        {
            LogEnvVarUndefined(envVarName);
            return false;
        }

        return true;
    }

    private class GitHubArtifactsResponse
    {
        public List<GitHubArtifact>? Artifacts { get; set; }
    }

    private class GitHubArtifact
    {
        public required string Name { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string ArchiveDownloadUrl { get; set; }
    }
}
