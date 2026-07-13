namespace CampFitFurDogs.Application.Exceptions;

public sealed class UserNotAuthenticatedException : System.Exception
{
    public UserNotAuthenticatedException()
        : base("User not authenticated.") { }
}
