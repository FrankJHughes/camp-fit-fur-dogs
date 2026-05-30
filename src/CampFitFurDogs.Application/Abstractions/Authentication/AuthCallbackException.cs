using System;

namespace CampFitFurDogs.Application.Abstractions.Authentication;

public sealed class AuthCallbackException : Exception
{
    public AuthCallbackError Error { get; }

    public AuthCallbackException(AuthCallbackError error)
        : base(error.ToString())
    {
        Error = error;
    }
}
