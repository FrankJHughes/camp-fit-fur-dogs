using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SharedKernel.Api;
using CampFitFurDogs.Application.Abstractions.Identity.External;
using CampFitFurDogs.Application.Abstractions.Audit;
using CampFitFurDogs.Domain.Errors;

namespace CampFitFurDogs.Api.Authentication;

public class AuthCallbackEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/callback", async (
            HttpContext http,
            IHostEnvironment env,
            [FromServices] IExternalIdentityResolver externalUserMapper,
            [FromServices] HttpClient httpClient,
            [FromServices] IAuditLogger auditLogger) =>
        {
            var config = http.RequestServices.GetRequiredService<IConfiguration>();
            var domain = config["Auth0:Domain"];
            var clientId = config["Auth0:ClientId"];
            var clientSecret = config["Auth0:ClientSecret"];
            var callbackUrl = config["Auth0:CallbackUrl"];
            var postLoginRedirect = config["Auth0:PostLoginRedirectUrl"];

            if (string.IsNullOrWhiteSpace(domain) ||
                string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret) ||
                string.IsNullOrWhiteSpace(callbackUrl) ||
                string.IsNullOrWhiteSpace(postLoginRedirect))
            {
                throw new BadConfigurationException("Auth0 configuration is incomplete.");
            }

            // Extract query parameters
            var code = http.Request.Query["code"].ToString();
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BadRequestException("Missing authorization code.");
            }

            // Exchange authorization code for tokens
            var tokenEndpoint = $"https://{domain}/oauth/token";

            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["redirect_uri"] = callbackUrl
            };

            var tokenResponse = await httpClient.PostAsync(
                tokenEndpoint,
                new FormUrlEncodedContent(tokenRequest));

            if (!tokenResponse.IsSuccessStatusCode)
                throw new ExternalAuthProviderException("Failed to exchange authorization code.");

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);

            var accessToken = tokenData.GetProperty("access_token").GetString();
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ExternalAuthProviderException("Missing access token.");

            // Call /userinfo to get Auth0 user profile
            var userInfoEndpoint = $"https://{domain}/userinfo";

            using var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
            userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var userInfoResponse = await httpClient.SendAsync(userInfoRequest);
            if (!userInfoResponse.IsSuccessStatusCode)
                throw new ExternalAuthProviderException("Failed to retrieve user profile.");

            var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<JsonElement>(userInfoJson);

            var auth0UserId = userInfo.GetProperty("sub").GetString();
            if (string.IsNullOrWhiteSpace(auth0UserId))
                throw new ExternalAuthProviderException("Missing Auth0 user identifier.");

            var auth0FirstName = userInfo.GetProperty("given_name").GetString();
            if (string.IsNullOrWhiteSpace(auth0FirstName))
                throw new ExternalAuthProviderException("Missing Auth0 first name.");

            var auth0LastName = userInfo.GetProperty("family_name").GetString();
            if (string.IsNullOrWhiteSpace(auth0LastName))
                throw new ExternalAuthProviderException("Missing Auth0 last name.");

            var auth0Email = userInfo.GetProperty("email").GetString();
            if (string.IsNullOrWhiteSpace(auth0Email))
                throw new ExternalAuthProviderException("Missing Auth0 email.");

            var customerId = await externalUserMapper.ResolveAsync(
                auth0UserId,
                auth0FirstName,
                auth0LastName,
                auth0Email,
                CancellationToken.None);

            // ⭐ Required by US‑110: audit the login event
            await auditLogger.LoginSucceeded(customerId, auth0UserId);

            // Issue session cookie
            http.Response.Cookies.Append(
                "cfd.session",
                customerId.ToString(),
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = env.IsProduction(),
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                });

            return Results.Redirect(postLoginRedirect);
        });
    }
}
