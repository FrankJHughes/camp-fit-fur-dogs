using CampFitFurDogs.Application.Abstractions.Authentication;

namespace CampFitFurDogs.Application.Authentication;

public sealed class AuthCallbackContext
{
    public string Code { get; }
    public AuthToken? Token { get; set; }
    public AuthUser? User { get; set; }
    public Guid? CustomerId { get; set; }
    public AuthCallbackResult? Result { get; set; }

    public AuthCallbackContext(string code)
    {
        Code = code;
    }
}
