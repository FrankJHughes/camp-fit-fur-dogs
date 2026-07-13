namespace Frank.Identity.Application.Callback.Oidc;

public sealed record OidcUserInfo(
    string Subject,
    string? Email,
    string? GivenName,
    string? FamilyName,
    string? Picture,
    IReadOnlyDictionary<string, string> Claims
);
