namespace Frank.Identity.Application.Abstractions.Oidc;

public sealed record OidcUserInfo(
    string Subject,
    string? Email,
    string? GivenName,
    string? FamilyName,
    string? Picture,
    IReadOnlyDictionary<string, string> Claims
);
