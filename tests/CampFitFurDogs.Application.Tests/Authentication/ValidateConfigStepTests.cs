using CampFitFurDogs.Application.Abstractions.Authentication.Oidc;
using CampFitFurDogs.Application.Authentication;
using CampFitFurDogs.Application.Authentication.Steps;
using CampFitFurDogs.Domain.Errors;
using Microsoft.Extensions.Options;

namespace CampFitFurDogs.Application.Tests.Authentication
{
    public sealed class ValidateConfigStepTests
    {
        private static ValidateConfigStep CreateStep(OidcOptions options)
        {
            return new ValidateConfigStep(Options.Create(options));
        }

        private static AuthCallbackContext DummyContext()
        {
            return new AuthCallbackContext("dummy-code");
        }

        // ------------------------------------------------------------
        // INVALID CONFIGURATION CASES
        // ------------------------------------------------------------

        [Fact]
        public async Task Missing_domain_throws_BadConfigurationException()
        {
            var options = new OidcOptions
            {
                Authority = "",
                ClientId = "id",
                ClientSecret = "secret",
                CallbackUrl = "http://localhost/callback",
                PostLoginRedirectUrl = "http://localhost/"
            };

            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(DummyContext(), CancellationToken.None);

            await act.Should()
                .ThrowAsync<BadConfigurationException>()
                .WithMessage("Auth0 configuration is incomplete");
        }

        [Fact]
        public async Task Missing_client_id_throws_BadConfigurationException()
        {
            var options = new OidcOptions
            {
                Authority = "example.auth0.com",
                ClientId = "",
                ClientSecret = "secret",
                CallbackUrl = "http://localhost/callback",
                PostLoginRedirectUrl = "http://localhost/"
            };

            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(DummyContext(), CancellationToken.None);

            await act.Should()
                .ThrowAsync<BadConfigurationException>()
                .WithMessage("Auth0 configuration is incomplete");
        }

        [Fact]
        public async Task Missing_client_secret_throws_BadConfigurationException()
        {
            var options = new OidcOptions
            {
                Authority = "example.auth0.com",
                ClientId = "id",
                ClientSecret = "",
                CallbackUrl = "http://localhost/callback",
                PostLoginRedirectUrl = "http://localhost/"
            };

            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(DummyContext(), CancellationToken.None);

            await act.Should()
                .ThrowAsync<BadConfigurationException>()
                .WithMessage("Auth0 configuration is incomplete");
        }

        [Fact]
        public async Task Missing_callback_url_throws_BadConfigurationException()
        {
            var options = new OidcOptions
            {
                Authority = "example.auth0.com",
                ClientId = "id",
                ClientSecret = "secret",
                CallbackUrl = "",
                PostLoginRedirectUrl = "http://localhost/"
            };

            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(DummyContext(), CancellationToken.None);

            await act.Should()
                .ThrowAsync<BadConfigurationException>()
                .WithMessage("Auth0 configuration is incomplete");
        }

        [Fact]
        public async Task Missing_post_login_redirect_url_throws_BadConfigurationException()
        {
            var options = new OidcOptions
            {
                Authority = "example.auth0.com",
                ClientId = "id",
                ClientSecret = "secret",
                CallbackUrl = "http://localhost/callback",
                PostLoginRedirectUrl = ""
            };

            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(DummyContext(), CancellationToken.None);

            await act.Should()
                .ThrowAsync<BadConfigurationException>()
                .WithMessage("Auth0 configuration is incomplete");
        }

        // ------------------------------------------------------------
        // VALID CONFIGURATION CASE
        // ------------------------------------------------------------

        [Fact]
        public async Task Valid_configuration_does_not_throw()
        {
            var options = new OidcOptions
            {
                Authority = "example.auth0.com",
                ClientId = "id",
                ClientSecret = "secret",
                CallbackUrl = "http://localhost/callback",
                PostLoginRedirectUrl = "http://localhost/"
            };

            var step = CreateStep(options);

            var act = () => step.ExecuteAsync(DummyContext(), CancellationToken.None);

            await act.Should().NotThrowAsync();
        }
    }
}
