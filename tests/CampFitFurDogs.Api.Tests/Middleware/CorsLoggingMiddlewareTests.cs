using CampFitFurDogs.Api.Horizontal.Cors.Middleware;
using CampFitFurDogs.TestUtilities.Fakes;
using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CampFitFurDogs.Api.Tests.Middleware;

public class CorsLoggingMiddlewareTests
{
    private static CorsLoggingMiddleware CreateMiddleware(
        CorsPolicy policy,
        TestLogger<CorsLoggingMiddleware> logger)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ICorsPolicyProvider>(new FakeCorsPolicyProvider(policy));
        services.AddSingleton<ILogger<CorsLoggingMiddleware>>(logger);

        var provider = services.BuildServiceProvider();

        return new CorsLoggingMiddleware(
            _ => Task.CompletedTask,
            provider.GetRequiredService<ILogger<CorsLoggingMiddleware>>(),
            provider.GetRequiredService<ICorsPolicyProvider>());
    }

    private static DefaultHttpContext CreateContext(
        string? origin = null,
        string method = "GET",
        string? accessControlRequestMethod = null,
        string path = "/api/test")
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Method = method;
        ctx.Request.Path = path;

        if (origin is not null)
            ctx.Request.Headers["Origin"] = origin;

        if (accessControlRequestMethod is not null)
            ctx.Request.Headers["Access-Control-Request-Method"] = accessControlRequestMethod;

        return ctx;
    }

    // ------------------------------------------------------------
    // ALLOWED ORIGIN
    // ------------------------------------------------------------
    [Fact]
    public async Task Logs_allowed_origin_request()
    {
        var logger = new TestLogger<CorsLoggingMiddleware>();
        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        var middleware = CreateMiddleware(policy, logger);
        var ctx = CreateContext(origin: "https://allowed.com", method: "GET");

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS request allowed"));
    }

    // ------------------------------------------------------------
    // BLOCKED ORIGIN
    // ------------------------------------------------------------
    [Fact]
    public async Task Logs_blocked_origin_request()
    {
        var logger = new TestLogger<CorsLoggingMiddleware>();
        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        var middleware = CreateMiddleware(policy, logger);
        var ctx = CreateContext(origin: "https://evil.com", method: "POST", path: "/api/orders");

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS request blocked"));
    }

    // ------------------------------------------------------------
    // ALLOWED PREFLIGHT
    // ------------------------------------------------------------
    [Fact]
    public async Task Logs_allowed_preflight()
    {
        var logger = new TestLogger<CorsLoggingMiddleware>();
        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        var middleware = CreateMiddleware(policy, logger);
        var ctx = CreateContext(
            origin: "https://allowed.com",
            method: "OPTIONS",
            accessControlRequestMethod: "POST");

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS preflight allowed"));
    }

    // ------------------------------------------------------------
    // BLOCKED PREFLIGHT
    // ------------------------------------------------------------
    [Fact]
    public async Task Logs_blocked_preflight()
    {
        var logger = new TestLogger<CorsLoggingMiddleware>();
        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        var middleware = CreateMiddleware(policy, logger);
        var ctx = CreateContext(
            origin: "https://evil.com",
            method: "OPTIONS",
            accessControlRequestMethod: "POST");

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().Contain(m => m.Contains("CORS preflight blocked"));
    }

    // ------------------------------------------------------------
    // NO ORIGIN HEADER
    // ------------------------------------------------------------
    [Fact]
    public async Task Logs_nothing_when_no_origin_header()
    {
        var logger = new TestLogger<CorsLoggingMiddleware>();
        var policy = new CorsPolicyBuilder()
            .WithOrigins("https://allowed.com")
            .Build();

        var middleware = CreateMiddleware(policy, logger);
        var ctx = CreateContext(origin: null, method: "GET");

        await middleware.InvokeAsync(ctx);

        logger.Messages.Should().BeEmpty();
    }
}
