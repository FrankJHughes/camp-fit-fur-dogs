namespace Frank.Authentication.Callback.Oidc;

public sealed class OidcProtocolException : Exception
{
    public OidcProtocolException(string message) : base(message) { }
}
