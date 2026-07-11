namespace Frank.Application.Abstractions.Identity;

public sealed class AuthCallbackException : System.Exception
{
    public AuthCallbackError Error { get; }

    public AuthCallbackException(AuthCallbackError error)
        : base(error.ToString())
    {
        Error = error;
    }
}
