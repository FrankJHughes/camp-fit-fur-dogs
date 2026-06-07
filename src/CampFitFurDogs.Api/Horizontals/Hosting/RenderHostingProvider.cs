using Frank.Api.Hosting;
using Frank.Abstractions.Environment;

namespace CampFitFurDogs.Api.Horizontals.Hosting;

public sealed class RenderHostingProvider : IHostingProvider
{
    private readonly IEnvironment _env;
    private readonly IRenderPrParser _prParser;
    private readonly IGitHubArtifactClient _artifacts;
    private readonly IRenderConfigurationWriter _configWriter;

    // Kept for tests: allows overriding HttpClient factory indirectly
    public static Func<string, HttpClient>? HttpClientFactoryOverride { get; set; }

    private const string Env_IsPullRequest = "IS_PULL_REQUEST";
    private const string Env_GitRepoSlug = "RENDER_GIT_REPO_SLUG";
    private const string Env_RenderServiceName = "RENDER_SERVICE_NAME";
    private const string Env_GithubPat = "GITHUB_PAT";

    private const string DbConnFileName = "db-conn.txt";
    private const string FrontendUrlFileName = "frontend-url.txt";

    public string ProviderName => "Render";

    public RenderHostingProvider(
        IEnvironment env,
        IRenderPrParser prParser,
        IGitHubArtifactClient artifacts,
        IRenderConfigurationWriter configWriter)
    {
        _env = env;
        _prParser = prParser;
        _artifacts = artifacts;
        _configWriter = configWriter;
    }

    public bool IsActive()
    {
        var isPr = _env.Get(Env_IsPullRequest);
        return string.Equals(isPr, "true", StringComparison.OrdinalIgnoreCase)
            && HasEnvVar(Env_GitRepoSlug)
            && HasEnvVar(Env_RenderServiceName)
            && HasEnvVar(Env_GithubPat);
    }

    public async Task ConfigureAsync(WebApplicationBuilder builder)
    {
        var repoSlug = GetRequiredEnvVar(Env_GitRepoSlug);
        var serviceName = GetRequiredEnvVar(Env_RenderServiceName);
        var githubPat = GetRequiredEnvVar(Env_GithubPat);

        if (!_prParser.TryParse(serviceName, out var prNumber) || string.IsNullOrWhiteSpace(prNumber))
        {
            throw new InvalidOperationException(
                $"Render hosting provider is active, but could not extract PR number from '{Env_RenderServiceName}' value '{serviceName}'.");
        }

        var dbArtifactName = $"pr-{prNumber}-db";
        var frontendArtifactName = $"pr-{prNumber}-frontend";

        var dbConn = await _artifacts.GetArtifactFileAsync(
            githubPat, repoSlug, dbArtifactName, DbConnFileName);

        if (string.IsNullOrWhiteSpace(dbConn))
        {
            throw new InvalidOperationException(
                $"Render hosting provider could not load required database connection string from GitHub artifact '{dbArtifactName}/{DbConnFileName}'.");
        }

        var frontendUrl = await _artifacts.GetArtifactFileAsync(
            githubPat, repoSlug, frontendArtifactName, FrontendUrlFileName);

        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            throw new InvalidOperationException(
                $"Render hosting provider could not load required frontend base URL from GitHub artifact '{frontendArtifactName}/{FrontendUrlFileName}'.");
        }

        _configWriter.Apply(builder, dbConn, frontendUrl);
    }

    private bool HasEnvVar(string name)
        => !string.IsNullOrWhiteSpace(_env.Get(name));

    private string GetRequiredEnvVar(string name)
        => _env.Get(name)
           ?? throw new InvalidOperationException(
               $"Required environment variable '{name}' is not set.");
}
