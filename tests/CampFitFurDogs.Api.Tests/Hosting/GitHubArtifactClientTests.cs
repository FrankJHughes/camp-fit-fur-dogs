using System.IO.Compression;
using System.Net;
using CampFitFurDogs.Api.Hosting;
using CampFitFurDogs.TestUtilities.Fakes;
using FluentAssertions;

namespace CampFitFurDogs.Api.Tests.Hosting;

public sealed class GitHubArtifactClientTests
{
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

    [Fact]
    public async Task Returns_null_when_no_artifacts_found()
    {
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=test"] =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"artifacts": []}""")
                }
        };

        var handler = new FakeHttpMessageHandler(responses);
        var client = new GitHubArtifactClient(_ => new HttpClient(handler));

        var result = await client.GetArtifactFileAsync("token", "owner/repo", "test", "file.txt");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Returns_null_when_zip_missing_file()
    {
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=test"] =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                        "artifacts": [
                            {
                                "name": "test",
                                "created_at": "2024-01-01T00:00:00Z",
                                "archive_download_url": "https://download/zip"
                            }
                        ]
                    }
                    """)
                },
            ["https://download/zip"] = ZipWith("other.txt", "nope")
        };

        var handler = new FakeHttpMessageHandler(responses);
        var client = new GitHubArtifactClient(_ => new HttpClient(handler));

        var result = await client.GetArtifactFileAsync("token", "owner/repo", "test", "file.txt");

        result.Should().BeNull();
    }

    [Fact]
    public async Task Extracts_file_contents_from_zip()
    {
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=test"] =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                        "artifacts": [
                            {
                                "name": "test",
                                "created_at": "2024-01-01T00:00:00Z",
                                "archive_download_url": "https://download/zip"
                            }
                        ]
                    }
                    """)
                },
            ["https://download/zip"] = ZipWith("file.txt", "Hello!")
        };

        var handler = new FakeHttpMessageHandler(responses);
        var client = new GitHubArtifactClient(_ => new HttpClient(handler));

        var result = await client.GetArtifactFileAsync("token", "owner/repo", "test", "file.txt");

        result.Should().Be("Hello!");
    }

    [Fact]
    public async Task Selects_newest_artifact_when_multiple_exist()
    {
        var responses = new Dictionary<string, HttpResponseMessage>
        {
            ["https://api.github.com/repos/owner/repo/actions/artifacts?per_page=100&name=test"] =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                        "artifacts": [
                            {
                                "name": "old",
                                "created_at": "2023-01-01T00:00:00Z",
                                "archive_download_url": "https://download/old"
                            },
                            {
                                "name": "new",
                                "created_at": "2024-01-01T00:00:00Z",
                                "archive_download_url": "https://download/new"
                            }
                        ]
                    }
                    """)
                },
            ["https://download/new"] = ZipWith("file.txt", "NEW")
        };

        var handler = new FakeHttpMessageHandler(responses);
        var client = new GitHubArtifactClient(_ => new HttpClient(handler));

        var result = await client.GetArtifactFileAsync("token", "owner/repo", "test", "file.txt");

        result.Should().Be("NEW");
    }
}
