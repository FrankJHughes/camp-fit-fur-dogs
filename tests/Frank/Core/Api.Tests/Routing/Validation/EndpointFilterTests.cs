using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Frank.Core.Api.Routing.Validation;
using Frank.Core.Application.Abstractions.Observations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Frank.Core.Api.Tests.Routing.Validation;

public class EndpointFilterTests
{
    private class TestRequest { public string Name { get; set; } = ""; }

    private class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private class TestObs : IRequestObservationContext
    {
        public string CorrelationId => "corr-123";
        public string Channel => "test";
        public string Agent => "test-agent";
        public string Environment => "test-env";
        public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
        public IReadOnlyDictionary<string, object?> Metadata => _metadata;
        private readonly Dictionary<string, object?> _metadata = new();
        public void AddMetadata(string key, object? value) => _metadata[key] = value;
        public string? UserId => null;
    }

    [Fact]
    public async Task ValidRequest_AllowsPipelineContinuation()
    {
        var validator = new TestValidator();
        var logger = new LoggerFactory().CreateLogger<EndpointFilter<TestRequest>>();
        var obs = new TestObs();

        var filter = new EndpointFilter<TestRequest>(validator, logger, obs);

        var context = new DefaultEndpointFilterInvocationContext(
            new DefaultHttpContext(),
            new object[] { new TestRequest { Name = "Doggo" } });

        var nextCalled = false;

        var result = await filter.InvokeAsync(context, ctx =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(null);
        });

        Assert.True(nextCalled);
        Assert.Null(result);
    }

    [Fact]
    public async Task InvalidRequest_ReturnsValidationProblem()
    {
        var validator = new TestValidator();
        var logger = new LoggerFactory().CreateLogger<EndpointFilter<TestRequest>>();
        var obs = new TestObs();

        var filter = new EndpointFilter<TestRequest>(validator, logger, obs);

        var context = new DefaultEndpointFilterInvocationContext(
            new DefaultHttpContext(),
            new object[] { new TestRequest { Name = "" } });

        var result = await filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(null));

        // 1. Outer result type
        var problemResult = Assert.IsType<ProblemHttpResult>(result);

        // 2. Inner payload
        var problemDetails = Assert.IsType<HttpValidationProblemDetails>(problemResult.ProblemDetails);

        // 3. Validate error keys
        Assert.Contains("Name", problemDetails.Errors.Keys);
    }
}
