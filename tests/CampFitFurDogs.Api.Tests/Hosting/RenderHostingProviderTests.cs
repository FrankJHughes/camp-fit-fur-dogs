using System.IO.Compression;
using System.Net;
using CampFitFurDogs.Api.Hosting;
using CampFitFurDogs.TestUtilities.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;

namespace CampFitFurDogs.Api.Tests.Hosting;

public sealed class RenderHostingProviderTests
{
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
    // Provider builder
    // ------------------------------------------------------------
    private static RenderHostingProvider CreateProvider(
        FakeEnvironment env,
        FakeHttpMessageHandler handler)
    {
        var parser = new FakeRenderPrParser();
        var artifacts = new FakeGitHubArtifactClient(handler);
        var config = new FakeRenderConfigurationWriter();

        return new RenderHostingProvider(env, parser, artifacts, config);
    }

    // ------------------------------------------------------------
    // TESTS
    // ------------------------------------------------------------

    [Fact]
    public async Task Missing_required_env_var_throws()
    {
        var env = new FakeEnvironment();
        env.Set("IS_PULL_REQUEST", "true");
        env.Set("RENDER_GIT_REPO_SLUG", null);

        var provider = CreateProvider(env, new FakeHttpMessageHandler());
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*RENDER_GIT_REPO_SLUG*");
    }

    [Fact]
    public async Task Missing_pr_number_throws()
    {
        var env = new FakeEnvironment();
        env.Set("IS_PULL_REQUEST", "true");
        env.Set("RENDER_GIT_REPO_SLUG", "owner/repo");
        env.Set("RENDER_SERVICE_NAME", "api");
        env.Set("GITHUB_PAT", "token");

        var provider = CreateProvider(env, new FakeHttpMessageHandler());
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*PR number*");
    }

    [Fact]
    public async Task Missing_db_artifact_throws()
    {
        var env = new FakeEnvironment();
        env.Set("IS_PULL_REQUEST", "true");
        env.Set("RENDER_GIT_REPO_SLUG", "owner/repo");
        env.Set("RENDER_SERVICE_NAME", "api-pr-123");
        env.Set("GITHUB_PAT", "token");

        var handler = new FakeHttpMessageHandler();
        handler.Add(
            "https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-db",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"artifacts": []}""")
            });

        var provider = CreateProvider(env, handler);
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*database connection string*");
    }

    [Fact]
    public async Task Missing_frontend_artifact_throws()
    {
        var env = new FakeEnvironment();
        env.Set("IS_PULL_REQUEST", "true");
        env.Set("RENDER_GIT_REPO_SLUG", "owner/repo");
        env.Set("RENDER_SERVICE_NAME", "api-pr-123");
        env.Set("GITHUB_PAT", "token");

        var handler = new FakeHttpMessageHandler();

        handler.Add(
            "https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-db",
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
            });

        handler.Add("https://download/db", ZipWith("db-conn.txt", "Server=Test;"));

        handler.Add(
            "https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-frontend",
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"artifacts": []}""")
            });

        var provider = CreateProvider(env, handler);
        var builder = WebApplication.CreateBuilder();

        Func<Task> act = () => provider.ConfigureAsync(builder);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*frontend base URL*");
    }

    [Fact]
    public async Task Successful_configuration_sets_values()
    {
        var env = new FakeEnvironment();
        env.Set("IS_PULL_REQUEST", "true");
        env.Set("RENDER_GIT_REPO_SLUG", "owner/repo");
        env.Set("RENDER_SERVICE_NAME", "api-pr-123");
        env.Set("GITHUB_PAT", "token");

        var handler = new FakeHttpMessageHandler();

        handler.Add(
            "https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-db",
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
            });

        handler.Add("https://download/db", ZipWith("db-conn.txt", "Server=Test;"));

        handler.Add(
            "https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=pr-123-frontend",
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
            });

        handler.Add("https://download/frontend",
            ZipWith("frontend-url.txt", "https://preview.example.com"));

        var provider = CreateProvider(env, handler);
        var builder = WebApplication.CreateBuilder();

        await provider.ConfigureAsync(builder);

        builder.Configuration["ConnectionStrings:DefaultConnection"]
            .Should().Be("Server=Test;");

        builder.Configuration["Frontend__BaseUrl"]
            .Should().Be("https://preview.example.com");
    }
}
