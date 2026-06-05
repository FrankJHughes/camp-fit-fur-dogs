using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Frank.Api.SecurityHeaders;
using Xunit;

namespace Frank.Api.Tests.SecurityHeaders;

public sealed class SecurityHeadersMiddlewareTests
{
    private static SecurityHeadersMiddleware Create() => new();

    [Fact]
    public async Task Adds_all_expected_headers()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        var h = context.Response.Headers;

        h["X-Content-Type-Options"].ToString().Should().Be("nosniff");
        h["X-Frame-Options"].ToString().Should().Be("DENY");
        h["X-XSS-Protection"].ToString().Should().Be("0");
        h["Referrer-Policy"].ToString().Should().Be("strict-origin-when-cross-origin");

        h["Permissions-Policy"].ToString().Should().Contain("geolocation=()");
        h["Permissions-Policy"].ToString().Should().Contain("microphone=()");
        h["Permissions-Policy"].ToString().Should().Contain("camera=()");
        h["Permissions-Policy"].ToString().Should().Contain("payment=()");
        h["Permissions-Policy"].ToString().Should().Contain("usb=()");

        h["Cross-Origin-Opener-Policy"].ToString().Should().Be("same-origin");
        h["Cross-Origin-Embedder-Policy"].ToString().Should().Be("require-corp");
        h["Cross-Origin-Resource-Policy"].ToString().Should().Be("same-origin");

        var csp = h["Content-Security-Policy"].ToString();
        csp.Should().Contain("default-src 'self'");
        csp.Should().Contain("script-src 'self'");
        csp.Should().Contain("style-src 'self'");
        csp.Should().Contain("img-src 'self' data:");
        csp.Should().Contain("font-src 'self'");
        csp.Should().Contain("connect-src 'self'");
        csp.Should().Contain("frame-ancestors 'none'");
        csp.Should().Contain("object-src 'none'");
        csp.Should().Contain("base-uri 'self'");
        csp.Should().Contain("form-action 'self'");
    }

    [Fact]
    public async Task Does_not_overwrite_existing_headers()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        context.Response.Headers["X-Frame-Options"]
            .ToString()
            .Should()
            .Be("SAMEORIGIN");
    }

    [Fact]
    public async Task Adds_headers_even_when_next_throws()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        Func<Task> act = () =>
            middleware.InvokeAsync(context, _ => throw new Exception("boom"));

        await act.Should().ThrowAsync<Exception>();

        context.Response.Headers["X-Content-Type-Options"]
            .ToString()
            .Should()
            .Be("nosniff");
    }

    [Fact]
    public async Task Adds_headers_to_redirect_responses()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, ctx =>
        {
            ctx.Response.StatusCode = 302;
            return Task.CompletedTask;
        });

        context.Response.Headers["X-Frame-Options"]
            .ToString()
            .Should()
            .Be("DENY");
    }

    [Fact]
    public async Task Csp_contains_required_directives()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        var csp = context.Response.Headers["Content-Security-Policy"].ToString();

        csp.Should().Contain("default-src 'self'");
        csp.Should().Contain("script-src 'self'");
        csp.Should().Contain("style-src 'self'");
        csp.Should().Contain("img-src 'self' data:");
        csp.Should().Contain("font-src 'self'");
        csp.Should().Contain("connect-src 'self'");
        csp.Should().Contain("frame-ancestors 'none'");
        csp.Should().Contain("object-src 'none'");
        csp.Should().Contain("base-uri 'self'");
        csp.Should().Contain("form-action 'self'");
    }

    [Fact]
    public async Task Permissions_policy_is_strict()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        var policy = context.Response.Headers["Permissions-Policy"].ToString();

        policy.Should().Contain("geolocation=()");
        policy.Should().Contain("microphone=()");
        policy.Should().Contain("camera=()");
        policy.Should().Contain("payment=()");
        policy.Should().Contain("usb=()");
    }

    [Fact]
    public async Task Cross_origin_headers_are_correct()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        context.Response.Headers["Cross-Origin-Opener-Policy"]
            .ToString()
            .Should()
            .Be("same-origin");

        context.Response.Headers["Cross-Origin-Embedder-Policy"]
            .ToString()
            .Should()
            .Be("require-corp");

        context.Response.Headers["Cross-Origin-Resource-Policy"]
            .ToString()
            .Should()
            .Be("same-origin");
    }

    [Fact]
    public async Task Middleware_does_not_throw()
    {
        var middleware = Create();
        var context = new DefaultHttpContext();

        await middleware
            .Invoking(m => m.InvokeAsync(context, _ => Task.CompletedTask))
            .Should()
            .NotThrowAsync();
    }
}
