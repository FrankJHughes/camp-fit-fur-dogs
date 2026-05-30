using System;
using System.Threading;
using System.Threading.Tasks;
using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace CampFitFurDogs.Application.Tests.Authentication
{
    public sealed class BuildRedirectStepTests
    {
        private static OidcOptions OptionsWithRedirect(string url) =>
            new OidcOptions
            {
                Authority = "example.auth0.com",
                ClientId = "client123",
                ClientSecret = "secret456",
                CallbackUrl = "http://localhost/callback",
                PostLoginRedirectUrl = url
            };

        private static AuthCallbackContext ContextWithResult(Guid customerId)
        {
            return new AuthCallbackContext("dummy-code")
            {
                Result = AuthCallbackResult.CreateSuccess(customerId)
            };
        }

        private static BuildRedirectStep CreateStep(OidcOptions options)
            => new BuildRedirectStep(Options.Create(options));

        // ------------------------------------------------------------
        // SUCCESSFUL REDIRECT UPDATE
        // ------------------------------------------------------------
        [Fact]
        public async Task Updates_redirect_url_on_existing_result()
        {
            var customerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var ctx = ContextWithResult(customerId);

            var options = OptionsWithRedirect("https://example.com/after-login");
            var step = CreateStep(options);

            await step.ExecuteAsync(ctx, CancellationToken.None);

            ctx.Result.Should().NotBeNull();
            ctx.Result!.CustomerId.Should().Be(customerId);
            ctx.Result.SessionCookie.Should().Be($"cfd.session={customerId}");
            ctx.Result.RedirectUrl.Should().Be("https://example.com/after-login");
        }

        // ------------------------------------------------------------
        // NULL RESULT → NULL REFERENCE EXCEPTION
        // ------------------------------------------------------------
        [Fact]
        public async Task Null_result_throws_NullReferenceException()
        {
            var ctx = new AuthCallbackContext("dummy-code")
            {
                Result = null
            };

            var options = OptionsWithRedirect("https://example.com/after-login");
            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(ctx, CancellationToken.None);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}
