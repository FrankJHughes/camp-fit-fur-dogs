using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Frank.Core.Api.Routing.Validation;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Frank.Core.Api.Tests.Routing.Validation;

public class RouteGroupBuilderExtensionsTests
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

    private HttpClient BuildClient(bool includeValidator)
    {
        var builder = WebApplication.CreateBuilder();

        if (includeValidator)
            builder.Services.AddSingleton<IValidator<TestRequest>, TestValidator>();

        builder.Services.AddSingleton<IRequestObservationContext, TestObs>();
        builder.Services.AddLogging();

        builder.WebHost.UseTestServer();

        var app = builder.Build();

        var group = app.MapGroup("/dogs")
                       .AddRequestValidation();

        group.MapPost("/", (TestRequest req) => Results.Ok());

        app.Start();

        return app.GetTestClient();
    }

    [Fact]
    public async Task AddRequestValidation_WithValidator_InvalidRequestReturns400()
    {
        var client = BuildClient(includeValidator: true);

        var response = await client.PostAsJsonAsync("/dogs", new TestRequest { Name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddRequestValidation_WithoutValidator_InvalidRequestStillReturns200()
    {
        var client = BuildClient(includeValidator: false);

        var response = await client.PostAsJsonAsync("/dogs", new TestRequest { Name = "" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
