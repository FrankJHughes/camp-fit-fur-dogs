namespace Frank.Identity.Application.Abstractions;

public sealed class AuthCallbackException : Exception
{
    public AuthCallbackError Error { get; }

    public AuthCallbackException(AuthCallbackError error)
        : base(error.ToString())
    {
        Error = error;
    }
}
