namespace Frank.Identity.Application.Callback.Oidc;

public sealed class OidcProtocolException : Exception
{
    public OidcProtocolException(string message) : base(message) { }
}
