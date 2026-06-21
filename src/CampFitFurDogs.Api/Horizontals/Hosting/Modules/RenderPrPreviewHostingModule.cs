using Frank.Abstractions.Environment;
using Frank.Abstractions.Hosting;
using Frank.Infrastructure.Environment;

namespace CampFitFurDogs.Api.Horizontals.Hosting.Modules;

[HostingModule(0)]
public sealed class RenderPrPreviewHostingModule : IHostingModule
{
    private const string ConfigKey_DbConn = "ConnectionStrings:DefaultConnection";
    private const string ConfigKey_FrontendBaseUrl = "Frontend:BaseUrl";
    private readonly IEnvironment _env;
    private readonly IRenderPrParser _prParser;
    private readonly IGitHubArtifactClient _artifacts;

    // Kept for tests: allows overriding HttpClient factory indirectly
    public static Func<string, HttpClient>? HttpClientFactoryOverride { get; set; }

    private const string Env_IsPullRequest = "IS_PULL_REQUEST";
    private const string Env_GitRepoSlug = "RENDER_GIT_REPO_SLUG";
    private const string Env_RenderServiceName = "RENDER_SERVICE_NAME";
    private const string Env_GithubPat = "GITHUB_PAT";

    private const string DbConnFileName = "db-conn.txt";
    private const string FrontendUrlFileName = "frontend-url.txt";

    public string ProviderName => "Render";

    public RenderPrPreviewHostingModule() : this(
            new SystemEnvironment(),
            new RenderPrParser(),
            new GitHubArtifactClient())
    { }
    public RenderPrPreviewHostingModule(
        IEnvironment env,
        IRenderPrParser prParser,
        IGitHubArtifactClient artifacts)
    {
        _env = env;
        _prParser = prParser;
        _artifacts = artifacts;
    }

    public bool IsActive(WebApplicationBuilder builder)
    {
        var isPr = _env.Get(Env_IsPullRequest);
        return string.Equals(isPr, "true", StringComparison.OrdinalIgnoreCase)
            && HasEnvVar(Env_GitRepoSlug)
            && HasEnvVar(Env_RenderServiceName)
            && HasEnvVar(Env_GithubPat);
    }

    public async Task<IDictionary<string, string?>> GetConfigurationOverridesAsync(WebApplicationBuilder builder)
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

        return new Dictionary<string, string?>
        {
            [ConfigKey_DbConn] = dbConn,
            [ConfigKey_FrontendBaseUrl] = frontendUrl
        };
    }

    private bool HasEnvVar(string name)
        => !string.IsNullOrWhiteSpace(_env.Get(name));

    private string GetRequiredEnvVar(string name)
        => _env.Get(name)
           ?? throw new InvalidOperationException(
               $"Required environment variable '{name}' is not set.");

}
