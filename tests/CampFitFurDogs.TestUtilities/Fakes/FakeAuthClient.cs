using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.TestUtilities.Fakes;

public sealed class FakeAuthClient : IAuthClient
{
    //
    // CONFIGURATION PROPERTIES
    //

    /// <summary>
    /// The token that ExchangeAsync should return.
    /// If null, ExchangeAsync will throw InvalidOperationException.
    /// </summary>
    public AuthToken? TokenToReturn { get; set; }

    /// <summary>
    /// The user that GetUserAsync should return.
    /// If null, GetUserAsync will return null (allowing tests to assert failure paths).
    /// </summary>
    public AuthUser? UserToReturn { get; set; }

    /// <summary>
    /// Optional exception to throw from ExchangeAsync.
    /// </summary>
    public Exception? ExchangeException { get; set; }

    /// <summary>
    /// Optional exception to throw from GetUserAsync.
    /// </summary>
    public Exception? GetUserException { get; set; }

    //
    // CAPTURED INPUTS (for assertions)
    //

    public string? LastAuthorizationCode { get; private set; }
    public AuthToken? LastTokenReceived { get; private set; }

    //
    // IMPLEMENTATION
    //

    public Task<AuthToken> ExchangeAsync(string authorizationCode, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        LastAuthorizationCode = authorizationCode;

        if (ExchangeException is not null)
            throw ExchangeException;

        if (TokenToReturn is null)
            throw new InvalidOperationException("FakeAuthClient.TokenToReturn was not set.");

        return Task.FromResult(TokenToReturn);
    }

    public Task<AuthUser> GetUserAsync(AuthToken token, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        LastTokenReceived = token;

        if (GetUserException is not null)
            throw GetUserException;

        // Returning null is allowed — tests use this to assert failure paths.
        return Task.FromResult(UserToReturn!);
    }
}
