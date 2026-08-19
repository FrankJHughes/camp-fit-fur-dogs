using Frank.Core.Application.Abstractions.EnvironmentVariables;
using Frank.Core.Application.Abstractions.Hosting;
using Frank.Core.Infrastructure.EnvironmentVariables;
using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.Api.HostingModules;

/// <summary>
/// Hosting module that activates when running inside a Render PR Preview environment.
/// <para>
/// Render PR Preview deployments expose metadata such as the originating pull‑request
/// number, repository slug, and GitHub PAT via environment variables.
/// This module uses that metadata to dynamically fetch environment‑specific
/// configuration values (database connection string and frontend base URL) from
/// GitHub Actions artifacts produced during CI.
/// </para>
/// </summary>
[HostingModule(0)]
public sealed class RenderPrPreviewHostingModule : IHostingModule
{
    private const string ConfigKey_DbConn = "ConnectionStrings:DefaultConnection";
    private const string ConfigKey_FrontendBaseUrl = "Frontend:BaseUrl";

    private readonly IEnvironmentVariables _env;
    private readonly IRenderPrParser _prParser;
    private readonly IGitHubArtifactClient _artifacts;

    /// <summary>
    /// Allows tests to override the <see cref="HttpClient"/> factory used by
    /// <see cref="GitHubArtifactClient"/>.
    /// This is intentionally static to simplify test injection.
    /// </summary>
    public static Func<string, HttpClient>? HttpClientFactoryOverride { get; set; }

    private const string Env_IsPullRequest = "IS_PULL_REQUEST";
    private const string Env_GitRepoSlug = "RENDER_GIT_REPO_SLUG";
    private const string Env_RenderServiceName = "RENDER_SERVICE_NAME";
    private const string Env_GithubPat = "GITHUB_PAT";

    private const string DbConnFileName = "db-conn.txt";
    private const string FrontendUrlFileName = "frontend-url.txt";

    /// <summary>
    /// Gets the provider name used for diagnostics and hosting‑module identification.
    /// </summary>
    public string ProviderName => "Render";

    /// <summary>
    /// Creates a new <see cref="RenderPrPreviewHostingModule"/> using default
    /// environment variable access, PR parsing, and GitHub artifact retrieval.
    /// </summary>
    public RenderPrPreviewHostingModule() : this(
        new SystemEnvironmentVariables(),
        new RenderPrParser(),
        new GitHubArtifactClient())
    { }

    /// <summary>
    /// Creates a new <see cref="RenderPrPreviewHostingModule"/> with explicit
    /// dependencies.
    /// Useful for testing and advanced hosting customization.
    /// </summary>
    public RenderPrPreviewHostingModule(
        IEnvironmentVariables env,
        IRenderPrParser prParser,
        IGitHubArtifactClient artifacts)
    {
        _env = env;
        _prParser = prParser;
        _artifacts = artifacts;
    }

    /// <summary>
    /// Determines whether this hosting module should be active based on Render PR
    /// Preview environment variables.
    /// </summary>
    /// <param name="builder">The current <see cref="WebApplicationBuilder"/>.</param>
    /// <returns>
    /// <c>true</c> if all required Render PR Preview environment variables are present;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool IsActive(WebApplicationBuilder builder)
    {
        var isPr = _env.Get(Env_IsPullRequest);
        return string.Equals(isPr, "true", StringComparison.OrdinalIgnoreCase)
            && HasEnvVar(Env_GitRepoSlug)
            && HasEnvVar(Env_RenderServiceName)
            && HasEnvVar(Env_GithubPat);
    }

    /// <summary>
    /// Retrieves configuration overrides for Render PR Preview deployments by
    /// downloading PR‑specific GitHub Actions artifacts.
    /// <para>
    /// Expected artifacts:
    /// <list type="bullet">
    /// <item><description><c>pr-{number}-db/db-conn.txt</c></description></item>
    /// <item><description><c>pr-{number}-frontend/frontend-url.txt</c></description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="builder">The current <see cref="WebApplicationBuilder"/>.</param>
    /// <returns>
    /// A dictionary containing configuration keys and their overridden values.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required environment variables are missing, PR number cannot be
    /// parsed, or required artifact files cannot be retrieved.
    /// </exception>
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

    /// <summary>
    /// Determines whether the specified environment variable exists and is non‑empty.
    /// </summary>
    private bool HasEnvVar(string name)
        => !string.IsNullOrWhiteSpace(_env.Get(name));

    /// <summary>
    /// Retrieves a required environment variable or throws an exception if missing.
    /// </summary>
    private string GetRequiredEnvVar(string name)
        => _env.Get(name)
           ?? throw new InvalidOperationException(
               $"Required environment variable '{name}' is not set.");
}
