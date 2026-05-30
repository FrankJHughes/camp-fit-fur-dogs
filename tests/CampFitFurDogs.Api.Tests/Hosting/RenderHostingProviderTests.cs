using System.IO.Compression;
using System.Net;
using System.Text;
using CampFitFurDogs.Api.Hosting;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.Api.Tests.Hosting;

public sealed class RenderHostingProviderTests
{
    // ------------------------------------------------------------
    // Fake HTTP handler for GitHub API responses
    // ------------------------------------------------------------
    private sealed class FakeHttpHandler : HttpMessageHandler
    {
        public Dictionary<string, HttpResponseMessage> Map { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (Map.TryGetValue(request.RequestUri!.ToString(), out var response))
                return Task.FromResult(response);

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }

    // ------------------------------------------------------------
    // Environment variable helper
    // ------------------------------------------------------------
    private IDisposable SetEnv(string key, string? value)
    {
        var original = Environment.GetEnvironmentVariable(key);
        Environment.SetEnvironmentVariable(key, value);
        return new EnvReset(key, original);
    }

    private sealed class EnvReset : IDisposable
    {
        private readonly string _key;
        private readonly string? _value;

        public EnvReset(string key, string? value)
        {
            _key = key;
            _value = value;
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(_key, _value);
        }
    }

    // ------------------------------------------------------------
    // ZIP builder helper
    // ------------------------------------------------------------
    private static HttpResponseMessage ZipWith(string fileName, string content)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            var entry = zip.CreateEntry(fileName);
            using var writer = new StreamWriter(entry.Open());
            writer.Write(content);
        }

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(ms.ToArray())
        };
    }

    // ------------------------------------------------------------
    // TESTS
    // ------------------------------------------------------------

    [Fact]
    public async Task Missing_required_env_var_throws()
    {
        using var _1 = SetEnv("IS_PULL_REQUEST", "true");
        using var _2 = SetEnv("RENDER_GIT_REPO_SLUG", null); // missing

        var provider = new RenderHostingProvider();
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*RENDER_GIT_REPO_SLUG*");
    }

    [Fact]
    public async Task Missing_pr_number_throws()
    {
        using var _1 = SetEnv("IS_PULL_REQUEST", "true");
        using var _2 = SetEnv("RENDER_GIT_REPO_SLUG", "owner/repo");
        using var _3 = SetEnv("RENDER_SERVICE_NAME", "api"); // no PR suffix
        using var _4 = SetEnv("GITHUB_PAT", "token");

        var provider = new RenderHostingProvider();
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*PR number*");
    }

    [Fact]
    public async Task Missing_db_artifact_throws()
    {
        using var _1 = SetEnv("IS_PULL_REQUEST", "true");
        using var _2 = SetEnv("RENDER_GIT_REPO_SLUG", "owner/repo");
        using var _3 = SetEnv("RENDER_SERVICE_NAME", "api-pr-123");
        using var _4 = SetEnv("GITHUB_PAT", "token");

        var handler = new FakeHttpHandler();

        handler.Map["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-db"] =
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"artifacts": []}""")
            };

        RenderHostingProvider.HttpClientFactoryOverride = _ => new HttpClient(handler);

        var provider = new RenderHostingProvider();
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*database connection string*");

        RenderHostingProvider.HttpClientFactoryOverride = null;
    }

    [Fact]
    public async Task Missing_frontend_artifact_throws()
    {
        using var _1 = SetEnv("IS_PULL_REQUEST", "true");
        using var _2 = SetEnv("RENDER_GIT_REPO_SLUG", "owner/repo");
        using var _3 = SetEnv("RENDER_SERVICE_NAME", "api-pr-123");
        using var _4 = SetEnv("GITHUB_PAT", "token");

        var handler = new FakeHttpHandler();

        // DB artifact exists
        handler.Map["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-db"] =
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                    "artifacts": [
                        {
                            "name": "pr-123-db",
                            "created_at": "2024-01-01T00:00:00Z",
                            "archive_download_url": "https://download/db"
                        }
                    ]
                }
                """)
            };

        handler.Map["https://download/db"] = ZipWith("db-conn.txt", "Server=Test;");

        // Frontend artifact missing
        handler.Map["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-frontend"] =
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"artifacts": []}""")
            };

        RenderHostingProvider.HttpClientFactoryOverride = _ => new HttpClient(handler);

        var provider = new RenderHostingProvider();
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*frontend base URL*");

        RenderHostingProvider.HttpClientFactoryOverride = null;
    }

    [Fact]
    public async Task Successful_configuration_sets_values()
    {
        using var _1 = SetEnv("IS_PULL_REQUEST", "true");
        using var _2 = SetEnv("RENDER_GIT_REPO_SLUG", "owner/repo");
        using var _3 = SetEnv("RENDER_SERVICE_NAME", "api-pr-123");
        using var _4 = SetEnv("GITHUB_PAT", "token");

        var handler = new FakeHttpHandler();

        // DB artifact
        handler.Map["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-db"] =
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                    "artifacts": [
                        {
                            "name": "pr-123-db",
                            "created_at": "2024-01-01T00:00:00Z",
                            "archive_download_url": "https://download/db"
                        }
                    ]
                }
                """)
            };

        handler.Map["https://download/db"] = ZipWith("db-conn.txt", "Server=Test;");

        // Frontend artifact
        handler.Map["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-frontend"] =
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""
                {
                    "artifacts": [
                        {
                            "name": "pr-123-frontend",
                            "created_at": "2024-01-01T00:00:00Z",
                            "archive_download_url": "https://download/frontend"
                        }
                    ]
                }
                """)
            };

        handler.Map["https://download/frontend"] =
            ZipWith("frontend-url.txt", "https://preview.example.com");

        RenderHostingProvider.HttpClientFactoryOverride = _ => new HttpClient(handler);

        var provider = new RenderHostingProvider();
        var builder = WebApplication.CreateBuilder();

        await provider.ConfigureAsync(builder);

        builder.Configuration["ConnectionStrings:DefaultConnection"]
            .Should().Be("Server=Test;");

        builder.Configuration["Frontend__BaseUrl"]
            .Should().Be("https://preview.example.com");

        RenderHostingProvider.HttpClientFactoryOverride = null;
    }
}
