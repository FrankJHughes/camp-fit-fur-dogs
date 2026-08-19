// tests/Frank/Core/Api.Tests/Routing/Validation/RouteHandlerBuilderExtensionsTests.cs

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Frank.Core.Api.Routing.Validation;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Api.Tests.Routing.Validation;

public class RouteHandlerBuilderExtensionsTests
{
    private class TestRequest
    {
        public string Name { get; set; } = "";
    }

    private class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private class TestObs : IRequestObservationContext
    {
        public string CorrelationId => "corr";
        public string Channel => "test";
        public string Agent => "agent";
        public string Environment => "env";
        public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
        public IReadOnlyDictionary<string, object?> Metadata => _metadata;
        private readonly Dictionary<string, object?> _metadata = new();
        public void AddMetadata(string key, object? value) => _metadata[key] = value;
        public string? UserId => null;
    }

    private HttpClient BuildClient()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSingleton<IValidator<TestRequest>, TestValidator>();
        builder.Services.AddSingleton<IRequestObservationContext, TestObs>();
        builder.Services.AddLogging();

        builder.WebHost.UseTestServer();

        var app = builder.Build();

        app.MapPost("/dogs", (TestRequest req) => Results.Ok())
           .WithValidation<TestRequest>();

        app.Start();

        return app.GetTestClient();
    }

    [Fact]
    public async Task WithValidation_InvalidRequestReturns400()
    {
        var client = BuildClient();

        var response = await client.PostAsJsonAsync("/dogs", new TestRequest { Name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task WithValidation_ValidRequestReturns200()
    {
        var client = BuildClient();

        var response = await client.PostAsJsonAsync("/dogs", new TestRequest { Name = "Doggo" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
