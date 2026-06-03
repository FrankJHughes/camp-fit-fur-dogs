using CampFitFurDogs.Application.Abstractions.Authentication;
using CampFitFurDogs.Domain.Authentication.Sessions;

public sealed class AuthCallbackContextBuilder
{
    private string _code = "test-code";
    private AuthToken? _token;
    private AuthUser? _user;
    private Guid? _customerId;
    private SessionTokenHash? _tokenHash;
    private Session? _session;
    private SessionCookie? _cookie;
    private string? _redirect;
    private DateTimeOffset _now = DateTimeOffset.UtcNow;

    public void Code(string value) => _code = value;
    public void Token(AuthToken value) => _token = value;
    public void User(AuthUser value) => _user = value;
    public void CustomerId(Guid value) => _customerId = value;
    public void TokenHash(SessionTokenHash value) => _tokenHash = value;
    public void Session(Session value) => _session = value;
    public void SessionCookie(SessionCookie value) => _cookie = value;
    public void RedirectUrl(string value) => _redirect = value;
    public void Now(DateTimeOffset value) => _now = value;

    public AuthCallbackContext Build()
        => new AuthCallbackContext(
            Code: _code,
            Token: _token,
            User: _user,
            CustomerId: _customerId,
            TokenHash: _tokenHash,
            Session: _session,
            SessionCookie: _cookie,
            RedirectUrl: _redirect,
            Now: _now
        );
}
